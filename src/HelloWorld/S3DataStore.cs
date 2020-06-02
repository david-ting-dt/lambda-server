using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    public class S3DataStore : IDataStore
    {
        private readonly string _bucketName = "david-ting-hello-world";
        private readonly AmazonS3Client _s3Client = new AmazonS3Client();
        
        public async Task<Stream> Get()
        {
            var keys = (await _s3Client.ListObjectsAsync(_bucketName)).S3Objects.Select(o => o.Key);
            var names = await Task.WhenAll(keys.Select(GetNamesFromS3));
            return new MemoryStream(Encoding.UTF8.GetBytes(string.Join(", ", names)));
        }

        private async Task<string> GetNamesFromS3(string key)
        {
            var objResponse = await _s3Client.GetObjectAsync(_bucketName, key);
            using var sr = new StreamReader(objResponse.ResponseStream);
            return await sr.ReadToEndAsync();
        }
    }
}