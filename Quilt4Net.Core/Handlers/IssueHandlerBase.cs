using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quilt4Net.Core.DataTransfer;
using Quilt4Net.Core.Events;
using Quilt4Net.Core.Interfaces;

namespace Quilt4Net.Core
{
    public abstract class IssueHandlerBase : IIssueHandler
    {
        private const string IssueThreadKeyName = "IssueThreadKey";
        private readonly object _syncRoot = new object();
        private static int _instanceCounter;
        private readonly ISessionHandler _sessionHandler;
        private readonly Queue<Tuple<IssueRequest, Exception>> _issuesThatFailedToRegister = new Queue<Tuple<IssueRequest, Exception>>();

        protected internal IssueHandlerBase(ISessionHandler sessionHandler)
        {
            lock (_syncRoot)
            {
                if (_instanceCounter != 0)
                {
                    if (!sessionHandler.Client.Configuration.AllowMultipleInstances)
                    {
                        throw new InvalidOperationException("Multiple instances is not allowed. Set configuration setting AllowMultipleInstances to true if you want to use multiple instances of this object.");
                    }
                }
                _instanceCounter++;
            }

            _sessionHandler = sessionHandler;
        }

        public IQuilt4NetClient Client => _sessionHandler.Client;
        public event EventHandler<IssueRegistrationStartedEventArgs> IssueRegistrationStartedEvent;
        public event EventHandler<IssueRegistrationCompletedEventArgs> IssueRegistrationCompletedEvent;

        public async Task<IssueResult> RegisterAsync(string message, MessageIssueLevel issueLevel, string userHandle = null, IDictionary<string, string> data = null)
        {
            var sessionKey = await _sessionHandler.GetSessionKeyAsync();
            var issueData = PrepareIssueData(sessionKey, message, issueLevel, userHandle, data);
            return await RegisterEx(true, issueData);
        }

        public void RegisterStart(string message, MessageIssueLevel issueLevel, string userHandle = null, IDictionary<string, string> data = null)
        {
            Task.Run(async () =>
                {
                    var sessionKey = await _sessionHandler.GetSessionKeyAsync();
                    var issueData = PrepareIssueData(sessionKey, message, issueLevel, userHandle, data);
                    await RegisterEx(false, issueData);
                });
        }

        public IssueResult Register(string message, MessageIssueLevel issueLevel, string userHandle = null, IDictionary<string, string> data = null)
        {
            try
            {
                var sessionKey = _sessionHandler.GetSessionKeyAsync().Result;
                var issueData = PrepareIssueData(sessionKey, message, issueLevel, userHandle, data);
                var response = RegisterEx(true, issueData).Result;
                return response;
            }
            catch (AggregateException exception)
            {
                throw exception.InnerException;
            }
        }

        public async Task<IssueResult> RegisterAsync(Exception exception, ExceptionIssueLevel issueLevel = ExceptionIssueLevel.Error, string userHandle = null)
        {
            HandleIssueThreadKey(exception);

            var sessionKey = await _sessionHandler.GetSessionKeyAsync();
            var issueData = PrepareIssueData(sessionKey, exception, issueLevel, userHandle);
            var respnse = await RegisterEx(true, issueData);
            return respnse;
        }

        public void RegisterStart(Exception exception, ExceptionIssueLevel issueLevel = ExceptionIssueLevel.Error, string userHandle = null)
        {
            HandleIssueThreadKey(exception);

            Task.Run(async () =>
                {
                    var sessionKey = await _sessionHandler.GetSessionKeyAsync();
                    var issueData = PrepareIssueData(sessionKey, exception, issueLevel, userHandle);
                    await RegisterEx(false, issueData);
                });
        }

        public IssueResult Register(Exception exception, ExceptionIssueLevel issueLevel = ExceptionIssueLevel.Error, string userHandle = null)
        {
            HandleIssueThreadKey(exception);

            try
            {
                var sessionKey = _sessionHandler.GetSessionKeyAsync().Result;
                var issueData = PrepareIssueData(sessionKey, exception, issueLevel, userHandle);
                var response = RegisterEx(true, issueData).Result;
                return response;
            }
            catch (AggregateException exp)
            {
                throw exp.InnerException;
            }
        }

        public async Task<IEnumerable<IssueTypeResponse>> GetIssueTypesAsync(Guid versionKey)
        {
            return await _sessionHandler.Client.WebApiClient.ExecuteQueryAsync<Guid, IEnumerable<IssueTypeResponse>>("Client/IssueType", versionKey.ToString());
        }

        public async Task<IEnumerable<IssueResponse>> GetIssuesAsync(Guid versionKey)
        {
            return await _sessionHandler.Client.WebApiClient.ExecuteQueryAsync<Guid, IEnumerable<IssueResponse>>("Client/Issue", versionKey.ToString());
        }

        private async Task<IssueResult> RegisterEx(bool doThrow, IssueRequest request)
        {
            if (string.IsNullOrEmpty(request.SessionKey))
                throw new ArgumentException("No SessionKey has been assigned.");

            var result = new IssueResult();
            IssueResponse response = null;

            try
            {
                OnIssueRegistrationStartedEvent(new IssueRegistrationStartedEventArgs(request));

                response = await _sessionHandler.Client.WebApiClient.CreateAsync<IssueRequest, IssueResponse>("Client/Issue", request);
            }
            catch (Exception exception)
            {
                result.SetException(exception);
                _issuesThatFailedToRegister.Enqueue(new Tuple<IssueRequest, Exception>(request, exception));

                if (doThrow)
                    throw;
            }
            finally
            {
                result.SetCompleted(response);
                OnIssueRegistrationCompletedEvent(new IssueRegistrationCompletedEventArgs(request, result));
            }

            return result;
        }

