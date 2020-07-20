using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    /// <summary>
    /// Function code for AWS Lambda Function - GetPeopleDNFunction
    /// </summary>
    public class GetPeopleNamesHandler
    {
        private readonly IDbHandler _dbHandler;
        private readonly ILogger _logger;

        public GetPeopleNamesHandler()
        {
            _dbHandler = new DynamoDbHandler(new AmazonDynamoDBClient());
            _logger = new LambdaFnLogger();
        }

        public GetPeopleNamesHandler(IDbHandler dbHandler, ILogger logger)
        {
            _dbHandler = dbHandler;
            _logger = logger;
        }

        public async Task<APIGatewayProxyResponse> GetPeopleNames(APIGatewayProxyRequest request)
        {
            _logger.Log($"API Gateway request received - HttpMethod: {request.HttpMethod}  Path: {request.Path}");
            try
            {
                return await CreateResponse();
            }
            catch (Exception e)
            {
                var response = DefaultServerResponse.CreateServerErrorResponse();
                _logger.Log($"API Gateway response produced - StatusCode: {response.StatusCode}  Body: {response.Body}");
                _logger.Log(e.ToString());
                throw;
            }
        }

        private async Task<APIGatewayProxyResponse> CreateResponse()
        {
            var people = await _dbHandler.GetPeopleAsync();
            var names = people.Select(p => p.Name);
            var body = string.Join(", ", names);
            var response = new APIGatewayProxyResponse
            {
                Body = body,
                StatusCode = 200,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
            };
            
            _logger.Log($"API Gateway response produced - StatusCode: {response.StatusCode}  Body: {response.Body}");
            return response;
        }
    }
}