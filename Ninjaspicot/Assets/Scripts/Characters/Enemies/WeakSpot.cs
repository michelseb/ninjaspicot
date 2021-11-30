﻿public class WeakSpot : AimableLocation
{
    private Enemy _enemy;
    public Enemy Enemy { get { if (_enemy == null) _enemy = GetComponentInParent<Enemy>(); return _enemy; } }

    public override bool Charge => true;

    public override bool Taken => base.Taken || Enemy.Dead;

    public override void GoTo()
    {
        base.GoTo();
        Enemy.Die();
    }
}
