using UnityEngine;

public class HidingZone : MonoBehaviour, IFocusable
{
    private Hero _hero;
    public Hero Hero { get { if (_hero == null) _hero = Hero.Instance; return _hero; } }

    public bool IsSilent => true;

    public bool Active { get; set; }
    public bool Taken => !Active;

    public bool FocusedByNormalJump => true;

    private void Awake()
    {
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
        Active = true;
    }

    private void DeactivateLocation()
    {
        Active = false;
    }
}
