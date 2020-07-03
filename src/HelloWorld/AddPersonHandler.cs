using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    public class AddPersonHandler
    {
        private readonly IDbHandler _dbHandler;

        public AddPersonHandler()
        {
            var dbContext = new DynamoDBContext(new AmazonDynamoDBClient());
            _dbHandler = new DynamoDbHandler(dbContext);
        }

        public AddPersonHandler(IDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public async Task<APIGatewayProxyResponse> AddPerson(APIGatewayProxyRequest request)
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
            var requestBody = request.Body;
            var isRequestValid = Validator.ValidateRequest(requestBody);
            if (!isRequestValid)
                return new APIGatewayProxyResponse{StatusCode = 400, Body = "Invalid request - name must be between 0 and 30 characters"};
            var id = await GenerateNewId();
            await _dbHandler.AddPersonAsync(id, requestBody);
            return new APIGatewayProxyResponse
            {
                Body = requestBody,
                StatusCode = 200,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}, 
                    {"Location", $"{request.Path}/{id}"},
                }
            };
        }

        private async Task<int> GenerateNewId()
        {
            var people = await _dbHandler.GetPeopleAsync();
            if (people.Count == 0)
                return 0;
            var id = people.Select(p => p.Id).Max() + 1;
            return id;
        }
    }
}