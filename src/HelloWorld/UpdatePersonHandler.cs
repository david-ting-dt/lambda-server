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
        private readonly IDataStore _dataStore = new S3DataStore();

        public async Task<APIGatewayProxyResponse> UpdatePerson(APIGatewayProxyRequest request)
        {
            var oldKey = request.PathParameters["person"];
            var newKey = request.Body;
            await _dataStore.Put(oldKey, newKey);

            var location = GetNewLocation(newKey, request.Path);
            return new APIGatewayProxyResponse
            {
                StatusCode = 301,
                Headers = new Dictionary<string, string> {{"Location", location}}
            };
        }

        private string GetNewLocation(string newKey, string path)
        {
            var uri = new Uri(path);
            var pathWithoutLastSegment = uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length);
            return $"{pathWithoutLastSegment}{newKey}";
        }
    }
}