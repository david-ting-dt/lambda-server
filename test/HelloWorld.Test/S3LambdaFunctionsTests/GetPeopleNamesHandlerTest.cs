using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using HelloWorld.S3LambdaFunctions;
using Xunit;
using Moq;

namespace HelloWorld.Tests.S3LambdaFunctionsTests
{
    public class GetPeopleNamesHandlerTest
    {
        [Fact]
        public async Task GetNames_ShouldReturnCorrectStringNames()
        {
            var mockS3Client = new Mock<IAmazonS3>();
            SetupMock(mockS3Client);
            var handler = new GetPeopleNamesHandler(mockS3Client.Object);

            var result = await handler.GetNames();
            const string expected = "David, Michael, Will";
            Assert.Equal(expected, result);
        }

        private static void SetupMock(Mock<IAmazonS3> mock)
        {
            mock
                .Setup(s3 => s3.ListObjectsAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ListObjectsResponse{S3Objects = new List<S3Object>
                {
                    new S3Object{Key = "david.txt"}, new S3Object{Key = "michael.txt"}, new S3Object{Key = "will.txt"}
                }});
            mock
                .SetupSequence(s3 => s3.GetObjectAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new GetObjectResponse {ResponseStream = new MemoryStream(Encoding.UTF8.GetBytes("David"))})
                .ReturnsAsync(
                    new GetObjectResponse {ResponseStream = new MemoryStream(Encoding.UTF8.GetBytes("Michael"))})
                .ReturnsAsync(
                    new GetObjectResponse {ResponseStream = new MemoryStream(Encoding.UTF8.GetBytes("Will"))});
        }
    }
}