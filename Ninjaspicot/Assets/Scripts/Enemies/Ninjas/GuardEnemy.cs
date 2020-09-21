using UnityEngine;

public enum GuardMode
{
    Guarding,
    Checking,
    Chasing,
    Returning
}

public class GuardEnemy : EnemyNinja, IListener
{
    private Vector3 _initPos;
    private GuardMode _guardMode;
    private Vector3 _target;

    public float Range => 100f;

    protected override void Awake()
    {
        base.Awake();
        _initPos = Transform.position;
    }

    protected override void Update()
    {
        base.Update();

        switch (_guardMode)
        {
            case GuardMode.Guarding:
                Guard();
                break;

            case GuardMode.Checking:
                Check();
                break;

            case GuardMode.Chasing:
                Chase();
                break;

            case GuardMode.Returning:
                Return();
                break;

        }
    }

    public void Guard()
    {

    }

    public void Check()
    {

    }

    public void Chase()
    {

    }

    public void Return()
    {

    }

    public void Hear(Vector3 source)
    {
        if (_guardMode == GuardMode.Chasing)
            return;

        _guardMode = GuardMode.Checking;
        _target = source;
    }
}
