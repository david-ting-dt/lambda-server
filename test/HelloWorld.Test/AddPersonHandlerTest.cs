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
    public class AddPersonHandlerTest
    {
        private readonly Mock<IDbHandler> _mockDbHandler;
        private readonly Mock<ILogger> _mockLogger;
        private readonly APIGatewayProxyRequest _fakeRequest = new APIGatewayProxyRequest { Body = "Name_to_add" };

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
        public async Task AddPerson_ShouldCallLoggerLogMethodAtLeastOnce()
        {
            var handler = new AddPersonHandler(_mockDbHandler.Object, _mockLogger.Object);
            await handler.AddPerson(_fakeRequest);
            _mockLogger.Verify(logger => logger.Log(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task AddPerson_ShouldReturnResponseStatusCode200_IfSuccessfullyAdded()
        {
            _mockDbHandler.Setup(db => db.GetPeopleAsync())
                .ReturnsAsync(new List<Person>());
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
    }
}