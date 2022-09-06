using UnityEngine;

namespace ZepLink.RiceNinja.Logger
{
    public class ErrorLogger : LoggerBase
    {
        public override void Log(string message)
        {
            Debug.LogError(message);
        }
    }
}
