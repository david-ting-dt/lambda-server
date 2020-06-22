using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3.Model;
using HelloWorld.Interfaces;
using Moq;
using Xunit;

namespace HelloWorld.Tests
{
    public class AddPersonHandlerTest
    {
        private readonly Mock<IDataStore> _mockDataStore;

        public AddPersonHandlerTest()
        {
            _mockDataStore = new Mock<IDataStore>();
            _mockDataStore
                .Setup(d => d.Get())
                .ReturnsAsync(new List<string> {"David", "Michael", "Will"});
        }

        [Fact]
        public async Task AddPerson_ShouldCallDataStorePostMethodOnce_IfPersonNotAlreadyExist()
        {
            MockDataStorePutMethod();
            var handler = new AddPersonHandler(_mockDataStore.Object);
            var request = new APIGatewayProxyRequest { Body = "Name_to_add" };
            await handler.AddPerson(request);
            _mockDataStore.Verify(d => d.Post(request.Body), Times.Once);
        }

        [Fact]
        public async Task AddPerson_ShouldReturnResponseStatusCode200_IfPersonNotAlreadyExit()
        {
            MockDataStorePutMethod();
            var handler = new AddPersonHandler(_mockDataStore.Object);
            var request = new APIGatewayProxyRequest { Body = "Name_to_add" };
            var response = await handler.AddPerson(request);
            Assert.Equal(200, response.StatusCode);
        }
        
        private void MockDataStorePutMethod()
        {
            _mockDataStore.Setup(s3 => s3.Post(It.IsAny<string>()))
                .ReturnsAsync(new PutObjectResponse {ETag = "fake_etag"});
        }

        [Fact]
        public async Task AddPerson_ShouldNotCallDataStorePostMethod_IfPersonAlreadyExist()
        {
            var handler = new AddPersonHandler(_mockDataStore.Object);
            var request = new APIGatewayProxyRequest { Body = "David" };
            await handler.AddPerson(request);
            _mockDataStore.Verify(d => d.Post(request.Body), Times.Never);
        }

        [Fact]
        public async Task AddPerson_ShouldReturnResponseStatusCode202_IfPersonAlreadyExists()
        {
            var handler = new AddPersonHandler(_mockDataStore.Object);
            var request = new APIGatewayProxyRequest{ Body = "David" };
            var response = await handler.AddPerson(request);
            Assert.Equal(202, response.StatusCode);
        }
    }
}