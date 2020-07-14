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
        private readonly ILogger _logger;

        public DeletePersonHandler()
        {
            var dbContext = new DynamoDBContext(new AmazonDynamoDBClient());
            _dbHandler = new DynamoDbHandler(dbContext);
            _logger = new LambdaFnLogger();
        }

        public DeletePersonHandler(IDbHandler dbHandler, ILogger logger)
        {
            _dbHandler = dbHandler;
            _logger = logger;
        }

        public async Task<APIGatewayProxyResponse> DeletePerson(APIGatewayProxyRequest request)
        {
            _logger.Log($"API Gateway request received - HttpMethod: {request.HttpMethod}  Path: {request.Path}");
            try
            {
                return await CreateDeleteResponse(request);
            }
            catch (Exception e)
            {
                var response = DefaultServerResponse.CreateServerErrorResponse();
                _logger.Log($"API Gateway response produced - StatusCode: {response.StatusCode}  Body: {response.Body}");
                _logger.Log(e.ToString()); 
                return response;
            }
        }

        private async Task<APIGatewayProxyResponse> CreateDeleteResponse(APIGatewayProxyRequest request)
        {
            var id = request.PathParameters["id"];
            APIGatewayProxyResponse response;
            try
            {
                await _dbHandler.DeletePersonAsync(id);
                response = new APIGatewayProxyResponse {StatusCode = 204};
            }
            catch (NullReferenceException)
            {
                response = new APIGatewayProxyResponse {StatusCode = 404, Body = "Resource not found"};
            }
            _logger.Log($"API Gateway response produced - StatusCode: {response.StatusCode}  Body: {response.Body}");
            return response;
        }
    }
}