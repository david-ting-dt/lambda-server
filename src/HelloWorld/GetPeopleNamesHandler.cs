using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using HelloWorld.Interfaces;
using Newtonsoft.Json;

namespace HelloWorld
{
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
            _logger.Log($"API GATEWAY REQUEST: {JsonConvert.SerializeObject(request)}");
            try
            {
                return await CreateResponse();
            }
            catch (Exception e)
            {
                var response = DefaultServerResponse.CreateServerErrorResponse();
                _logger.Log($"API GATEWAY RESPONSE: {JsonConvert.SerializeObject(response)}");
                _logger.Log(e.ToString()); 
                return response;
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
            
            _logger.Log($"API GATEWAY RESPONSE: {JsonConvert.SerializeObject(response)}");
            return response;
        }
    }
}