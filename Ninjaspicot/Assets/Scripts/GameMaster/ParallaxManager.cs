using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    private List<ParallaxObject> _parallaxObjects;
    private CameraBehaviour _cameraBehaviour;

    private Vector3 _previousCameraPosition;

    private static ParallaxManager _instance;
    public static ParallaxManager Instance { get { if (_instance == null) _instance = FindObjectOfType<ParallaxManager>(); return _instance; } }

    public const int MIN_DEPTH = 3;
    public const int MAX_DEPTH = 20;
    public const float PARALLAX_XFACTOR = .2f;
    public const float PARALLAX_YFACTOR = .05f;

    private void Awake()
    {
        _parallaxObjects = new List<ParallaxObject>();
        _cameraBehaviour = CameraBehaviour.Instance;
    }

    private void Start()
    {
        _previousCameraPosition = new Vector3(_cameraBehaviour.transform.position.x * PARALLAX_XFACTOR,
            _cameraBehaviour.transform.position.y * PARALLAX_YFACTOR,
            _cameraBehaviour.transform.position.z);
    }

    private void Update()
    {
        var position = new Vector3(_cameraBehaviour.transform.position.x * PARALLAX_XFACTOR, 
            _cameraBehaviour.transform.position.y * PARALLAX_YFACTOR, 
            _cameraBehaviour.transform.position.z);

        if (position == _previousCameraPosition)
            return;

        foreach (var parallaxObject in _parallaxObjects)
        {

            var translate =  position - _previousCameraPosition;
            parallaxObject.transform.Translate(-translate / parallaxObject.Depth, Space.World);
        }

        _previousCameraPosition = position;
    }

    public void AddObject(ParallaxObject parallaxObject)
    {
        if (_parallaxObjects.Contains(parallaxObject))
            return;

        _parallaxObjects.Add(parallaxObject);
    }
}