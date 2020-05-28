using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HelloWorld
{

    public class HelloWorldHandler
    {
        private readonly AmazonS3Client _client = new AmazonS3Client();
        public async Task<APIGatewayProxyResponse> HelloClients(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {
            var names = await GetNamesFromS3Bucket();
            var message = GetHelloMessage(names);
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

        private async Task<string> GetNamesFromS3Bucket()
        {
            const string bucketName = "david-ting-hello-world";
            var keys = (await _client.ListObjectsAsync(bucketName)).S3Objects.Select(o => o.Key);
            var names = new List<string>();
            foreach (var key in keys)
            {
                var obj = await _client.GetObjectAsync(bucketName, key);
                using var sr = new StreamReader(obj.ResponseStream);
                names.Add(await sr.ReadToEndAsync());
            }

            return string.Join(", ", names);
        }

        private static string GetHelloMessage(string name)
        {
            var time = DateTime.Now.ToShortTimeString();
            var date = DateTime.Now.ToLongDateString();
            return $"Hello {name} - the time on the server is {time} on {date}";
        }
    }
}
