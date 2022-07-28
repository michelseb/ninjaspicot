using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Scenery.Map;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class TileBrushService : ScriptableObjectService<TileBrush>, ITileBrushService
    {
        public override string ObjectsPath => "Brushes";

        public TileBrush FindByBrushType(BrushType brushType)
        {
            var result = Collection.FirstOrDefault(b => b.Type == brushType);

            if (BaseUtils.IsNull(result))
            {
                Debug.LogError($"Could not find brush of type {brushType}");
                return default;
            }

            return result;
        }
    }
}