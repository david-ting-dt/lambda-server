using System.Collections.Generic;
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
        private readonly IAmazonS3 _s3Client;

        public S3DataStore()
        {
            _s3Client = new AmazonS3Client();
        }

        public S3DataStore(IAmazonS3 s3)
        {
            _s3Client = s3;
        }
        
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
                ContentBody = key
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

        public async Task<PutObjectResponse> Put(string oldKey, string newKey)
        {
            var copyObjectRequest = new CopyObjectRequest
            {
                SourceBucket = BucketName,
                DestinationBucket = BucketName,
                SourceKey = oldKey,
                DestinationKey = newKey
            };
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = newKey,
                ContentBody = newKey
            };

            await _s3Client.CopyObjectAsync(copyObjectRequest);
            await _s3Client.DeleteObjectAsync(BucketName, oldKey);
            return await _s3Client.PutObjectAsync(putObjectRequest);
        }
    }
}