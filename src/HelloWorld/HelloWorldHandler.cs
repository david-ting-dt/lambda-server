using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HelloWorld
{

    public class HelloWorldHandler
    {
        private readonly IDbHandler _dbHandler;

        public HelloWorldHandler()
        {
            _dbHandler = new DynamoDbHandler(new AmazonDynamoDBClient());
        }

        public HelloWorldHandler(IDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public async Task<APIGatewayProxyResponse> HelloWorld()
        {
            try
            {
                return await CreateResponse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return DefaultServerResponse.CreateServerErrorResponse(e);
            }
        }

        private async Task<APIGatewayProxyResponse> CreateResponse()
        {
            var people = await _dbHandler.GetPeopleAsync();
            var names = people.Select(p => p.Name);
            var message = GetHelloMessage(string.Join(", ", names));

            return new APIGatewayProxyResponse
            {
                Body = message,
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        private static string GetHelloMessage(string name)
        {
            var time = DateTime.Now.ToShortTimeString();
            var date = DateTime.Now.ToLongDateString();
            return $"Hello {name} - the time on the server is {time} on {date}";
        }
    }
}
