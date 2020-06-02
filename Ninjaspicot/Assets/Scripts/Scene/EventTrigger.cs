using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [SerializeField]
    private bool _singleTime;
    public int Id { get; private set; }
    public bool SingleTime => _singleTime;

    private void Awake()
    {
        Id = gameObject.GetInstanceID();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hero = collision.GetComponent<Hero>() ?? collision.GetComponentInParent<Hero>();
        if (hero != null)
        {
            StartCoroutine(hero.Trigger(this));
        }
    }
}
