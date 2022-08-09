using UnityEngine;
using ZepLink.RiceNinja.Logger;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Abstract
{
    public abstract class GameService : IGameService
    {
        public virtual string Name => GetType().Name;
        public virtual GameObject ServiceObject { get; protected set; }

        public virtual DebugMode DebugMode => DebugMode.Error;

        public virtual void Init(Transform parent)
        {
            ServiceObject = new GameObject(Name);
            ServiceObject.transform.SetParent(parent);
        }

        public void Log(string message)
        {
            LoggerHelper.Log(message, DebugMode);
        }
    }
}
