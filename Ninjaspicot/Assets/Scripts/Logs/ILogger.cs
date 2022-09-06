namespace ZepLink.RiceNinja.Logger
{
    public interface ILogger
    {
        /// <summary>
        /// Log event with message
        /// </summary>
        /// <param name="message"></param>
        void Log(string message);
    }
}