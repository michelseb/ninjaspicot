using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChargeTrajectory : TrajectoryBase
{
    public Enemy Target { get; set; }
    private List<Bonus> _bonuses;
    public List<Bonus> Bonuses { get { if (_bonuses == null) _bonuses = new List<Bonus>(); return _bonuses; } }

    public override CustomColor Color => CustomColor.Red;
    public override JumpMode JumpMode => JumpMode.Charge;

    private List<IActivable> _interactives;
    public List<IActivable> Interactives { get { if (_interactives == null) _interactives = new List<IActivable>(); return _interactives; } }

    public bool Collides { get; private set; }
    private AimIndicator _aimIndicator;
    private Transform _targetTransform;
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

        HandleTrajectoryHit(chargeHit, ref chargePos);

        if (_jumper != null)
        {
            _jumper.ChargeDestination = chargePos;
        }

        _line.SetPosition(1, chargePos);

        Interact(linePosition, chargePos);
    }

    private bool HandleTrajectoryHit(RaycastHit2D hit, ref Vector2 chargePos)
    {
        if (!hit)
        {
            DeactivateAim();
            Target = null;
            Collides = false;
            return false;
        }

        if (!HandleEnemyCast(hit, ref chargePos))
        {
            Target = null;
            SetAudioSimulator(_line.GetPosition(1), 5);

            if (!HandleFocusableCast(hit, ref chargePos))
            {
                DeactivateAim();
                chargePos = hit.point;
            }
        }

        Collides = true;

        return true;
    }

    private bool HandleFocusableCast(RaycastHit2D hit, ref Vector2 chargePos)
    {
        if (!hit.collider.CompareTag("Interactive"))
            return false;

        if (hit.collider.TryGetComponent(out IFocusable focusable))
        {
            if (focusable is MonoBehaviour focusableObject)
            {
                chargePos = focusableObject.transform.position;
                if (_aimIndicator == null) _aimIndicator = _poolManager.GetPoolable<AimIndicator>(chargePos, Quaternion.identity);
                _aimIndicator.Transform.position = chargePos;
            }

        }

        return true;
    }

    private bool HandleEnemyCast(RaycastHit2D hit, ref Vector2 chargePos)
    {
        if (!hit.collider.CompareTag("Enemy"))
            return false;

        var hitTransform = hit.collider.transform; 
        chargePos = hitTransform.position;

        // if other enemy
        if (Target != null && Vector2.Distance(chargePos, Utils.ToVector2(Target.transform.position)) > 1f)
        {
            DeactivateAim();
            Target = null;
        }

        if (Target == null)
        {
            if (hit.collider.TryGetComponent(out Enemy enemy))
            {
                Target = enemy;
                _targetTransform = enemy?.Renderer?.transform ?? enemy?.Image?.transform;
                var pos = _targetTransform?.position ?? chargePos;

                _aimIndicator = _poolManager.GetPoolable<AimIndicator>(pos, Quaternion.identity);
            }
            else
            {
                Target = hit.collider.GetComponentInParent<Enemy>();
                enemy = Target;
                _targetTransform = enemy?.Renderer?.transform ?? enemy?.Image?.transform;
                var pos = _targetTransform?.position ?? chargePos;

                _aimIndicator = _poolManager.GetPoolable<AimIndicator>(pos, Quaternion.identity);
            }
        }

        if (_aimIndicator != null)
        {
            _aimIndicator.Transform.position = _targetTransform.position;
        }

        return true;
    }

    private void Interact(Vector2 lineOrigin, Vector2 chargePos)
    {
        Interactives.Clear();
        Bonuses.Clear();

        var interactableDetect = Utils.LineCastAll(lineOrigin, chargePos, includeTriggers: true);
        var interactives = interactableDetect.Where(i => i.transform.CompareTag("Interactive")).ToArray();

        foreach (var interactive in interactives)
        {
            if (interactive.transform.TryGetComponent(out IActivable activable))
            {
                Interactives.Add(activable);
            }
        }

        var bonuses = interactableDetect.Where(b => b.transform.CompareTag("Bonus")).ToArray();
        foreach (var item in bonuses)
        {
            if (item.transform.TryGetComponent(out Bonus bonus))
            {
                Bonuses.Add(bonus);
            }
        }
    }

    private void DeactivateAim()
    {
        if (_aimIndicator == null)
            return;

        _aimIndicator.Sleep();
        _aimIndicator = null;
    }

    public override void StartFading()
    {
        base.StartFading();
        DeactivateAim();
    }
}