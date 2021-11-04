using UnityEngine;

public class LegTarget : Dynamic
{
    private void Update()
    {
        CheckGround();
    }

    public void CheckGround()
    {
        var hit = Utils.RayCast(Transform.position, Vector3.down, 15, includeTriggers: false);
        
        if (hit.collider != null)
        {
            Transform.position = hit.point + Vector2.up * .1f;
        }
    }
}
