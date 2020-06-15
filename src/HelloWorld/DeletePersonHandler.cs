using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    public class DeletePersonHandler
    {
        private readonly IDataStore _dataStore;

        public DeletePersonHandler()
        {
            _dataStore = new S3DataStore();
        }

        public DeletePersonHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<APIGatewayProxyResponse> DeletePerson(APIGatewayProxyRequest request)
        {
            try
            {
                var key = request.PathParameters["name"];
                await _dataStore.Delete(key);
            }
            catch (AmazonS3Exception e)
            {
                return new APIGatewayProxyResponse{StatusCode = 403};
            }
            return new APIGatewayProxyResponse
            {
                StatusCode = 204
            };
        }
    }
}