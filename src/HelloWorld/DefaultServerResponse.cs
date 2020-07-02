using System;
using Amazon.Lambda.APIGatewayEvents;

namespace HelloWorld
{
    public static class DefaultServerResponse
    {
        public static APIGatewayProxyResponse CreateServerErrorResponse(Exception e)
        {
            return new APIGatewayProxyResponse
            {
                Body = e.ToString(),
                StatusCode = 500,
            };
        }
    }
}