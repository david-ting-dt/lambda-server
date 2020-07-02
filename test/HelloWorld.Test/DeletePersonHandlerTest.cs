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

        public DeletePersonHandlerTest()
        {
            _mockDbHandler = new Mock<IDbHandler>();
        }

        [Fact]
        public async Task DeletePerson_ShouldCallDbHandlerDeleteAsyncOnce()
        {
            var handler = new DeletePersonHandler(_mockDbHandler.Object);
            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string>{{"id", "1"}}
            };
            await handler.DeletePerson(request);
            _mockDbHandler.Verify(db => db.DeletePersonAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnResponseStatusCode204_IfDeleteSuccessfully()
        {
            var handler = new DeletePersonHandler(_mockDbHandler.Object);
            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string>{{"id", "1"}}
            };
            var response = await handler.DeletePerson(request);
            Assert.Equal(204, response.StatusCode);
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnResponseStatusCode500_IfExceptionIsThrown()
        {
            _mockDbHandler
                .Setup(db => db.DeletePersonAsync(It.IsAny<int>()))
                .Throws(new Exception());
            var handler = new DeletePersonHandler(_mockDbHandler.Object);
            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string>{{"id", "id_to_delete"}}
            };
            var response = await handler.DeletePerson(request);
            Assert.Equal(500, response.StatusCode);
        }
    }
}