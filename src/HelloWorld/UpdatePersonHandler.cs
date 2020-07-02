using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdatePersonHandler
    {
        private readonly IDbHandler _dbHandler;

        public UpdatePersonHandler()
        {
            var dbContext = new DynamoDBContext(new AmazonDynamoDBClient());
            _dbHandler = new DynamoDbHandler(dbContext);
        }

        public UpdatePersonHandler(IDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public async Task<APIGatewayProxyResponse> UpdatePerson(APIGatewayProxyRequest request)
        {
            try
            {
                return await CreateResponse(request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return DefaultServerResponse.CreateServerErrorResponse(e);
            }
        }

        private async Task<APIGatewayProxyResponse> CreateResponse(APIGatewayProxyRequest request)
        {
            var id = request.PathParameters["id"];
            var newName = request.Body;
            var isRequestValid = Validator.ValidateRequest(newName);
            if (!isRequestValid)
                return new APIGatewayProxyResponse { StatusCode = 400, Body = "Invalid request - name must be between 0 and 30 characters"};

            await _dbHandler.UpdatePersonAsync(id, newName);
            return new APIGatewayProxyResponse { StatusCode = 301};
        }
    }
}