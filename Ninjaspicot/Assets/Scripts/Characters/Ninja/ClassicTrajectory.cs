﻿using UnityEngine;

public class ClassicTrajectory : TrajectoryBase
{
    public override CustomColor Color => CustomColor.Blue;
    public override JumpMode JumpMode => JumpMode.Classic;

    private AnimationCurve _focusWidth;

    protected override void Awake()
    {
        base.Awake();

        _focusWidth = new AnimationCurve();
        _focusWidth.AddKey(0, 1);
        _focusWidth.AddKey(1, .1f);
    }

    public override void DrawTrajectory(Vector2 linePosition, Vector2 direction)
    {
        Vector2 gravity = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y);
        direction = -direction.normalized;
        Vector2 velocity = direction * Strength;

        _line.positionCount = MAX_VERTEX;
        var isContact = false;

        for (var i = 0; i < _line.positionCount; i++)
        {
            velocity += gravity * LENGTH;
            linePosition += velocity * LENGTH;
            _line.SetPosition(i, new Vector3(linePosition.x, linePosition.y));

            if (i > 1)
            {
                var hit = StepClear(_line.GetPosition(i - 1), _line.GetPosition(i - 2) - _line.GetPosition(i - 1), .1f);

                if (!hit)
                    continue;

                isContact = true;

                if (!HandleFocusableCast(hit, _line))
                {
                    DeactivateAim();

                    if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("DynamicWall"))
                    {
                        _line.positionCount = i;
                        break;
                    }
                }
            }
        }

        if (!isContact)
        {
            DeactivateAim();
        }
    }

    private bool HandleFocusableCast(RaycastHit2D hit, LineRenderer line)
    {
        if (!hit.collider.CompareTag("Interactive"))
            return false;

        if (!hit.collider.TryGetComponent(out IFocusable focusable))
            return false;

        if (focusable.Taken || !focusable.FocusedByNormalJump)
            return false;


        if (focusable is MonoBehaviour focusableObject)
        {
            var pos = focusableObject.transform.position;
            pos.z = 0;

            if (Utils.LineCast(line.GetPosition(0), pos, new int[] { Hero.Instance.Id }))
                return false;

            line.positionCount = 2;
            line.widthCurve = _focusWidth;
            line.SetPosition(1, new Vector3(pos.x, pos.y));
            ActivateAim(focusable, pos);
        }

        _jumper.JumpMode = JumpMode.Direct;
        _jumper.AimTarget = _aimIndicator.Transform.position;

        return true;
    }

    protected override void DeactivateAim()
    {
        base.DeactivateAim();
        ResetWidths();
        _jumper.JumpMode = JumpMode.Classic;
    }
}