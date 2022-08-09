using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Scenery.Map;
using ZepLink.RiceNinja.Logger;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class BrushService : ScriptableObjectService<Color, Brush>, IBrushService
    {
        public override string ObjectsPath => "Brushes";
        public override DebugMode DebugMode => DebugMode.Warning;
    }
}