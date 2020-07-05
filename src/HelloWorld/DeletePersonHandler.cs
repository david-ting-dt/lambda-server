using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    public class DeletePersonHandler
    {
        private readonly IDbHandler _dbHandler;

        public DeletePersonHandler()
        {
            var dbContext = new DynamoDBContext(new AmazonDynamoDBClient());
            _dbHandler = new DynamoDbHandler(dbContext);
        }

        public DeletePersonHandler(IDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public async Task<APIGatewayProxyResponse> DeletePerson(APIGatewayProxyRequest request)
        {
            try
            {
                await ExecuteDeleteCommand(request);
                return new APIGatewayProxyResponse {StatusCode = 204};
            }
            catch (NullReferenceException)
            {
                return new APIGatewayProxyResponse {StatusCode = 404, Body = "Resource not found"};
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return DefaultServerResponse.CreateServerErrorResponse(e);
            }
        }

        private async Task ExecuteDeleteCommand(APIGatewayProxyRequest request)
        {
            var id = request.PathParameters["id"];
            await _dbHandler.DeletePersonAsync(id);
        }
    }
}