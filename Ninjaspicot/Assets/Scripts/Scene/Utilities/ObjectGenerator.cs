using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _object;
    [SerializeField] private int _amount;
    [SerializeField] private int _xAmplitude;
    [SerializeField] private int _yAmplitude;
    [SerializeField] private float _rotation;

    private void Awake()
    {
        StartCoroutine(WaitForScene());
        
    }

    private void GenerateObjects()
    {
        for (int i = 0; i < _amount; i++)
        {
            var x = transform.position.x + Random.Range(-_xAmplitude, _xAmplitude);
            var y = transform.position.y + Random.Range(-_yAmplitude, _yAmplitude);
            Instantiate(_object, new Vector3(x, y, -5), Quaternion.Euler(0, 0, _rotation), transform);
        }
    }

    private IEnumerator WaitForScene()
    {
        while(gameObject.scene != SceneManager.GetActiveScene())
        {
            yield return null;
        }

        GenerateObjects();
    }
}
