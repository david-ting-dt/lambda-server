using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    /// <summary>
    /// 
    /// </summary>
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
            var oldName = request.PathParameters["name"];
            var newName = request.Body;
            var requestETag = request.Headers != null && request.Headers.ContainsKey("If-Match") ? 
                request.Headers["If-Match"] : "";
            try
            {
                var response = await _dataStore.Put(oldName, newName, requestETag);
                return CreateSuccessUpdateResponse(newName, request.Path, response.ETag);
            }
            catch (AmazonS3Exception)
            {
                return CreateFailUpdateResponse(oldName);
            }
        }

        private static APIGatewayProxyResponse CreateFailUpdateResponse(string oldName)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 404,
                Body = $"Cannot update {oldName} - Resource not found"
            };
        }

        private static APIGatewayProxyResponse CreateSuccessUpdateResponse(string newName, string path, string etag)
        {
            var location = GetNewLocation(newName, path);
            return new APIGatewayProxyResponse
            {
                StatusCode = 301,
                Headers = new Dictionary<string, string>
                {
                    {"Location", location},
                    {"ETag", etag}
                }
            };
        }

        private static string GetNewLocation(string newName, string path)
        {
            var lastSlashIndex = path.LastIndexOf('/');
            var resourcePath = (lastSlashIndex > -1) ? path.Substring(0, lastSlashIndex) : path;
            return $"{resourcePath}/{newName}";
        }
    }
}