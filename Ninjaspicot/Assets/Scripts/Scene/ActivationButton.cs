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

        if (_active)
        {
            _activable.Activate();
        }
        else
        {
            _activable.Deactivate();
        }
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

        if (active)
        {
            _activable.Activate();
        }
        else
        {
            _activable.Deactivate();
        }
    }
}
