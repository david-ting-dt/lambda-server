using Amazon.Lambda.Core;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    public class LambdaFnLogger : ILogger
    {
        private readonly ILambdaLogger _logger;

        public LambdaFnLogger(ILambdaLogger logger)
        {
            _logger = logger;
        }

        public void Log(string message)
        {
            _logger.Log(message);
        }
    }
}