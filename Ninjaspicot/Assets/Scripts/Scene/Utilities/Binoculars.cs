using UnityEngine;

public class Binoculars : CameraCatcher
{
    [SerializeField] private Transform _zoomCenter;
    public override Transform ZoomCenter => _zoomCenter;
}
