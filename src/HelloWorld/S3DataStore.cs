using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    public class S3DataStore : IDataStore
    {
        private const string BucketName = "david-ting-hello-world";
        private readonly AmazonS3Client _s3Client = new AmazonS3Client();
        
        public async Task<List<string>> Get()
        {
            var keys = (await _s3Client.ListObjectsAsync(BucketName)).S3Objects.Select(o => o.Key);
            var names = await Task.WhenAll(keys.Select(GetNamesFromS3));
            return names.ToList();
        }

        private async Task<string> GetNamesFromS3(string key)
        {
            var objResponse = await _s3Client.GetObjectAsync(BucketName, key);
            using var sr = new StreamReader(objResponse.ResponseStream);
            return await sr.ReadToEndAsync();
        }
        
        public async Task<PutObjectResponse> Post(string requestBody)
        {
            var request = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = $"{requestBody}.txt",
                ContentBody = requestBody
            };
            return await _s3Client.PutObjectAsync(request);
        }

        public async Task Delete(string key)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = BucketName,
                Key = $"{key}.txt",
            };
            await _s3Client.DeleteObjectAsync(request);
        }
    }
}