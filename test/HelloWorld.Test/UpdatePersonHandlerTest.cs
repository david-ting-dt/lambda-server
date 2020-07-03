using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;
using Moq;
using Xunit;

namespace HelloWorld.Tests
{
    public class UpdatePersonHandlerTest
    {
        private readonly Mock<IDbHandler> _mockDbHandler;

        public UpdatePersonHandlerTest()
        {
            _mockDbHandler = new Mock<IDbHandler>();
        }

        [Fact]
        public async Task UpdatePerson_ShouldCallDbHandlerUpdatePersonAsyncOnce()
        {
            var handler = new UpdatePersonHandler(_mockDbHandler.Object);
            var request = new APIGatewayProxyRequest
            {
                Body = "New_Name",
                PathParameters = new Dictionary<string, string>{ {"id", "1"} }
            };
            await handler.UpdatePerson(request);
            _mockDbHandler.Verify(db => db.UpdatePersonAsync(1, "New_Name"), Times.Once);
        }

        [Fact]
        public async Task UpdatePerson_ShouldReturnResponseStatusCode301_IfUpdateSuccessfully()
        {
            var handler = new UpdatePersonHandler(_mockDbHandler.Object);
            var request = new APIGatewayProxyRequest
            {
                Body = "New_Name",
                PathParameters = new Dictionary<string, string>{ {"id", "1"} }
            };
            var response = await handler.UpdatePerson(request);

            Assert.Equal(301, response.StatusCode);
        }
        
        [Fact]
        public async Task UpdatePerson_ShouldReturnResponseStatusCode400_IfRequestBodyLengthGreaterThan30()
        {
            var handler = new UpdatePersonHandler(_mockDbHandler.Object);
            var request = new APIGatewayProxyRequest
            {
                Body = "the_length_of_the_request_body_is_greater_than_30",
                PathParameters = new Dictionary<string, string>{ {"id", "1"} }
            };
            var response = await handler.UpdatePerson(request);
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePerson_ShouldNotCallDbHandlerUpdatePersonAsync_IfRequestBodyLengthGreaterThan30()
        {
            var handler = new UpdatePersonHandler(_mockDbHandler.Object);
            var request = new APIGatewayProxyRequest
            {
                Body = "the_length_of_the_request_body_is_greater_than_30",
                PathParameters = new Dictionary<string, string>{ {"id", "1"} }
            };
            await handler.UpdatePerson(request);
            _mockDbHandler
                .Verify(db => db.UpdatePersonAsync(1, It.IsAny<string>()),
                    Times.Never);
        }

        [Fact]
        public async Task UpdatePerson_ShouldReturnResponseStatusCode500_IfExceptionIsThrown()
        {
            _mockDbHandler
                .Setup(db => db.UpdatePersonAsync(It.IsAny<int>(), It.IsAny<string>()))
                .Throws(new Exception());
            
            var handler = new UpdatePersonHandler(_mockDbHandler.Object);
            var request = new APIGatewayProxyRequest
            {
                Body = "New_Name",
                PathParameters = new Dictionary<string, string>{ {"id", "id_to_update"} }
            };
            var response = await handler.UpdatePerson(request);
            Assert.Equal(500, response.StatusCode);
        }
    }
}