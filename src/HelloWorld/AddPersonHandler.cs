using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    public class AddPersonHandler
    {
        private readonly IDataStore _dataStore;

        public AddPersonHandler()
        {
            _dataStore = new S3DataStore();
        }

        public AddPersonHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<APIGatewayProxyResponse> AddPerson(APIGatewayProxyRequest request)
        {
            var names = await _dataStore.Get();
            var requestBody = request.Body;
            if (names.Any(n => n == requestBody) || requestBody == "")
                return new APIGatewayProxyResponse {StatusCode = 202};

            var response = await _dataStore.Post(requestBody);
            return new APIGatewayProxyResponse
            {
                Body = requestBody,
                StatusCode = 200,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}, 
                    {"Location", $"{request.Path}/{requestBody}"},
                    {"ETag", response.ETag}
                }
            };
        }
    }
}