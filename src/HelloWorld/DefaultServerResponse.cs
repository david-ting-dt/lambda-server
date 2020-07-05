using System;
using Amazon.Lambda.APIGatewayEvents;

namespace HelloWorld
{
    public static class DefaultServerResponse
    {
        public static APIGatewayProxyResponse CreateServerErrorResponse()
        {
            return new APIGatewayProxyResponse
            {
                Body = "Internal server error",
                StatusCode = 500,
            };
        }
    }
}