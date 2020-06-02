using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Interfaces;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HelloWorld
{

    public class HelloWorldHandler
    {
        private readonly IDataStore _dataStore = new S3DataStore();
        public async Task<APIGatewayProxyResponse> HelloClients(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {
            var names = await _dataStore.Get();
            using var sr = new StreamReader(names);
            var message = GetHelloMessage(await sr.ReadToEndAsync());
            var body = new Dictionary<string, string>
            {
                { "message", message },
            };

            return new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(body),
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
