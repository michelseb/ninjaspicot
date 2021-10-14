using UnityEngine;

public class HidingZone : MonoBehaviour
{
    private AimableLocation _aimableLocation;
    private Hero _hero;
    public Hero Hero { get { if (_hero == null) _hero = Hero.Instance; return _hero; } }

    private void Awake()
    {
        _aimableLocation = GetComponentInChildren<AimableLocation>();
        _aimableLocation.Active = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        _aimableLocation.Active = false;
        Hero.Hide();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        _aimableLocation.Active = true;
        Hero.Reveal();
    }
}
