namespace HelloWorld.Interfaces
{
    /// <summary>
    /// Provides expected Log method for Loggers
    /// </summary>
    public interface ILogger
    {
        void Log(string message);
    }
}