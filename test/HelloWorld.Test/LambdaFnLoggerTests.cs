using Amazon.Lambda.Core;
using Moq;
using Xunit;

namespace HelloWorld.Tests
{
    public class LambdaFnLoggerTests
    {
        [Fact]
        public void Log_ShouldCallILambdaLoggerLogMethod_WhenInvoked()
        {
            var mockLambdaLogger = new Mock<ILambdaLogger>();
            var logger = new LambdaFnLogger(mockLambdaLogger.Object);
            logger.Log("message");
            mockLambdaLogger
                .Verify(lambdaLogger => lambdaLogger.Log("message"), Times.Once);
        }
    }
}