using UnityEngine;

public class RobotView : FieldOfView
{
    private Robot _robot;

    protected override void Awake()
    {
        base.Awake();
        _robot = GetComponentInParent<Robot>();
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);

        if (collider.CompareTag("hero"))
        {
            _robot.See(Hero.Instance.Transform);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("hero"))
        {
            _robot.See(Hero.Instance.Transform);
        }
    }

    protected override void OnTriggerExit2D(Collider2D collider)
    {
        base.OnTriggerExit2D(collider);

        if (collider.CompareTag("hero"))
        {
            _robot.Seeing = false;
        }
    }
}
