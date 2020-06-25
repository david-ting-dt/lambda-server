using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
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
                await ExecuteDeleteCommand(request);
                return new APIGatewayProxyResponse { StatusCode = 204 };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return DefaultServerResponse.CreateServerErrorResponse();
            }
        }

        private async Task ExecuteDeleteCommand(APIGatewayProxyRequest request)
        {
            var key = request.PathParameters["name"];
            await _dataStore.Delete(key);
        }
    }
}