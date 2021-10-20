using UnityEngine;

public class HidingZone : MonoBehaviour
{
    private AimableLocation _aimableLocation;
    private Hero _hero;
    public Hero Hero { get { if (_hero == null) _hero = Hero.Instance; return _hero; } }

    private void Awake()
    {
        _aimableLocation = GetComponentInChildren<AimableLocation>();
        ActivateLocation();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        DeactivateLocation();
        Hero.Hide();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        ActivateLocation();
        Hero.Reveal();
    }

    private void ActivateLocation()
    {
        if (_aimableLocation == null)
            return;

        _aimableLocation.Active = true;
    }

    private void DeactivateLocation()
    {
        if (_aimableLocation == null)
            return;

        _aimableLocation.Active = false;
    }
}
