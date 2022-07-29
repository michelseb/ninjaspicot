using System;
using UnityEngine;
using ZepLink.RiceNinja.Manageables.Abstract;

namespace ZepLink.RiceNinja.Manageables.Audios
{
    [Serializable]
    public class AudioFile : GuidManageable
    {
        [SerializeField] private string _name;

        public override string Name => _name;
        public AudioClip Clip;
    }
}
