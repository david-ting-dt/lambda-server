using Amazon.Lambda.APIGatewayEvents;

namespace HelloWorld
{
    /// <summary>
    /// Provides default server response for all functions
    /// </summary>
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