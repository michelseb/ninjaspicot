using UnityEngine;

public class KnifePivot : MonoBehaviour {

    [SerializeField]
    private float _speed;

    private float _initSpeed;
    private Knife _knife;
    private Rigidbody2D _rigidBody;

    private void Awake ()
    {
        _knife = GetComponentInChildren<Knife>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _initSpeed = _speed;
	}
	
	private void FixedUpdate ()
    {
        _rigidBody.MoveRotation(_rigidBody.rotation + _speed * Time.deltaTime);

        if (_speed > -200)
        {
            _speed += _initSpeed * Time.deltaTime;
        }

        if (_knife.touched)
        {
            _speed = -_initSpeed;
        }
	}
}
