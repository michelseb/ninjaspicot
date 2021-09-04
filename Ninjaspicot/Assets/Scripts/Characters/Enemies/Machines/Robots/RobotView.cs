using UnityEngine;

public class RobotView : FieldOfView
{
    private GuardRobotBall _robot;

    protected override void Awake()
    {
        base.Awake();
        _robot = GetComponentInParent<GuardRobotBall>();
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);

        if (collider.CompareTag("hero"))
        {
            _robot.See(Hero.Instance.Transform);
        }
    }
}
