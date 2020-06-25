using Amazon.Lambda.APIGatewayEvents;

namespace HelloWorld
{
    public static class DefaultServerResponse
    {
        public static APIGatewayProxyResponse CreateServerErrorResponse()
        {
            return new APIGatewayProxyResponse
            {
                Body = "Ah oh... you've crashed our server :(",
                StatusCode = 500,
            };
        }
    }
}