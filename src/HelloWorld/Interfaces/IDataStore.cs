using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3.Model;

namespace HelloWorld.Interfaces
{
    public interface IDataStore
    {
        public Task<List<string>> Get();
        public Task<PutObjectResponse> Post(string key);
        public Task Delete(string key);
        public Task<PutObjectResponse> Put(string oldKey, string newKey);
    }
}