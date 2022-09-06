using UnityEngine;

namespace ZepLink.RiceNinja.Logger
{
    public class InformationLogger : LoggerBase
    {
        public override void Log(string message)
        {
            Debug.Log(message);
        }
    }
}
