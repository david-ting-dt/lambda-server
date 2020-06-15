using System.Collections.Generic;
using System.Threading.Tasks;
using HelloWorld.Interfaces;
using Moq;
using Xunit;

namespace HelloWorld.Tests
{
    public class GetPeopleNamesHandlerTest
    {
        private readonly Mock<IDataStore> _mockDataStore;
        
        public GetPeopleNamesHandlerTest()
        {
            _mockDataStore = new Mock<IDataStore>();
            _mockDataStore
                .Setup(d => d.Get())
                .ReturnsAsync(new List<string> {"David", "Michael", "Will"});
        }

        [Fact]
        public async Task GetPeopleNames_ShouldCallDataStoreGetMethodOnce()
        {
            var handler = new GetPeopleNamesHandler(_mockDataStore.Object);
            await handler.GetPeopleNames();
            _mockDataStore.Verify(d => d.Get(), Times.Once);
        }

        [Fact]
        public async Task GetPeopleNames_ShouldReturnResponseStatusCode200()
        {
            var handler = new GetPeopleNamesHandler(_mockDataStore.Object);
            var response = await handler.GetPeopleNames();
            Assert.Equal(200, response.StatusCode);
        }
    }
}