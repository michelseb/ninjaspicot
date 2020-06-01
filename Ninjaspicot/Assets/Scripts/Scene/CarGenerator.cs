using UnityEngine;

public class CarGenerator : MonoBehaviour {

    private float _coolDown;
    private float _timer;
    private bool _canGenerate;
    public GameObject car;

    private void Start () {
        _timer = _coolDown;
        _canGenerate = true;
	}

	private void Update () {
		if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            _canGenerate = true;
        }
        else
        {
            if (_canGenerate)
            {
                _timer = _coolDown;
                GenerateCar();
                _canGenerate = false;
            }
        }
	}

    private void GenerateCar()
    {
        Instantiate(car, transform.position, Quaternion.identity);
    }
}
