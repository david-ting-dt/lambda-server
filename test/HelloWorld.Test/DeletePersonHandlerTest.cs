using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;
using Moq;
using Xunit;

namespace HelloWorld.Tests
{
    public class DeletePersonHandlerTest
    {
        private readonly Mock<IDbHandler> _mockDbHandler;
        private readonly Mock<ILogger> _mockLogger;
        private readonly APIGatewayProxyRequest _deleteRequest = new APIGatewayProxyRequest
        {
            PathParameters = new Dictionary<string, string>{{"id", "1"}}
        };

        public DeletePersonHandlerTest()
        {
            _mockDbHandler = new Mock<IDbHandler>();
            _mockLogger = new Mock<ILogger>();
        }

        [Fact]
        public async Task DeletePerson_ShouldCallDbHandlerDeleteAsyncOnce()
        {
            var handler = new DeletePersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            await handler.DeletePerson(_deleteRequest);
            _mockDbHandler.Verify(db => db.DeletePersonAsync("1"), Times.Once);
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnResponseStatusCode204_IfDeleteSuccessfully()
        {
            var handler = new DeletePersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            var response = await handler.DeletePerson(_deleteRequest);
            Assert.Equal(204, response.StatusCode);
        }

        [Fact]
        public async Task DeletePerson_ShouldCallLoggerLogMethodAtLeastOnce()
        {
            var handler = new DeletePersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            await handler.DeletePerson(_deleteRequest);
            _mockLogger.Verify(logger => logger.Log(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnResponseStatusCode500_IfExceptionIsThrown()
        {
            _mockDbHandler
                .Setup(db => db.DeletePersonAsync(It.IsAny<string>()))
                .Throws(new Exception());
            var handler = new DeletePersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            var response = await handler.DeletePerson(_deleteRequest);
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnResponseStatusCode404_IfNullReferenceExceptionCaught()
        {
            _mockDbHandler
                .Setup(db => db.DeletePersonAsync(It.IsAny<string>()))
                .Throws(new NullReferenceException());
            var handler = new DeletePersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            var response = await handler.DeletePerson(_deleteRequest);
            Assert.Equal(404, response.StatusCode);
        }
    }
}