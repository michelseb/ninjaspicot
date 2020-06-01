using UnityEngine;

public class CheckPoint : DynamicEntity
{
    public bool Attained { get; private set; }
    private SpawnManager _spawnManager;

    private void Awake()
    {
        _spawnManager = SpawnManager.Instance;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }
}
