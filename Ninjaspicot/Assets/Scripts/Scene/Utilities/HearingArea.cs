using UnityEngine;

public class HearingArea
{
    public Vector3? SourcePoint { get; set; }
    public LocationPoint ClosestLocation { get; set; }

    public Vector3 GetSource()
    {
        return SourcePoint ?? ClosestLocation.transform.position;
    }
}
