using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using HelloWorld.Interfaces;
using Moq;
using Xunit;

namespace HelloWorld.Tests
{
    public class DeletePersonHandlerTest
    {
        private readonly Mock<IDataStore> _mockDataStore;

        public DeletePersonHandlerTest()
        {
            _mockDataStore = new Mock<IDataStore>();
        }

        [Fact]
        public async Task DeletePerson_ShouldCallDataStoreDeleteMethodOnce()
        {
            var handler = new DeletePersonHandler(_mockDataStore.Object);
            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string>{{"name", "Name_to_delete"}}
            };
            await handler.DeletePerson(request);
            _mockDataStore.Verify(d => d.Delete("Name_to_delete"), Times.Once);
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnResponseStatusCode204_IfDeleteSuccessfully()
        {
            var handler = new DeletePersonHandler(_mockDataStore.Object);
            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string>{{"name", "Name_to_delete"}}
            };
            var response = await handler.DeletePerson(request);
            Assert.Equal(204, response.StatusCode);
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnResponseStatusCode403_IfDeleteFails()
        {
            _mockDataStore
                .Setup(d => d.Delete("Name_to_delete"))
                .Callback(() => throw new AmazonS3Exception("fail to delete"));
            var handler = new DeletePersonHandler(_mockDataStore.Object);
            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { {"name", "Name_to_delete"} }
            };

            var response = await handler.DeletePerson(request);
            Assert.Equal(403, response.StatusCode);
        }
    }
}