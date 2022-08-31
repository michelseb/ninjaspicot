using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Map
{
    [RequireComponent(typeof(CompositeShadowCaster2D))]
    public class ShadowCaster : MonoBehaviour
    {
        [SerializeField] private bool _selfShadows = true;

        private CompositeCollider2D _tilemapCollider;
        public CompositeCollider2D TilemapCollider { get { if (BaseUtils.IsNull(_tilemapCollider)) _tilemapCollider = GetComponent<CompositeCollider2D>(); return _tilemapCollider; } }

        static readonly FieldInfo meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly MethodInfo generateShadowMeshMethod = typeof(ShadowCaster2D)
                                        .Assembly
                                        .GetType("UnityEngine.Rendering.Universal.ShadowUtility")
                                        .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

        // Generating with small delay to let colliders get calculated
        public void Generate()
        {
            Invoke(nameof(DoGenerate), .01f);
        }

        public void DoGenerate()
        {
            DestroyAllChildren();

            for (int i = 0; i < TilemapCollider.pathCount; i++)
            {
                
                var pathVertices = new Vector2[TilemapCollider.GetPathPointCount(i)];
                TilemapCollider.GetPath(i, pathVertices);

                var shadowCaster = new GameObject("shadow_caster_" + i);
                shadowCaster.transform.parent = transform;
                shadowCaster.gameObject.isStatic = true;

                var shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
                shadowCasterComponent.selfShadows = _selfShadows;


                shapePathField.SetValue(shadowCasterComponent, pathVertices.Select(x => new Vector3(x.x, x.y)).ToArray());
                meshField.SetValue(shadowCasterComponent, new Mesh());
                generateShadowMeshMethod.Invoke(shadowCasterComponent, new object[] { meshField.GetValue(shadowCasterComponent), shapePathField.GetValue(shadowCasterComponent) });

                shadowCasterComponent.OnAfterDeserialize();
            }
        }

        public void DestroyAllChildren()
        {
            var tempList = transform.Cast<Transform>().ToList();

            foreach (var child in tempList)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}
