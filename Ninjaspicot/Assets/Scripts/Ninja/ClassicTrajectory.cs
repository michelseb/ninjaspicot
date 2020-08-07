using UnityEngine;

public class ClassicTrajectory : TrajectoryBase
{
    public override void DrawTrajectory(Vector2 linePosition, Vector2 click, Vector2 startClick)
    {
        Vector2 gravity = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y);
        Vector2 direction = (startClick - click).normalized;
        Vector2 velocity = direction * Strength;

        _line.positionCount = MAX_VERTEX;

        for (var i = 0; i < _line.positionCount; i++)
        {
            velocity = velocity + gravity * LENGTH;
            linePosition = linePosition + velocity * LENGTH;
            _line.SetPosition(i, new Vector3(linePosition.x, linePosition.y, 0));

            if (i > 1)
            {
                var hit = StepClear(_line.GetPosition(i - 1), _line.GetPosition(i - 2) - _line.GetPosition(i - 1), .1f);
                if (hit)
                {
                    if (hit.collider.gameObject.GetComponent<Obstacle>() != null)
                    {
                        _line.positionCount = i;
                        break;
                    }
                }
            }
        }
    }
}