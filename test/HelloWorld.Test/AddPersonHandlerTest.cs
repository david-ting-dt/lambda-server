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

        public AddPersonHandlerTest()
        {
            _mockDbHandler = new Mock<IDbHandler>();
        }

        [Fact]
        public async Task AddPerson_ShouldCallDbHandlerAddPersonAsyncOnce()
        {
            var handler = new AddPersonHandler(_mockDbHandler.Object);
            var request = new APIGatewayProxyRequest { Body = "Name_to_add" };
            await handler.AddPerson(request);
            _mockDbHandler
                .Verify(db => db.AddPersonAsync(It.IsAny<string>(), request.Body), Times.Once);
        }

        [Fact]
        public async Task AddPerson_ShouldReturnResponseStatusCode200_IfSuccessfullyAdded()
        {
            var handler = new AddPersonHandler(_mockDbHandler.Object);
            var request = new APIGatewayProxyRequest { Body = "Name_to_add" };
            var response = await handler.AddPerson(request);
            Assert.Equal(200, response.StatusCode);
        }
        
        [Fact]
        public async Task AddPerson_ShouldReturnResponseStatusCode400_IfRequestBodyIsNotValid()
        {
            var handler = new AddPersonHandler(_mockDbHandler.Object);
            var request = new APIGatewayProxyRequest { Body = "the_length_of_the_request_body_is_greater_than_30" };
            var response = await handler.AddPerson(request);
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async Task AddPerson_ShouldReturnResponseStatusCode500_IfExceptionIsThrown()
        {
            _mockDbHandler
                .Setup(db => db.AddPersonAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());
            var handler = new AddPersonHandler(_mockDbHandler.Object);
            var request = new APIGatewayProxyRequest { Body = "Name_To_ADd"};
            var response = await handler.AddPerson(request);
            Assert.Equal(500, response.StatusCode);
        }
    }
}