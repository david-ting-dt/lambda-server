using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
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
            try
            {
                await _dataStore.Put(oldKey, newKey);
            }
            catch (AmazonS3Exception e)
            {
                return CreateFailUpdateResponse(oldKey);
            }
            return CreateSuccessUpdateResponse(newKey, request.Path);
        }

        private static APIGatewayProxyResponse CreateFailUpdateResponse(string oldKey)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 404,
                Body = $"Cannot update {oldKey} - Resource not found"
            };
        }

        private static APIGatewayProxyResponse CreateSuccessUpdateResponse(string newKey, string path)
        {
            var location = GetNewLocation(newKey, path);
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