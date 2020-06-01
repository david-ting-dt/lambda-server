using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.S3;

// [assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HelloWorld.S3LambdaFunctions
{
    public class GetPeopleNamesHandler
    {
        private readonly IAmazonS3 _s3Client;

        public GetPeopleNamesHandler(IAmazonS3 s3Client = null)
        {
            _s3Client = s3Client ?? new AmazonS3Client();
        }

        public async Task<string> GetNames()
        {
            const string bucketName = "david-ting-hello-world";
            var keys = (await _s3Client.ListObjectsAsync(bucketName)).S3Objects.Select(o => o.Key);
            var names = await GetNamesFromS3(bucketName, keys);
            return string.Join(", ", names);
        }

        private async Task<List<string>> GetNamesFromS3(string bucketName, IEnumerable<string> keys)
        {
            var names = new List<string>();
            foreach (var key in keys)
            {
                var obj = await _s3Client.GetObjectAsync(bucketName, key);
                using var sr = new StreamReader(obj.ResponseStream);
                names.Add(await sr.ReadToEndAsync());
            }
            return names;
        }
    }
}