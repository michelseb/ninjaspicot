using UnityEngine;
using ZepLink.RiceNinja.Manageables.Interfaces;
using ZepLink.RiceNinja.ServiceLocator;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Manageables.Scenes
{
    [CreateAssetMenu(fileName = "Scene", menuName = "Zeplink/Scene")]
    public class SceneInfos : ScriptableObject, IManageable<int>
    {
        [SerializeField] private AudioClip _audio;
        [SerializeField] private int _index;
        [SerializeField] private Texture2D _structureMap;
        [SerializeField] private Texture2D _zoneMap;
        [SerializeField] private Texture2D _utilitiesMap;
        [SerializeField] private Sprite _image;
        [SerializeField] private CustomColor _fontColor;
        [SerializeField] private CustomColor _globalLightColor;
        [SerializeField] private CustomColor _frontLightColor;

        public string Name => name;
        public AudioClip Audio => _audio;
        public int Id => _index;
        public Texture2D StructureMap => _structureMap;
        public Texture2D ZoneMap => _zoneMap;
        public Texture2D UtilitiesMap => _utilitiesMap;
        public Sprite Img => _image;
        public CustomColor FontColor => _fontColor;
        public CustomColor GlobalLightColor => _globalLightColor;
        public CustomColor FrontLightColor => _frontLightColor;
        public ServiceFinder ServiceFinder => ServiceFinder.Instance;
        public bool Loaded { get; private set; }

        public void Load()
        {
            Loaded = true;
        }

        public void Unload()
        {
            Loaded = false;
        }
    }
}
