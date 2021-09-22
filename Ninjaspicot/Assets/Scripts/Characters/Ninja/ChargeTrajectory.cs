using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChargeTrajectory : TrajectoryBase
{
    public Enemy Target { get; private set; }
    private List<Bonus> _bonuses;
    public List<Bonus> Bonuses { get { if (_bonuses == null) _bonuses = new List<Bonus>(); return _bonuses; } }

    public override CustomColor Color => CustomColor.Red;
    public override JumpMode JumpMode => JumpMode.Charge;

    private List<IActivable> _interactives;
    public List<IActivable> Interactives { get { if (_interactives == null) _interactives = new List<IActivable>(); return _interactives; } }
    public bool Collides { get; private set; }
    protected override float _fadeSpeed => 5;
    private const float CHARGE_LENGTH = 60f;

    public override void DrawTrajectory(Vector2 linePosition, Vector2 direction)
    {
        //Vector2 gravity = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y);
        direction = -direction.normalized;
        //Vector2 velocity = direction * Strength;

        //bool anyTarget = false;

        _line.positionCount = 2; //MAX_VERTEX;

        _line.SetPosition(0, new Vector3(linePosition.x, linePosition.y, 0));
        var chargePos = linePosition + direction * CHARGE_LENGTH;

        var chargeHit = StepClear(linePosition, chargePos - linePosition, CHARGE_LENGTH);

        if (chargeHit)
        {
            if (chargeHit.collider.CompareTag("Enemy"))
            {
                //anyTarget = true;
                if (Target == null)
                {
                    if (chargeHit.collider.TryGetComponent(out Enemy enemy))
                    {
                        Target = enemy;
                    }
                    else
                    {
                        Target = chargeHit.collider.GetComponentInParent<Enemy>();
                    }
                }

                chargePos = chargeHit.collider.transform.position;
            }
            else
            {
                chargePos = chargeHit.point;
                Collides = true;
                SetAudioSimulator(_line.GetPosition(1), 3);
            }
        }
        else
        {
            Target = null;
            Collides = false;
        }

        if (_jumper != null)
        {
            _jumper.ChargeDestination = chargePos;
        }

        _line.SetPosition(1, chargePos);

        var interactableDetect = Utils.LineCastAll(linePosition, chargePos, includeTriggers: true);
        var interactives = interactableDetect.Where(i => i.transform.CompareTag("Interactive")).ToArray();

        Interactives.Clear();
        foreach (var interactive in interactives)
        {
            if (interactive.transform.TryGetComponent(out IActivable activable))
            {
                Interactives.Add(activable);
            }
        }

        var bonuses = interactableDetect.Where(b => b.transform.CompareTag("Bonus")).ToArray();
        Bonuses.Clear();
        foreach (var item in bonuses)
        {
            if (item.transform.TryGetComponent(out Bonus bonus))
            {
                Bonuses.Add(bonus);
            }
        }

        //linePosition = chargePos;

        // NE PAS SUPPRIMER => ANCIENNE TRAJECTOIRE DE LA CHARGE

        //for (var i = 2; i < _line.positionCount; i++)
        //{
        //    velocity = velocity + gravity * LENGTH;
        //    linePosition = linePosition + velocity * LENGTH;
        //    _line.SetPosition(i, new Vector3(linePosition.x, linePosition.y, 0));

        //    if (i > 1)
        //    {
        //        var hit = StepClear(_line.GetPosition(i - 1), _line.GetPosition(i - 2) - _line.GetPosition(i - 1), .1f);
        //        if (hit)
        //        {
        //            if (hit.collider.CompareTag("Enemy"))
        //            {
        //                anyTarget = true;
        //                if (Target == null)
        //                {
        //                    Target = chargeHit.collider.GetComponent<EnemyNinja>();
        //                }
        //            }
        //            if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("DynamicWall"))
        //            {
        //                _line.positionCount = i;
        //                SetAudioSimulator(_line.GetPosition(i - 1), 30);
        //                break;
        //            }
        //        }
        //    }
        //}
        //if (!anyTarget && Target != null) // Reinit target
        //{
        //    Target = null;
        //}
    }
}