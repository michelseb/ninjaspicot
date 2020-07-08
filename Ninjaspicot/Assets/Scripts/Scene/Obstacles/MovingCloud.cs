using UnityEngine;

public class MovingCloud : DynamicObstacle
{
    [SerializeField] private int _xAmplitude;
    [SerializeField] private int _yAmplitude;

    private int _xDir;
    private int _yDir;
    private float _xWay;
    private float _yWay;

    private Vector2 _initialPosition;

    public virtual void Start()
    {
        _xDir = 1;
        _yDir = 1;

        _initialPosition = Rigidbody.position;
    }

    private void FixedUpdate()
    {
        var xMove = _xAmplitude > 0 ? _customSpeed * _xDir : 0;
        var yMove = _yAmplitude > 0 ? _customSpeed * _yDir : 0;

        Rigidbody.MovePosition(Rigidbody.position + new Vector2(xMove, yMove) * Time.deltaTime);

        var xSign = Mathf.Sign(Rigidbody.position.x - _initialPosition.x);
        var ySign = Mathf.Sign(Rigidbody.position.y - _initialPosition.y);


        if (Mathf.Abs(Rigidbody.position.x - _initialPosition.x) > _xAmplitude && xSign != _xWay)
        {
            _xDir = -_xDir;
            _xWay = xSign;
        }

        if (Mathf.Abs(Rigidbody.position.y - _initialPosition.y) > _yAmplitude && ySign != _yWay)
        {
            _yDir = -_yDir;
            _yWay = ySign;
        }
    }
}
