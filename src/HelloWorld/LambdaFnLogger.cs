using Amazon.Lambda.Core;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    /// <summary>
    /// Logs given message to AWS CloudWatch using LambdaLogger
    /// </summary>
    public class LambdaFnLogger : ILogger
    {
        public void Log(string message)
        {
            LambdaLogger.Log(message);
        }
    }
}