using System.Collections;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] private bool _singleTime;
    [SerializeField] private GameObject _triggerableModel;
    public int Id { get; private set; }
    public bool SingleTime => _singleTime;
    private bool _ready;

    private ITriggerable _triggerable;

    private void Awake()
    {
        Id = gameObject.GetInstanceID();

        if (_triggerableModel != null)
        {
            _ready = true;
            _triggerable = _triggerableModel.GetComponent<ITriggerable>();
        }
        else
        {
            StartCoroutine(GetReady());
        }    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_ready)
            return;

        var triggerable = collision.GetComponent<ITriggerable>() ?? collision.GetComponentInParent<ITriggerable>();
        if (triggerable != null && triggerable == _triggerable)
        {
            _triggerable.StartTrigger(this);
        }
    }

    private IEnumerator GetReady()
    {
        while (Hero.Instance == null)
            yield return null;

        _triggerable = Hero.Instance.GetComponent<ITriggerable>();
        _ready = true;
    }
}
