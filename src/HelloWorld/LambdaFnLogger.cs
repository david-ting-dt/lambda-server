using Amazon.Lambda.Core;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    public class LambdaFnLogger : ILogger
    {
        public void Log(string message)
        {
            LambdaLogger.Log(message);
        }
    }
}