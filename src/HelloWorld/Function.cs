using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HelloWorld
{

    public class Function
    {
        public APIGatewayProxyResponse HelloHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {
            var message = GetHelloMessage();
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

        private static string GetHelloMessage()
        {
            var time = DateTime.Now.ToShortTimeString();
            var date = DateTime.Now.ToLongDateString();
            return $"Hello David - the time on the server is {time} on {date}";
        }
    }
}