        private IssueRequest PrepareIssueData(string sessionKey, Exception exception, ExceptionIssueLevel issueLevel, string userHandle)
        {
            var level = issueLevel.ToString();
            var issueType = CreateIssueTypeData(exception);

            var issueData = new IssueRequest
                                {
                                    IssueLevel = level,
                                    UserHandle = userHandle,
                                    ClientTime = DateTime.UtcNow,
                                    IssueKey = Guid.NewGuid(),
                                    IssueThreadKey = HandleIssueThreadKey(exception),
                                    IssueType = issueType,
                                    SessionKey = sessionKey,                                    
                                };

            return issueData;
        }

        private static Dictionary<string, string> GetExceptionData(Exception exception)
        {
            return exception.Data.Cast<DictionaryEntry>().Where(x => x.Value != null && x.Key.ToString() != IssueThreadKeyName).ToDictionary(item => item.Key.ToString(), item => item.Value.ToString());
        }

        private IssueTypeData CreateIssueTypeData(Exception exception)
        {
            var issueTypes = GetInnerIssueTypes(exception).ToArray();
            var exceptionData = GetExceptionData(exception);

            var issueType = new IssueTypeData
                                {
                                    Message = exception.Message,
                                    InnerIssueTypes = issueTypes,
                                    StackTrace = exception.StackTrace,
                                    Type = exception.GetType().ToString(),
                                    Data = exceptionData
                                };
            return issueType;
        }

        private IEnumerable<IssueTypeData> GetInnerIssueTypes(Exception exception)
        {
            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                foreach (var inner in (aggregateException.InnerExceptions))
                {
                    yield return exception.InnerException != null ? CreateIssueTypeData(inner) : null;
                }
            }
            else
            {
                if (exception.InnerException != null)
                    yield return CreateIssueTypeData(exception.InnerException);
            }
        }

        private IssueRequest PrepareIssueData(string sessionKey, string message, MessageIssueLevel issueLevel, string userHandle, IDictionary<string, string> data)
        {
            var issueType = new IssueTypeData
                                {
                                    Message = message,
                                    InnerIssueTypes = null,
                                    StackTrace = null,
                                    Type = "Message",
                                    Data = data
                                };

            var issueData = new IssueRequest
                                {
                                    UserHandle = userHandle,
                                    ClientTime = DateTime.UtcNow,
                                    IssueKey = Guid.NewGuid(),
                                    IssueThreadKey = null,
                                    IssueType = issueType,
                                    IssueLevel = issueLevel.ToString(),
                                    SessionKey = sessionKey,                                    
                                };

            return issueData;
        }

        private static Guid HandleIssueThreadKey(Exception exception)
        {
            var refItg = Guid.NewGuid();
            if (exception == null) return refItg;

            lock (exception)
            {
                if (!exception.Data.Contains(IssueThreadKeyName))
                {
                    if (exception.InnerException != null)
                    {
                        var anotherRefItg = GetIssueThreadKey(exception.InnerException);
                        refItg = anotherRefItg ?? refItg;
                    }

                    //Check all levels of inner exceptions to get the key
                    exception.Data.Add(IssueThreadKeyName, refItg);
                }
                else
                {
                    Guid result;
                    if (Guid.TryParse(exception.Data[IssueThreadKeyName].ToString(), out result))
                    {
                        refItg = result;
                    }
                    else
                    {
                        //NOTE: When there is a general message/warning event. Fire this information.
                        //Provided IssueThreadKey cannot be parsed as Guid. Apply a new valid value.
                        exception.Data[IssueThreadKeyName] = refItg;
                    }
                }

                ClearIssueThreadKey(exception.InnerException);
            }

            return refItg;
        }       

        private static Guid? GetIssueThreadKey(Exception exception)
        {
            if (exception.Data.Contains(IssueThreadKeyName))
            {
                Guid result;
                if (Guid.TryParse(exception.Data[IssueThreadKeyName].ToString(), out result))
                {
                    return result;
                }
            }

            if (exception.InnerException != null)
            {
                return GetIssueThreadKey(exception.InnerException);
            }

            return null;
        }

        private static void ClearIssueThreadKey(Exception exception)
        {
            if (exception == null) return;

            if (exception.Data.Contains(IssueThreadKeyName))
            {
                exception.Data.Remove(IssueThreadKeyName);
            }

            if (exception.InnerException != null)
            {
                ClearIssueThreadKey(exception.InnerException);
            }
        }

        protected virtual void OnIssueRegistrationStartedEvent(IssueRegistrationStartedEventArgs e)
        {
            IssueRegistrationStartedEvent?.Invoke(this, e);
        }

        protected virtual void OnIssueRegistrationCompletedEvent(IssueRegistrationCompletedEventArgs e)
        {
            IssueRegistrationCompletedEvent?.Invoke(this, e);
        }
    }
}