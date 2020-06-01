using UnityEngine;

public class Trigger : MonoBehaviour
{
    private CapsuleCollider2D _collider;
    private Trajectory _trajectory;
    private CameraBehaviour _cameraBehaviour;
    private Hero _hero;
    private StealthyMovement _movement;
    private TimeManager _timeManager;
    private int _bulletsColliding;

    private bool _attacked;

    private void Awake()
    {
        _hero = Hero.Instance;
        _movement = _hero.Movement;
        _timeManager = TimeManager.Instance;
        _cameraBehaviour = CameraBehaviour.Instance;
        _trajectory = Trajectory.Instance;
        _collider = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        _bulletsColliding = 0;
    }

    private void Update()
    {
        if (_bulletsColliding <= 0 && _attacked)
        {
            _collider.size = new Vector2(10, 14);
            _cameraBehaviour.Zoom(ZoomType.Out);
            _timeManager.StartTimeRestore();
            _attacked = false;
        }
    }

    private void FixedUpdate()
    {
        _bulletsColliding = 0;
    }



    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            _bulletsColliding++;
            _attacked = true;
        }

        //if (collision.gameObject.CompareTag("Wall")) ????????????????????? What.
        //{
        //    if (_hero.HasBeenCurrentCollider(collision.gameObject))
        //    {
        //        if (_movement.isAttached)
        //        {
        //            _movement._walking = false;
        //            _movement.Detach();
        //        }
        //    }
        //}
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "bullet(Clone)")
        {
            if (_movement != null && _movement.GetJumps() > 0)
            {
                _collider.size = new Vector2(12, 16);
                _cameraBehaviour.Zoom(ZoomType.In, 20);
                //t.SlowDown(.03f);

            }
        }

        //if (collision.gameObject.tag == "Wall") ??????????????????????????????????????
        //{
        //    if (_hero.HasBeenCurrentCollider(collision.gameObject))
        //    {
        //        if (_movement.readyToJump)
        //        {
        //            _trajectory.ReinitTrajectory();
        //        }
        //        _movement._walking = false;
        //        _movement.Detach();
        //    }
        //}
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //if (_hero.CurrentAttachment != null) ??????????????????????????????????????
        //{
        //    if (collision.gameObject == _hero.CurrentAttachment || collision.gameObject.tag == "Wall" || _hero.CurrentAttachment.tag == "Wall")
        //    {
        //        if (_hero.HasBeenCurrentCollider(collision.gameObject))
        //        {
        //            if (_movement.readyToJump)
        //            {
        //                _trajectory.ReinitTrajectory();
        //            }
        //            _movement._walking = false;
        //            _movement.Detach();
        //            _movement.LoseJump();
        //        }

        //    }
        //}
    }

}
