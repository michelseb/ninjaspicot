using System;
using UnityEngine;

namespace ZepLink.RiceNinja.Manageables.Audios
{
    [Serializable]
    public class AudioFile : IntManageable
    {
        [SerializeField] private string _name;

        public override string Name => _name;
        public AudioClip Clip;
    }
}
