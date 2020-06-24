using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.S3.Model;

namespace HelloWorld.Interfaces
{
    public interface IDataStore
    {
        public Task<List<string>> Get();
        public Task<PutObjectResponse> Post(string key);
        public Task<DeleteObjectResponse> Delete(string key, string requestETag = "");
        public Task<PutObjectResponse> Put(string oldKey, string newKey, string requestETag = "");
    }
}