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
        //_previousCameraPosition = new Vector3(_cameraBehaviour.transform.position.x * PARALLAX_XFACTOR,
        //    _cameraBehaviour.transform.position.y * PARALLAX_YFACTOR,
        //    _cameraBehaviour.transform.position.z);

        _previousCameraPosition = _cameraBehaviour.transform.position;
    }

    private void Update()
    {
        var deltaPosition = _cameraBehaviour.transform.position - _previousCameraPosition;
        //var position = new Vector3(_cameraBehaviour.transform.position.x * PARALLAX_XFACTOR, 
        //    _cameraBehaviour.transform.position.y * PARALLAX_YFACTOR, 
        //    _cameraBehaviour.transform.position.z);

        if (deltaPosition.magnitude == 0)
            return;

        foreach (var parallaxObject in _parallaxObjects)
        {

            //var translate =  position - _previousCameraPosition;
            parallaxObject.transform.position += new Vector3(deltaPosition.x, deltaPosition.y, 0) * parallaxObject.ParallaxFactor;
            //parallaxObject.transform.Translate(-translate * parallaxObject.ParallaxFactor, Space.World);
        }

        _previousCameraPosition = _cameraBehaviour.transform.position;
    }

    public void AddObject(ParallaxObject parallaxObject)
    {
        if (_parallaxObjects.Contains(parallaxObject))
            return;

        _parallaxObjects.Add(parallaxObject);
    }
}