using UnityEngine;

public class MovingCloud : Cloud {

    private Vector2 _originPos;
    private int _xWay = 1, _yWay = 1;
    private int _moveX, _moveY;
    private bool _reachedX, _reachedY;
    private float _speed;
    private Rigidbody2D _rigidBody;
    
    private void Start ()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        _originPos = _rigidBody.position;
        if (_moveX == 0)
        {
            _xWay = 0;
        }

        if (_moveY == 0)
        {
            _yWay = 0;
        }
    }
	
	private void FixedUpdate ()
    {
        _rigidBody.MovePosition(_rigidBody.position + new Vector2(_xWay, _yWay)*_speed*Time.deltaTime);//AddForce(new Vector2(xWay * speed * Time.deltaTime, yWay * speed * Time.deltaTime), ForceMode2D.Force);

        if (_rigidBody.position.x - _originPos.x > _moveX && _reachedX == false)
        {
            _xWay = -1;
            _reachedX = true;
        }else if (_rigidBody.position.x - _originPos.x < -_moveX && _reachedX == true)
        {
            _xWay = 1;
            _reachedX = false;
        }
        if (_rigidBody.position.y - _originPos.y > _moveY && _reachedY == false)
        {
            _yWay = -1;
            _reachedY = true;
        }
        else if (_rigidBody.position.y - _originPos.y < -_moveY && _reachedY == true)
        {
            _yWay = 1;
            _reachedY = false;
        }
    }
}
