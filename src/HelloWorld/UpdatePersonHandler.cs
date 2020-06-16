using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    public class UpdatePersonHandler
    {
        private readonly IDataStore _dataStore;

        public UpdatePersonHandler()
        {
            _dataStore = new S3DataStore();
        }

        public UpdatePersonHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<APIGatewayProxyResponse> UpdatePerson(APIGatewayProxyRequest request)
        {
            var oldKey = request.PathParameters["name"];
            var newKey = request.Body;
            await _dataStore.Put(oldKey, newKey);

            var location = GetNewLocation(newKey, request.Path);
            return new APIGatewayProxyResponse
            {
                StatusCode = 301,
                Headers = new Dictionary<string, string> {{"Location", location}}
            };
        }

        private static string GetNewLocation(string newKey, string path)
        {
            var lastSlashIndex = path.LastIndexOf('/');
            var resourcePath = (lastSlashIndex > -1) ? path.Substring(0, lastSlashIndex) : path;
            return $"{resourcePath}/{newKey}";
        }
    }
}