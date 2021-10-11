using UnityEngine;

public class SoundEffect : MonoBehaviour, IPoolable
{
    private float _currentLifeTime;
    private const float LIFE_TIME = 1;
    private Transform _transform;

    public PoolableType PoolableType => PoolableType.None;
    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        _currentLifeTime = LIFE_TIME;
    }

    private void Update()
    {
        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime <= 0)
        {
            Sleep();
        }
    }

    public void Pool(Vector3 position, Quaternion rotation, float size)
    {
        _transform.localScale = Vector3.one * size;
        _transform.position = new Vector3(position.x, position.y, -5);
        _transform.rotation = rotation;
    }

    public void Sleep()
    {
        gameObject.SetActive(false);
    }

    public void Wake()
    {
        gameObject.SetActive(true);
        _currentLifeTime = LIFE_TIME;
    }
}
