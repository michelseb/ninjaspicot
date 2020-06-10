using UnityEngine;

public class MovingCloud : DynamicObstacle
{
    [SerializeField] private int _xAmplitude;
    [SerializeField] private int _yAmplitude;
    [SerializeField] private float _speed;

    private int _xDir;
    private int _yDir;
    private float _xWay;
    private float _yWay;

    private Vector2 _initialPosition;
    private Rigidbody2D _rigidBody;

    public override void Awake()
    {
        base.Awake();
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    public virtual void Start()
    {
        _xDir = 1;
        _yDir = 1;

        _initialPosition = _rigidBody.position;
    }

    private void FixedUpdate()
    {
        var xMove = _xAmplitude > 0 ? _speed * _xDir : 0;
        var yMove = _yAmplitude > 0 ? _speed * _yDir : 0;

        _rigidBody.MovePosition(_rigidBody.position + new Vector2(xMove, yMove) * Time.deltaTime);

        var xSign = Mathf.Sign(_rigidBody.position.x - _initialPosition.x);
        var ySign = Mathf.Sign(_rigidBody.position.y - _initialPosition.y);


        if (Mathf.Abs(_rigidBody.position.x - _initialPosition.x) > _xAmplitude && xSign != _xWay)
        {
            _xDir = -_xDir;
            _xWay = xSign;
        }

        if (Mathf.Abs(_rigidBody.position.y - _initialPosition.y) > _yAmplitude && ySign != _yWay)
        {
            _yDir = -_yDir;
            _yWay = ySign;
        }
    }
}
