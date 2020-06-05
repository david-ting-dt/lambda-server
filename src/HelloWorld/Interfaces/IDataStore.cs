using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3.Model;

namespace HelloWorld.Interfaces
{
    public interface IDataStore
    {
        public Task<List<string>> Get();
        public Task<PutObjectResponse> Post(string requestBody);
        public Task Delete(string key);
    }
}