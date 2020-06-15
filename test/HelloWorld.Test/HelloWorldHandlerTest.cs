using System.Collections.Generic;
using System.Threading.Tasks;
using HelloWorld.Interfaces;
using Moq;
using Xunit;


namespace HelloWorld.Tests
{
  public class HelloWorldHandlerTest
  {
      private readonly Mock<IDataStore> _mockDataStore;

      public HelloWorldHandlerTest()
      {
          _mockDataStore = new Mock<IDataStore>();
          _mockDataStore
              .Setup(d => d.Get())
              .ReturnsAsync(new List<string> {"David", "Michael", "Will"});
      }

      [Fact]
      public async Task HelloWorld_ShouldCallDataStoreGetMethodOnce()
      {
          var helloWorldHandler = new HelloWorldHandler(_mockDataStore.Object);
          await helloWorldHandler.HelloWorld();
          _mockDataStore.Verify(d => d.Get(), Times.Once);
      }

      [Fact]
      public async Task HelloWorld_ShouldReturnResponseStatusCode200()
      {
          var helloWorldHandler = new HelloWorldHandler(_mockDataStore.Object);
          var response = await helloWorldHandler.HelloWorld();
          Assert.Equal(200, response.StatusCode);
      }
  }
}