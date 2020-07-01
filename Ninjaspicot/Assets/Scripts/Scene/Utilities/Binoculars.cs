using System.Linq;
using UnityEngine;

public class Binoculars : CameraCatcher
{
    [SerializeField] private Transform _zoomCenter;

    private Collider2D _trigger;
    public override Transform ZoomCenter => _zoomCenter;

    protected override void Awake()
    {
        base.Awake();
        _trigger = GetComponentsInChildren<Collider2D>().FirstOrDefault(c => c.isTrigger);
        _trigger.enabled = false;
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("hero") || _trigger.enabled)
            return;

        Activate();
        _trigger.enabled = true;
    }

    protected override void OnTriggerEnter2D(Collider2D collision) { }


    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        _trigger.enabled = false;
    }
}
