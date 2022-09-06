namespace ZepLink.RiceNinja.Logger
{
    public abstract class LoggerBase : ILogger
    {
        public abstract void Log(string message);
    }
}
