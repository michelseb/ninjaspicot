using UnityEngine;

public class TriggerExit : MonoBehaviour
{
    private Binoculars _binoculars;
    private Collider2D _collider;

    private void Awake()
    {
        _binoculars = GetComponentInParent<Binoculars>();
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        SetActive(false);
        _binoculars.Deactivate();
    }

    public void SetActive(bool active)
    {
        _collider.enabled = active;
    }
}
