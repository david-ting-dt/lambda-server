using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    public class AddPersonHandler
    {
        private readonly IDataStore _dataStore = new S3DataStore();

        public async Task<APIGatewayProxyResponse> AddPerson(APIGatewayProxyRequest request)
        {
            var names = await _dataStore.Get();
            var requestBody = request.Body;
            if (names.Any(n => n == requestBody) || requestBody == "")
                return new APIGatewayProxyResponse {StatusCode = 202};

            var response = await _dataStore.Post(requestBody);
            Console.WriteLine($"request: {requestBody}");
            return new APIGatewayProxyResponse
            {
                Body = requestBody,
                StatusCode = (int)response.HttpStatusCode,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}, 
                    {"Location", $"{request.Path}{requestBody}"}
                }
            };
        }
    }
}