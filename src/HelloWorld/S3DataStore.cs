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
            var keys = (await _s3Client.ListObjectsAsync(BucketName))
                .S3Objects.Select(o => o.Key)
                .ToList();
            return keys;
        }

        public async Task<PutObjectResponse> Post(string key)
        {
            var request = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = key,
            };
            return await _s3Client.PutObjectAsync(request);
        }

        public async Task Delete(string key)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = BucketName,
                Key = key,
            };
            await _s3Client.DeleteObjectAsync(request);
        }

        public async Task Put(string oldKey, string newKey)
        {
            var request = new CopyObjectRequest
            {
                SourceBucket = BucketName,
                DestinationBucket = BucketName,
                SourceKey = oldKey,
                DestinationKey = newKey
            };

            await _s3Client.CopyObjectAsync(request);
            await _s3Client.DeleteObjectAsync(BucketName, oldKey);
        }
    }
}