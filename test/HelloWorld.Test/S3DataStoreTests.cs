using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using Xunit;

namespace HelloWorld.Tests
{
    public class S3DataStoreTests
    {
        private readonly Mock<IAmazonS3> _mockS3Client;
        
        public S3DataStoreTests()
        {
            _mockS3Client = new Mock<IAmazonS3>();
        }
        
        [Fact]
        public async Task Get_ShouldReturnListOfNames()
        {
            _mockS3Client
                .Setup(s3 => s3.ListObjectsAsync(It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateMockListObjectResponse());
            
            var s3DataStore = new S3DataStore(_mockS3Client.Object);

            var result = await s3DataStore.Get();
            var expected = new List<string>{"David", "Michael", "Will"};
            Assert.Equal(expected, result);
        }

        private static ListObjectsResponse CreateMockListObjectResponse()
        {
            return new ListObjectsResponse
            {
                S3Objects = new List<S3Object>
                {
                    new S3Object{Key = "David"}, new S3Object{Key = "Michael"}, new S3Object{Key = "Will"}
                }
            };
        }
        
        [Fact]
        public async Task Post_ShouldCallPutObjectAsyncOnce()
        {
            var s3DataStore = new S3DataStore(_mockS3Client.Object);
            await s3DataStore.Post("Key_To_Add");
            _mockS3Client.Verify(s3 => 
                s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldCallDeleteObjectAsyncOnce()
        {
            var s3DataStore = new S3DataStore(_mockS3Client.Object);
            await s3DataStore.Delete("Key_To_Delete");
            _mockS3Client.Verify(s3 => 
                s3.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Put_ShouldCallCopyObjectAsyncAndDeleteObjectAsyncOnce()
        {
            var s3DataStore = new S3DataStore(_mockS3Client.Object);
            await s3DataStore.Put("Old_Key", "New_Key");
            _mockS3Client.Verify(s3 => 
                s3.CopyObjectAsync(It.IsAny<CopyObjectRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _mockS3Client.Verify(s3 => 
                    s3.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), 
                        It.IsAny<CancellationToken>()),
                Times.Once);
            _mockS3Client.Verify(s3 => 
                s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}