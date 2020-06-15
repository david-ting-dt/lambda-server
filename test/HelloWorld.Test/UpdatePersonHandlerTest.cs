using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
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
            var request = CreateMockRequest();
            await handler.UpdatePerson(request);
            _mockDataStore.Verify(d => d.Put("Old_Name", "New_Name"), Times.Once);
        }

        [Fact]
        public async Task UpdatePerson_ShouldReturnExpectedResponse()
        {
            var handler = new UpdatePersonHandler(_mockDataStore.Object);
            var request = CreateMockRequest();
            var response = await handler.UpdatePerson(request);
            var expected = new APIGatewayProxyResponse
            {
                StatusCode = 301,
                Headers = new Dictionary<string, string> { {"Location", "/people/New_Name"} }
            };

            var responseJson = JsonConvert.SerializeObject(response);
            var expectedJson = JsonConvert.SerializeObject(expected);
            Assert.Equal(expectedJson, responseJson);
        }

        private static APIGatewayProxyRequest CreateMockRequest()
        {
            return new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { {"name", "Old_Name"} },
                Body = "New_Name",
                Path = "/people/"
            };
        }
    }
}