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

    protected override void Awake()
    {
        base.Awake();
        _line.positionCount = 2;
    }

    public override void DrawTrajectory(Vector2 linePosition, Vector2 direction)
    {
        _line.SetPosition(0, new Vector3(linePosition.x, linePosition.y, 0));

        var chargePos = linePosition - direction.normalized * CHARGE_LENGTH;
        var chargeHit = StepClear(linePosition, chargePos - linePosition, CHARGE_LENGTH);

        if (chargeHit)
        {
            if (chargeHit.collider.CompareTag("Enemy"))
            {

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
                Target = null;
                SetAudioSimulator(_line.GetPosition(1), 5);
            }

            Collides = true;
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
    }
}