using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HelloWorld
{

    public class HelloWorldHandler
    {
        private readonly IDataStore _dataStore;

        public HelloWorldHandler()
        {
            _dataStore = new S3DataStore();
        }

        public HelloWorldHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<APIGatewayProxyResponse> HelloWorld()
        {
            var names = await _dataStore.Get();
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
