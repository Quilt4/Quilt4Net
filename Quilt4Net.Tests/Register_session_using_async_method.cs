using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Quilt4Net.Core;
using Quilt4Net.Core.DataTransfer;
using Quilt4Net.Core.Interfaces;

namespace Quilt4Net.Tests
{
    [TestFixture]
    public class Register_session_using_async_method
    {
        [Test]
        public async Task When_registering_session_with_no_projectApiKey_set()
        {
            //Arrange
            var configurationMock = new Mock<IConfiguration>(MockBehavior.Strict);
            configurationMock.SetupGet(x => x.ProjectApiKey).Returns((string)null);
            configurationMock.SetupGet(x => x.Session.Environment).Returns("Test");
            configurationMock.SetupGet(x => x.Target.Location).Returns("http://localhost");
            configurationMock.SetupGet(x => x.AllowMultipleInstances).Returns(true);

            var webApiClientMock = new Mock<IWebApiClient>(MockBehavior.Strict);
            webApiClientMock.Setup(x => x.CreateAsync<SessionRequest, SessionResponse>(It.IsAny<string>(), It.IsAny<SessionRequest>())).Returns(Task.FromResult(default(SessionResponse)));

            var session = Register_session_setup.GivenThereIsASession(webApiClientMock, configurationMock, null, null);

            ExpectedIssues.ProjectApiKeyNotSetException exception = null;
            SessionResult result = null;

            //Act
            try
            {
                result = await session.RegisterAsync();
            }
            catch (ExpectedIssues.ProjectApiKeyNotSetException exp)
            {
                exception = exp;
            }

            //Assert
            Assert.That(exception, Is.Not.Null);
            Assert.That(result, Is.Null);
            webApiClientMock.Verify(x => x.CreateAsync<SessionRequest, SessionResponse>(It.IsAny<string>(), It.IsAny<SessionRequest>()), Times.Never);
        }

        [Test]
        public async Task When_registering_session()
        {
            //Arrange
            var configurationMock = new Mock<IConfiguration>(MockBehavior.Strict);
            configurationMock.SetupGet(x => x.Enabled).Returns(true);
            configurationMock.SetupGet(x => x.ProjectApiKey).Returns("ABC123");
            configurationMock.SetupGet(x => x.Session.Environment).Returns("Test");
            configurationMock.SetupGet(x => x.AllowMultipleInstances).Returns(true);

            var webApiClientMock = new Mock<IWebApiClient>(MockBehavior.Strict);
            webApiClientMock.Setup(x => x.CreateAsync<SessionRequest,SessionResponse>(It.IsAny<string>(), It.IsAny<SessionRequest>())).Returns(Task.FromResult(new SessionResponse { SessionKey = Guid.NewGuid().ToString() }));

            var session = Register_session_setup.GivenThereIsASession(webApiClientMock, configurationMock, null, null);

            //Act
            var response = await session.RegisterAsync();

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.IsSuccess, Is.EqualTo(true));
            Assert.That(response.ErrorMessage, Is.Null);
            Assert.That(response.Elapsed.Ticks, Is.GreaterThan(1));
            webApiClientMock.Verify(x => x.CreateAsync<SessionRequest, SessionResponse>(It.IsAny<string>(), It.IsAny<SessionRequest>()), Times.Once);
        }
    }
}