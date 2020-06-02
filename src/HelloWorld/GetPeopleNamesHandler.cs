using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;
using Newtonsoft.Json;

namespace HelloWorld
{
    public class GetPeopleNamesHandler
    {
        private readonly IDataStore _dataStore = new S3DataStore();

        public async Task<APIGatewayProxyResponse> GetPeopleNames()
        {
            var dataStream = await _dataStore.Get();
            using var sr = new StreamReader(dataStream);
            var names = await sr.ReadToEndAsync();
            var body = new Dictionary<string, string>
            {
                {"names", names}
            };

            return new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(body),
                StatusCode = 200,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
            };
        }
    }
}