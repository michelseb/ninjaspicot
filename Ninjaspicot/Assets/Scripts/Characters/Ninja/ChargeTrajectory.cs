using UnityEngine;

public class ChargeTrajectory : TrajectoryBase
{
    public EnemyNinja Target { get; private set; }
    private Jumper _jumper;
    private const float CHARGE_LENGTH = 30f;

    public void SetJumper(Jumper jumper)
    {
        _jumper = jumper;
    }

    public override void DrawTrajectory(Vector2 linePosition, Vector2 direction)
    {
        Vector2 gravity = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y);
        direction = -direction.normalized;
        Vector2 velocity = direction * Strength;

        bool anyTarget = false;

        _line.positionCount = MAX_VERTEX;

        _line.SetPosition(0, new Vector3(linePosition.x, linePosition.y, 0));
        var chargePos = linePosition + direction * CHARGE_LENGTH;

        var chargeHit = StepClear(linePosition, chargePos - linePosition, Vector3.Distance(linePosition, chargePos));

        if (chargeHit)
        {
            if (chargeHit.collider.CompareTag("Enemy"))
            {
                anyTarget = true;
                if (Target == null)
                {
                    Target = chargeHit.collider.GetComponent<EnemyNinja>();
                }
            }
            else
            {
                chargePos = chargeHit.point - direction * 5;
            }
        }

        if (_jumper != null)
        {
            _jumper.ChargeDestination = chargePos;
        }

        _line.SetPosition(1, chargePos);
        linePosition = chargePos;

        for (var i = 2; i < _line.positionCount; i++)
        {
            velocity = velocity + gravity * LENGTH;
            linePosition = linePosition + velocity * LENGTH;
            _line.SetPosition(i, new Vector3(linePosition.x, linePosition.y, 0));

            if (i > 1)
            {
                var hit = StepClear(_line.GetPosition(i - 1), _line.GetPosition(i - 2) - _line.GetPosition(i - 1), .1f);
                if (hit)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        anyTarget = true;
                        if (Target == null)
                        {
                            Target = chargeHit.collider.GetComponent<EnemyNinja>();
                        }
                    }
                    if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("DynamicWall"))
                    {
                        _line.positionCount = i;
                        SetAudioSimulator(_line.GetPosition(i - 1), (int)velocity.magnitude / 50);
                        break;
                    }
                }
            }
        }
        if (!anyTarget && Target != null) // Reinit target
        {
            Target = null;
        }
    }
}