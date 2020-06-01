using UnityEngine;

[ExecuteInEditMode]
public class Mist : MonoBehaviour {

    private ParticleSystem _particles;
    private Renderer _renderer;

    private void Start () {
        _particles = GetComponent<ParticleSystem>();
        _renderer = _particles.GetComponent<Renderer>();
        _renderer.sortingLayerName = "Foreground";
    }
}
