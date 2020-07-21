using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;
using Moq;
using Xunit;

namespace HelloWorld.Tests
{
    public class AddPersonHandlerTest
    {
        private readonly Mock<IDbHandler> _mockDbHandler;
        private readonly Mock<ILogger> _mockLogger;
        private readonly APIGatewayProxyRequest _fakeRequest = new APIGatewayProxyRequest
        {
            HttpMethod = "PUT",
            Body = "Name_to_add",
            Path = "/people"
        };

        public AddPersonHandlerTest()
        {
            _mockDbHandler = new Mock<IDbHandler>();
            _mockLogger = new Mock<ILogger>();
        }

        [Fact]
        public async Task AddPerson_ShouldCallDbHandlerAddPersonAsyncOnce()
        {
            var handler = new AddPersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            await handler.AddPerson(_fakeRequest);
            _mockDbHandler
                .Verify(db => db.AddPersonAsync(It.IsAny<string>(), _fakeRequest.Body), Times.Once);
        }

        [Fact]
        public async Task AddPerson_ShouldLogReceivedAPIGatewayRequest()
        {
            var handler = new AddPersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            await handler.AddPerson(_fakeRequest);
            _mockLogger
                .Verify(logger => logger.Log(
                        $"API Gateway request received - HttpMethod: {_fakeRequest.HttpMethod}  " +
                        $"Path: {_fakeRequest.Path}  Body:{_fakeRequest.Body}")
                    , Times.Once);
        }
        
        [Fact]
        public async Task AddPerson_ShouldLogAPIGatewayResponseCreated_IfSuccessful()
        {
            var handler = new AddPersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            var response = await handler.AddPerson(_fakeRequest);
            _mockLogger
                .Verify(logger => logger.Log(
                        $"API Gateway response produced - StatusCode: {response.StatusCode}  Body: {response.Body}" +
                        $"  Location: {response.Headers["Location"]}")
                    , Times.Once);
        }

        [Fact]
        public async Task AddPerson_ShouldReturnResponseStatusCode200_IfSuccessfullyAdded()
        {
            var handler = new AddPersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            var response = await handler.AddPerson(_fakeRequest);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async Task AddPerson_ShouldReturnResponseStatusCode400_IfRequestBodyIsNotValid()
        {
            var handler = new AddPersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            var request = new APIGatewayProxyRequest { Body = "the_length_of_the_request_body_is_greater_than_30" };
            var response = await handler.AddPerson(request);
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async Task AddPerson_ShouldLogAPIGatewayResponseCreated_IfRequestBodyIsNotValid()
        {
            var handler = new AddPersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            var request = new APIGatewayProxyRequest { Body = "the_length_of_the_request_body_is_greater_than_30" };
            var response = await handler.AddPerson(request);
            _mockLogger.Verify(logger => logger.Log(
                $"API Gateway response produced - StatusCode: {response.StatusCode}  Body: {response.Body}"),
                Times.Once);
        }

        [Fact]
        public async Task AddPerson_ShouldLogResponseCreated_IfExceptionThrown()
        {
            _mockDbHandler
                .Setup(db => db.AddPersonAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());
            var handler = new AddPersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            await Assert.ThrowsAsync<Exception>(async () => await handler.AddPerson(_fakeRequest));
            _mockLogger.Verify(logger => logger.Log(
                $"API Gateway response produced - StatusCode: 500  Body: Internal server error"));
            
        }
    }
}