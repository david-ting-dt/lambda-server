using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using HelloWorld.DbItem;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace HelloWorld.Tests
{
    public class DynamoDbHandlerTests
    {
        private readonly Mock<IAmazonDynamoDB> _mockClient;
        private readonly Mock<IDynamoDBContext> _mockContext;
        private readonly ScanResponse _mockScanResponse = new ScanResponse
        {
            Items = new List<Dictionary<string, AttributeValue>>
            {
                new Dictionary<string, AttributeValue>
                {
                    {"id", new AttributeValue("1")}, {"name", new AttributeValue("David")}
                },
                new Dictionary<string, AttributeValue>
                {
                    {"id", new AttributeValue("2")}, {"name", new AttributeValue("Michael")}
                }
            }
        };

        public DynamoDbHandlerTests()
        {
            _mockClient = new Mock<IAmazonDynamoDB>();
            _mockContext = new Mock<IDynamoDBContext>();
            _mockClient
                .Setup(client =>
                    client.ScanAsync(It.IsAny<ScanRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockScanResponse);
        }
        
        [Fact]
        public async Task GetPeopleAsync_ShouldCallDynamoDBClientScanAsyncOnce()
        {
            var dbHandler = new DynamoDbHandler(_mockClient.Object);
            await dbHandler.GetPeopleAsync();
            _mockClient
                .Verify(client => 
                    client.ScanAsync(It.IsAny<ScanRequest>(), It.IsAny<CancellationToken>())
                    , Times.Once);
        }

        [Fact]
        public async Task GetPeopleAsync_ShouldReturnAListOfPersonObjectsAsExpected()
        {
            var dbHandler = new DynamoDbHandler(_mockClient.Object);
            var result = await dbHandler.GetPeopleAsync();
            var expected = new List<Person>
            {
                new Person{Id = "1", Name = "David"}, new Person{Id = "2", Name = "Michael"}
            };
            var resultJson = JsonConvert.SerializeObject(result);
            var expectedJson = JsonConvert.SerializeObject(expected);
            
            Assert.Equal(expectedJson, resultJson);
        }

        [Fact]
        public async Task AddPersonAsync_ShouldCallDynamoDBContextSaveAsyncOnce()
        {
            var dbHandler = new DynamoDbHandler(_mockContext.Object);
            await dbHandler.AddPersonAsync("New_Person_Name");
            _mockContext.Verify(context => 
                context.SaveAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeletePersonAsync_ShouldCallDynamoDBContextLoadAsyncOnce()
        {
            var dbHandler = new DynamoDbHandler(_mockContext.Object);
            await dbHandler.DeletePersonAsync("id_to_delete");
            _mockContext.Verify(context =>
                context.LoadAsync<Person>("id_to_delete", It.IsAny<CancellationToken>()), 
                Times.Once);
        }
        
        [Fact]
        public async Task DeletePersonAsync_ShouldCallDynamoDBContextDeleteAsyncOnce()
        {
            var dbHandler = new DynamoDbHandler(_mockContext.Object);
            await dbHandler.DeletePersonAsync("id_to_delete");
            _mockContext.Verify(context =>
                context.DeleteAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdatePersonAsync_ShouldCallDynamoDBContextLoadAsyncOnce()
        {
            MockLoadingPersonFromDb();
            var dbHandler = new DynamoDbHandler(_mockContext.Object);
            await dbHandler.UpdatePersonAsync("id_to_update", "New_Name");
            _mockContext.Verify(context =>
                context.LoadAsync<Person>("id_to_update", It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdatePersonAsync_ShouldCallDynamoDBContextSaveAsyncOnce()
        {
            MockLoadingPersonFromDb();
            var dbHandler = new DynamoDbHandler(_mockContext.Object);
            await dbHandler.UpdatePersonAsync("id_to_update", "New_Name");
            _mockContext.Verify(context => 
                context.SaveAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private void MockLoadingPersonFromDb()
        {
            _mockContext.Setup(context => 
                    context.LoadAsync<Person>("id_to_update", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Person {Id = "id_to_update", Name = "Old_Name"});
        }
    }
}