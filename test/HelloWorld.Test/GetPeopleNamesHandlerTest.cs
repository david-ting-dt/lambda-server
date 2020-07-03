using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelloWorld.DbItem;
using HelloWorld.Interfaces;
using Moq;
using Xunit;

namespace HelloWorld.Tests
{
    public class GetPeopleNamesHandlerTest
    {
        private readonly Mock<IDbHandler> _mockDbHandler;
        private readonly List<Person> _people = new List<Person>
        {
            new Person { Id = "1", Name = "David" },
            new Person { Id = "2", Name = "Michael" },
            new Person { Id = "3", Name = "Will" }
        };
        
        public GetPeopleNamesHandlerTest()
        {
            _mockDbHandler = new Mock<IDbHandler>();
        }

        [Fact]
        public async Task GetPeopleNames_ShouldCallDbHandlerGetPeopleAsyncOnce()
        {
            var handler = new GetPeopleNamesHandler(_mockDbHandler.Object);
            await handler.GetPeopleNames();
            _mockDbHandler.Verify(db => db.GetPeopleAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPeopleNames_ShouldReturnResponseStatusCode200_IfSuccessful()
        {
            _mockDbHandler
                .Setup(db => db.GetPeopleAsync())
                .ReturnsAsync(_people);
            var handler = new GetPeopleNamesHandler(_mockDbHandler.Object);
            var response = await handler.GetPeopleNames();
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async Task GetPeopleNames_ShouldReturnCorrectResponseBody_IfSuccessful()
        {
            _mockDbHandler
                .Setup(db => db.GetPeopleAsync())
                .ReturnsAsync(_people);
            var handler = new GetPeopleNamesHandler(_mockDbHandler.Object);
            var response = await handler.GetPeopleNames();
            const string expected = "David, Michael, Will";
            Assert.Equal(expected, response.Body);
        }

        [Fact]
        public async Task GetPeopleNames_ShouldReturnResponseStatusCode500_IfExceptionIsThrown()
        {
            _mockDbHandler
                .Setup(db => db.GetPeopleAsync())
                .Throws(new Exception());
            var handler = new GetPeopleNamesHandler(_mockDbHandler.Object);
            var response = await handler.GetPeopleNames();
            Assert.Equal(500, response.StatusCode);
        }
    }
}