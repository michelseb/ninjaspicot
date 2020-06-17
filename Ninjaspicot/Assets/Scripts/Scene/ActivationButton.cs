using UnityEngine;
using UnityEngine.UI;

public class ActivationButton : MonoBehaviour
{
    [SerializeField] private bool _active;
    [SerializeField] private GameObject _activableObject;
    public bool Pressing { get; set; }

    private IActivable _activable;
    private Image _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Image>();
    }

    private void Start()
    {
        _activable = _activableObject.GetComponent<IActivable>();
        _renderer.color = _active ? ColorUtils.Red : ColorUtils.Green;
        _activable.SetActive(_active);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hero") && !Pressing)
        {
            SetActive(ref _active);
            Pressing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Pressing = false;
    }

    private void SetActive(ref bool active)
    {
        active = !active;
        _renderer.color = active ? Color.red : Color.green;
        _activable.SetActive(active);
    }
}
