using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    private List<ParallaxObject> _parallaxObjects;
    private CameraBehaviour _cameraBehaviour;

    private Vector3 _previousCameraPosition;

    private static ParallaxManager _instance;
    public static ParallaxManager Instance { get { if (_instance == null) _instance = FindObjectOfType<ParallaxManager>(); return _instance; } }

    public const int MIN_DEPTH = 5;
    public const int MAX_DEPTH = 20;
    public const float PARALLAX_FACTOR = 1.5f;
    public const float SCALE_AMPLITUDE = 1;


    private void Awake()
    {
        _parallaxObjects = new List<ParallaxObject>();
        _cameraBehaviour = CameraBehaviour.Instance;
    }

    private void Start()
    {
        _previousCameraPosition = _cameraBehaviour.transform.position;
    }

    private void Update()
    {
        var deltaPosition = _cameraBehaviour.transform.position - _previousCameraPosition;

        if (deltaPosition.magnitude == 0)
            return;

        OperateParallax(deltaPosition);

        _previousCameraPosition = _cameraBehaviour.transform.position;
    }

    public void AddObject(ParallaxObject parallaxObject)
    {
        if (_parallaxObjects.Contains(parallaxObject))
            return;

        _parallaxObjects.Add(parallaxObject);
    }

    private void OperateParallax(Vector3 delta)
    {
        _parallaxObjects.RemoveAll(o => o == null);

        foreach (var parallaxObject in _parallaxObjects)
        {
            if (parallaxObject == null)
            {
                _parallaxObjects.Remove(parallaxObject);
                continue;
            }

            parallaxObject.transform.position += new Vector3(delta.x, delta.y, 0) * parallaxObject.ParallaxFactor;
        }
    }
}