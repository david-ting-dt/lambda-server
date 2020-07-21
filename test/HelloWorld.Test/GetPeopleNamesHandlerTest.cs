using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.DbItem;
using HelloWorld.Interfaces;
using Moq;
using Xunit;

namespace HelloWorld.Tests
{
    public class GetPeopleNamesHandlerTest
    {
        private readonly Mock<IDbHandler> _mockDbHandler;
        private readonly Mock<ILogger> _mockLogger;
        private readonly APIGatewayProxyRequest _fakeRequest = new APIGatewayProxyRequest
        {
            HttpMethod = "Get", 
            Path = "/people"
        };
        private readonly List<Person> _people = new List<Person>
        {
            new Person { Id = "1", Name = "David" },
            new Person { Id = "2", Name = "Michael" },
            new Person { Id = "3", Name = "Will" }
        };
        
        public GetPeopleNamesHandlerTest()
        {
            _mockDbHandler = new Mock<IDbHandler>();
            _mockLogger = new Mock<ILogger>();
        }

        [Fact]
        public async Task GetPeopleNames_ShouldCallDbHandlerGetPeopleAsyncOnce()
        {
            SetupMockDbHandlerToReturnFakePeopleData();
            var handler = new GetPeopleNamesHandler(_mockDbHandler.Object, _mockLogger.Object);
            await handler.GetPeopleNames(_fakeRequest);
            _mockDbHandler.Verify(db => db.GetPeopleAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPeopleNames_ShouldLogReceivedAPIGatewayRequest()
        {
            SetupMockDbHandlerToReturnFakePeopleData();
            var handler = new GetPeopleNamesHandler(_mockDbHandler.Object, _mockLogger.Object);
            await handler.GetPeopleNames(_fakeRequest);
            _mockLogger
                .Verify(logger => logger.Log(
                    $"API Gateway request received - HttpMethod: {_fakeRequest.HttpMethod}  Path: {_fakeRequest.Path}")
                , Times.Once);
        }
        
        [Fact]
        public async Task GetPeopleNames_ShouldLogAPIGatewayResponseCreated_IfSuccessful()
        {
            SetupMockDbHandlerToReturnFakePeopleData();
            var handler = new GetPeopleNamesHandler(_mockDbHandler.Object, _mockLogger.Object);
            var response = await handler.GetPeopleNames(_fakeRequest);
            _mockLogger
                .Verify(logger => logger.Log(
                        $"API Gateway response produced - StatusCode: {response.StatusCode}  Body: {response.Body}")
                    , Times.Once);
        }

        [Fact]
        public async Task GetPeopleNames_ShouldReturnResponseStatusCode200_IfSuccessful()
        {
            SetupMockDbHandlerToReturnFakePeopleData();
            var handler = new GetPeopleNamesHandler(_mockDbHandler.Object, _mockLogger.Object);
            var response = await handler.GetPeopleNames(_fakeRequest);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async Task GetPeopleNames_ShouldReturnCorrectResponseBody_IfSuccessful()
        {
            SetupMockDbHandlerToReturnFakePeopleData();
            var handler = new GetPeopleNamesHandler(_mockDbHandler.Object, _mockLogger.Object);
            var response = await handler.GetPeopleNames(_fakeRequest);
            const string expected = "David, Michael, Will";
            Assert.Equal(expected, response.Body);
        }

        private void SetupMockDbHandlerToReturnFakePeopleData()
        {
            _mockDbHandler
                .Setup(db => db.GetPeopleAsync())
                .ReturnsAsync(_people);
        }

        [Fact]
        public async Task GetPeopleNames_ShouldLogAPIGatewayResponseCreated_ExceptionThrown()
        {
            _mockDbHandler
                .Setup(db => db.GetPeopleAsync())
                .ThrowsAsync(new Exception());
            var handler = new GetPeopleNamesHandler(_mockDbHandler.Object, _mockLogger.Object);
            await Assert.ThrowsAsync<Exception>(async () => await handler.GetPeopleNames(_fakeRequest));
            _mockLogger
                .Verify(logger => logger.Log(
                    $"API Gateway response produced - StatusCode: 500  Body: Internal server error"),
                    Times.Once);
        }
    }
}