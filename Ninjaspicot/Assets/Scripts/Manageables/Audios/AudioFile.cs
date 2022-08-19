using System;
using UnityEngine;
using ZepLink.RiceNinja.Manageables.Interfaces;
using ZepLink.RiceNinja.ServiceLocator;

namespace ZepLink.RiceNinja.Manageables.Audios
{
    [CreateAssetMenu(fileName = "Audio file", menuName = "Zeplink/Audio")]
    public class AudioFile : ScriptableObject, IManageable<Guid>
    {
        private Guid _id;
        public Guid Id { get { if (_id == default) _id = Guid.NewGuid(); return _id; } }
        public string Name => name;

        public ServiceFinder ServiceFinder => ServiceFinder.Instance;

        public AudioClip Clip;
    }
}
