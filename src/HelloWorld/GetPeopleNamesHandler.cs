using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    public class GetPeopleNamesHandler
    {
        private readonly IDbHandler _dbHandler;

        public GetPeopleNamesHandler()
        {
            _dbHandler = new DynamoDbHandler(new AmazonDynamoDBClient());
        }

        public GetPeopleNamesHandler(IDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public async Task<APIGatewayProxyResponse> GetPeopleNames()
        {
            try
            {
                return await CreateResponse();
            }
            catch (Exception e)
            {
                LambdaLogger.Log(e.ToString());
                return DefaultServerResponse.CreateServerErrorResponse();
            }
        }

        private async Task<APIGatewayProxyResponse> CreateResponse()
        {
            var people = await _dbHandler.GetPeopleAsync();
            var names = people.Select(p => p.Name);
            var body = string.Join(", ", names);

            return new APIGatewayProxyResponse
            {
                Body = body,
                StatusCode = 200,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
            };
        }
    }
}