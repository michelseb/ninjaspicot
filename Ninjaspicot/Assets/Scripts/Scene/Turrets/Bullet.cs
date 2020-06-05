using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    public float Speed { get; set; }
    private float _currentLifeTime;
    private const float LIFE_TIME = 5;

    private void Start()
    {
        _currentLifeTime = LIFE_TIME;
    }

    private void Update()
    {
        transform.Translate(0, Speed, 0);
        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime <= 0)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hero"))
        {
            collision.GetComponent<Hero>().Die(transform);
            Deactivate();
        }
        if (collision.CompareTag("Wall"))
        {
            Deactivate();
        }

    }

    public void Pool(Vector3 position, Quaternion rotation)
    {
        gameObject.SetActive(true);
        transform.position = new Vector3(position.x, position.y, -5);
        transform.rotation = rotation;
        _currentLifeTime = LIFE_TIME;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
