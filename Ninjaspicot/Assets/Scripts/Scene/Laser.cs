using UnityEngine;

public class Laser : MonoBehaviour
{

    [SerializeField] private RectTransform _start;
    [SerializeField] private RectTransform _end;
    [SerializeField] private int _pointsAmount;

    private LineRenderer _laser;
    private PolygonCollider2D _collider;

    private void Awake()
    {
        _laser = GetComponent<LineRenderer>();
        _collider = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        _laser.positionCount = _pointsAmount;
        _laser.SetPosition(0, _start.position);
        _laser.SetPosition(_pointsAmount - 1, _end.position);
        SetCollider();
    }

    private void Update()
    {
        for (int i = 1; i < _pointsAmount - 1; i++)
        {
            var pos = _start.position + ((_end.position - _start.position) * (i + 1) / _pointsAmount);
            _laser.SetPosition(i, new Vector2(pos.x, pos.y) + new Vector2(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f)));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hero"))
        {
            Hero.Instance.Die();
        }
    }

    private void SetCollider()
    {
        var startCorners = new Vector3[4];
        var endCorners = new Vector3[4];
        _start.GetWorldCorners(startCorners);
        _end.GetWorldCorners(endCorners);
        _collider.SetPath(0, new Vector2[]
        {
            transform.InverseTransformPoint(startCorners[0]),
            transform.InverseTransformPoint(startCorners[3]),
            transform.InverseTransformPoint(endCorners[0]),
            transform.InverseTransformPoint(endCorners[3])
        });
    }
}
