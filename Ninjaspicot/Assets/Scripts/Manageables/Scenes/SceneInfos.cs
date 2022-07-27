using System;
using UnityEngine;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Manageables.Scenes
{
    [Serializable]
    public class SceneInfos : Manageable
    {
        [SerializeField] private string _name;
        [SerializeField] private AudioClip _audio;

        public override string Name => _name;
        public AudioClip Audio => _audio;
        public Sprite Img;
        public bool Loaded;
        public CustomColor FontColor;
        public CustomColor GlobalLightColor;
        public CustomColor FrontLightColor;
    }
}
