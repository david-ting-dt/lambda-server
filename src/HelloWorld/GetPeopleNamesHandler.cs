using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;
using Newtonsoft.Json;

namespace HelloWorld
{
    public class GetPeopleNamesHandler
    {
        private readonly IDataStore _dataStore;

        public GetPeopleNamesHandler()
        {
            _dataStore = new S3DataStore();
        }

        public GetPeopleNamesHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<APIGatewayProxyResponse> GetPeopleNames()
        {
            var names = await _dataStore.Get();
            var body = string.Join(", ", names);

            return new APIGatewayProxyResponse
            {
                Body = body,
                StatusCode = 200,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
            };
        }
    }
}