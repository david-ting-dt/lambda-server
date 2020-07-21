using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.DbItem;
using HelloWorld.Interfaces;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace HelloWorld.Tests
{
    public class DeletePersonHandlerTest
    {
        private readonly Mock<IDbHandler> _mockDbHandler;
        private readonly Mock<ILogger> _mockLogger;
        private readonly APIGatewayProxyRequest _deleteRequest = new APIGatewayProxyRequest
        {
            HttpMethod = "DELETE",
            Path = "/people/1",
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
        public async Task DeletePerson_ShouldReturnExpectedResponse_IfDeleteSuccessfully()
        {
            _mockDbHandler
                .Setup(dbHandler => dbHandler.DeletePersonAsync(It.IsAny<string>()))
                .ReturnsAsync(new Person{Name = "David"});
            var handler = new DeletePersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            var response = await handler.DeletePerson(_deleteRequest);
            var expected = new APIGatewayProxyResponse { StatusCode = 204, Body = "David"};

            var responseJson = JsonConvert.SerializeObject(response);
            var expectedJson = JsonConvert.SerializeObject(expected);
            Assert.Equal(expectedJson, responseJson);
        }

        [Fact]
        public async Task DeletePerson_ShouldLogAPIGatewayRequestReceived()
        {
            var handler = new DeletePersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            await handler.DeletePerson(_deleteRequest);
            _mockLogger.Verify(logger => logger.Log(
                $"API Gateway request received - HttpMethod: {_deleteRequest.HttpMethod}  Path: {_deleteRequest.Path}"),
                Times.Once);
        }

        [Fact]
        public async Task DeletePerson_ShouldLogResponseCreated_IfDeleteSuccessfully()
        {
            _mockDbHandler
                .Setup(dbHandler => dbHandler.DeletePersonAsync(It.IsAny<string>()))
                .ReturnsAsync(new Person{Name = "David"});
            var handler = new DeletePersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            var response = await handler.DeletePerson(_deleteRequest);
            _mockLogger.Verify(logger => logger.Log(
                    $"API Gateway response produced - StatusCode: {response.StatusCode}  Body: {response.Body}"),
                Times.Once);
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

        [Fact]
        public async Task DeletePerson_ShouldLogResponseCreated_IfExceptionThrown()
        {
            _mockDbHandler.Setup(db => db.DeletePersonAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception());
            var handler = new DeletePersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            await Assert.ThrowsAsync<Exception>(async () => await handler.DeletePerson(_deleteRequest));
            _mockLogger.Verify(logger => logger.Log(
                    $"API Gateway response produced - StatusCode: 500  Body: Internal server error"),
                Times.Once);
        }
    }
}