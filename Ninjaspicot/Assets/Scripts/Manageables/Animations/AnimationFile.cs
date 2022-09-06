using System;
using UnityEngine;
using ZepLink.RiceNinja.Manageables.Interfaces;
using ZepLink.RiceNinja.ServiceLocator;

namespace ZepLink.RiceNinja.Manageables.Animations
{
    [CreateAssetMenu(fileName = "Animation", menuName = "Zeplink/Animation")]
    public class AnimationFile : ScriptableObject, IManageable<Guid>
    {
        private Guid _id;
        public Guid Id { get { if (_id == default) _id = Guid.NewGuid(); return _id; } }
        public string Name => name;

        public ServiceFinder ServiceFinder => ServiceFinder.Instance;

        public RuntimeAnimatorController AnimatorController;
    }
}
