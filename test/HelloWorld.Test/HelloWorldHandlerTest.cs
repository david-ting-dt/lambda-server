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
  public class HelloWorldHandlerTest
  {
      private readonly Mock<IDbHandler> _mockDbHandler;
      private readonly Mock<ILogger> _mockLogger;

      private readonly APIGatewayProxyRequest _fakeRequest = new APIGatewayProxyRequest { HttpMethod = "Get" };
      private readonly List<Person> _people = new List<Person>
      {
          new Person { Id = "1", Name = "David" },
          new Person { Id = "2", Name = "Michael" },
          new Person { Id = "3", Name = "Will" }
      };

      public HelloWorldHandlerTest()
      {
          _mockDbHandler = new Mock<IDbHandler>();
          _mockLogger = new Mock<ILogger>();
      }

      [Fact]
      public async Task HelloWorld_ShouldCallDbHandlerGetPeopleAsyncOnce()
      {
          var handler = new HelloWorldHandler(_mockDbHandler.Object, _mockLogger.Object);
          await handler.HelloWorld(_fakeRequest);
          _mockDbHandler.Verify(db => db.GetPeopleAsync(), Times.Once);
      }

      [Fact]
      public async Task HelloWorld_ShouldCallLoggerLogMethodAtLeastOnce()
      {
          var handler = new HelloWorldHandler(_mockDbHandler.Object, _mockLogger.Object);
          await handler.HelloWorld(_fakeRequest);
          _mockLogger.Verify(logger => logger.Log(It.IsAny<string>()), Times.AtLeastOnce);
      }
      
      [Fact]
      public async Task HelloWorld_ShouldReturnStatusCode200_IfSuccessful()
      {
          _mockDbHandler
              .Setup(db => db.GetPeopleAsync())
              .ReturnsAsync(_people);
          var handler = new HelloWorldHandler(_mockDbHandler.Object, _mockLogger.Object);
          var response = await handler.HelloWorld(_fakeRequest);
          Assert.Equal(200, response.StatusCode);
      }
      
      [Fact]
      public async Task HelloWorld_ShouldReturnCorrectMessage_IfSuccessful()
      {
          _mockDbHandler
              .Setup(db => db.GetPeopleAsync())
              .ReturnsAsync(_people);
          var handler = new HelloWorldHandler(_mockDbHandler.Object, _mockLogger.Object);
          var response = await handler.HelloWorld(_fakeRequest);
          
          var time = DateTime.Now.ToShortTimeString();
          var date = DateTime.Now.ToLongDateString();
          var expected = $"Hello David, Michael, Will - the time on the server is {time} on {date}";
          Assert.Equal(expected, response.Body);
      }
      
      [Fact]
      public async Task HelloWorld_ShouldReturnResponseStatusCode500_IfExceptionIsThrown()
      {
          _mockDbHandler
              .Setup(db => db.GetPeopleAsync())
              .Throws(new Exception());
          var handler = new HelloWorldHandler(_mockDbHandler.Object, _mockLogger.Object);
          var response = await handler.HelloWorld(_fakeRequest);
          Assert.Equal(500, response.StatusCode);
      }

  }
}