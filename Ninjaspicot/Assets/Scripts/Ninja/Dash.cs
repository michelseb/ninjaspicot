using UnityEngine;

public class Dash : MonoBehaviour, IPoolable
{

    private float _currentLifeTime;
    private const float LIFE_TIME = 1;

    public PoolableType PoolableType => PoolableType.None;

    private void Start()
    {
        _currentLifeTime = LIFE_TIME;
    }

    private void Update()
    {
        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime <= 0)
        {
            Deactivate();
        }
    }

    public void Pool(Vector3 position, Quaternion rotation)
    {
        Activate();
        transform.position = new Vector3(position.x, position.y, -5);
        transform.rotation = rotation;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        _currentLifeTime = LIFE_TIME;
    }
}
