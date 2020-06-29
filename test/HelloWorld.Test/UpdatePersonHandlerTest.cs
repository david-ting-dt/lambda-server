using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using Amazon.S3.Model;
using HelloWorld.Interfaces;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace HelloWorld.Tests
{
    public class UpdatePersonHandlerTest
    {
        private readonly Mock<IDataStore> _mockDataStore;

        public UpdatePersonHandlerTest()
        {
            _mockDataStore = new Mock<IDataStore>();
        }

        [Fact]
        public async Task UpdatePerson_ShouldCallDataStorePutMethodOnce()
        {
            var handler = new UpdatePersonHandler(_mockDataStore.Object);
            MockDataStorePutMethod();
            var request = CreateMockRequest();
            await handler.UpdatePerson(request);
            _mockDataStore.Verify(d => d.Put("Old_Name", "New_Name", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePerson_ShouldReturnExpectedResponse_IfUpdateSuccessfully()
        {
            var handler = new UpdatePersonHandler(_mockDataStore.Object);
            MockDataStorePutMethod();
            var request = CreateMockRequest();
            var response = await handler.UpdatePerson(request);
            var expected = new APIGatewayProxyResponse
            {
                StatusCode = 301,
                Headers = new Dictionary<string, string> { {"Location", "/people/New_Name"}, {"ETag", "fake_etag"} }
            };

            var responseJson = JsonConvert.SerializeObject(response);
            var expectedJson = JsonConvert.SerializeObject(expected);
            Assert.Equal(expectedJson, responseJson);
        }

        private void MockDataStorePutMethod()
        {
            _mockDataStore.Setup(s3 => s3.Put(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new PutObjectResponse {ETag = "fake_etag"});
        }

        [Fact]
        public async Task UpdatePerson_ShouldReturnResponseStatusCode400_IfRequestBodyLengthGreaterThan30()
        {
            var handler = new UpdatePersonHandler(_mockDataStore.Object);
            var request = new APIGatewayProxyRequest { Body = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"};
            var response = await handler.UpdatePerson(request);
            Assert.Equal(400, response.StatusCode);
        }
        
        [Fact]
        public async Task UpdatePerson_ShouldReturnResponseStatusCode404_IfUpdateFails()
        {
            var handler = new UpdatePersonHandler(_mockDataStore.Object);
            var request = CreateMockRequest();
            MockFailedDataStoreUpdate();
            var response = await handler.UpdatePerson(request);
            Assert.Equal(404, response.StatusCode);
        }

        private void MockFailedDataStoreUpdate()
        {
            _mockDataStore
                .Setup(d => d.Put("Old_Name", "New_Name", It.IsAny<string>()))
                .Callback(() => throw new AmazonS3Exception("Cannot find 'Old_Name'"));
        }
        
        private static APIGatewayProxyRequest CreateMockRequest()
        {
            return new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { {"name", "Old_Name"} },
                Body = "New_Name",
                Path = "/people/Old_Name",
                Headers = null
            };
        }
    }
}