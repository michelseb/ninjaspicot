using System;
using ZepLink.RiceNinja.Manageables.Animations;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class AnimationService : ScriptableObjectService<Guid, AnimationFile>, IAnimationService
    {
        public override string ObjectsPath => "Animations/Objects";
    }
}
