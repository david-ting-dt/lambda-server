using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;
using Newtonsoft.Json;
using ILogger = HelloWorld.Interfaces.ILogger;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HelloWorld
{

    public class HelloWorldHandler
    {
        private readonly IDbHandler _dbHandler;
        private readonly ILogger _logger;

        public HelloWorldHandler()
        {
            _dbHandler = new DynamoDbHandler(new AmazonDynamoDBClient());
            _logger = new LambdaFnLogger();
        }

        public HelloWorldHandler(IDbHandler dbHandler, ILogger logger)
        {
            _dbHandler = dbHandler;
            _logger = logger;
        }

        public async Task<APIGatewayProxyResponse> HelloWorld(APIGatewayProxyRequest request)
        { 
            _logger.Log($"API GATEWAY REQUEST: {JsonConvert.SerializeObject(request)}");
            try
            {
                return await CreateResponse();
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString());
                return DefaultServerResponse.CreateServerErrorResponse();
            }
        }

        private async Task<APIGatewayProxyResponse> CreateResponse()
        {
            var people = await _dbHandler.GetPeopleAsync();
            var names = people.Select(p => p.Name);
            var message = GetHelloMessage(string.Join(", ", names));
            var response = new APIGatewayProxyResponse
            {
                Body = message,
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
            _logger.Log($"API GATEWAY RESPONSE: {JsonConvert.SerializeObject(response)}");
            return response;
        }

        private static string GetHelloMessage(string name)
        {
            var time = DateTime.Now.ToShortTimeString();
            var date = DateTime.Now.ToLongDateString();
            return $"Hello {name} - the time on the server is {time} on {date}";
        }
    }
}
