namespace ZepLink.RiceNinja.Logger
{
    public static class LoggerHelper
    {
        public static void Log(string message, DebugMode mode)
        {
            if (mode == DebugMode.Ignore)
                return;

            switch (mode)
            {
                case DebugMode.Warning:
                    new InformationLogger().Log(message);
                    break;

                case DebugMode.Error:
                    new ErrorLogger().Log(message);
                    break;
            }
        }
    }
}
