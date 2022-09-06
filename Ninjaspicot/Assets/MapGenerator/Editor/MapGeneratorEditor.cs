using UnityEditor;
using UnityEngine;
using ZepLink.RiceNinja.Managers;

namespace DigitalRuby.LightningBolt
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var myScript = target as MapGenerator;

            if (GUILayout.Button("Generate map"))
            {
                myScript.Generate();
            }

            if (GUILayout.Button("Generate shadow casters"))
            {
                myScript.GenerateShadowCasters();
            }
        }
    }
}