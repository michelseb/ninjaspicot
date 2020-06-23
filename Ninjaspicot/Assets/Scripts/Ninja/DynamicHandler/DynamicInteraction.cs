using UnityEngine;

public class DynamicInteraction : MonoBehaviour
{
    public bool Active { get; set; }
    public bool Interacting { get; set; }
    public Stickiness CloneHeroStickiness { get; private set; }

    private IDynamic _tempDynamic;
    private Hero _hero;

    private DynamicNinja _cloneHero;
    private DynamicNinjaCollider _cloneHeroCollider;
    private DynamicCollider _cloneOtherDynamic;

    private Vector3 _previousPosition;

    private PoolManager _poolManager;

    private void Awake()
    {
        _hero = GetComponent<Hero>();
        _poolManager = PoolManager.Instance;
    }

    private void Start()
    {
        Active = true;
    }

    private void Update()
    {
        if (!Active)
            return;

        if (_tempDynamic != null)
        {
            _hero.transform.localPosition += _cloneHeroCollider.transform.localPosition - _previousPosition;
            _hero.transform.rotation = _cloneHeroCollider.transform.rotation;
            _previousPosition = _cloneHeroCollider.transform.localPosition;
        }
    }

    public void StartInteraction(IDynamic otherDynamic)
    {
        Interacting = true;
        _tempDynamic = otherDynamic;

        _hero.SetWalkingActivation(false, true);
        _hero.SetStickinessActivation(false);

        var otherCollider = ((MonoBehaviour)otherDynamic).GetComponent<Collider2D>();

        _cloneOtherDynamic = _poolManager.GetPoolable<DynamicCollider>(otherCollider.transform.position, otherCollider.transform.rotation, otherDynamic.PoolableType);
        _cloneOtherDynamic.transform.localScale = otherCollider.transform.localScale;

        _cloneHeroCollider = _poolManager.GetPoolable<DynamicNinjaCollider>(transform.position, transform.rotation);
        CloneHeroStickiness = _cloneHeroCollider.GetComponent<Stickiness>();

        _cloneHero = _cloneHeroCollider.GetComponent<DynamicNinja>();
        _cloneHero.SetNinja(_hero);

        CloneHeroStickiness.Awake();
        CloneHeroStickiness.Start();

        CloneHeroStickiness.ContactPoint.SetParent(_cloneOtherDynamic.transform);

        _cloneHeroCollider.transform.SetParent(_cloneOtherDynamic.transform);
        _previousPosition = _cloneHeroCollider.transform.localPosition;

        _hero.transform.SetParent(otherCollider.transform);

        _hero.Stickiness.Rigidbody.angularVelocity = 0;
        _hero.Stickiness.Rigidbody.velocity = Vector2.zero;

        CloneHeroStickiness.ReactToObstacle(_cloneOtherDynamic, _hero.Stickiness.GetContactPosition());
    }

    public void StopInteraction(bool reinit)
    {
        _tempDynamic = null;
        _hero.transform.SetParent(null);

        if (reinit)
        {
            _hero.SetAllBehavioursActivation(true, false);
        }

        _hero.Stickiness.Rigidbody.isKinematic = false;
        Interacting = false;

        if (_cloneOtherDynamic != null && _cloneOtherDynamic.Active)
        {
            CloneHeroStickiness = null;
            _cloneHeroCollider.transform.SetParent(null);
            _cloneHeroCollider.Deactivate();
            _cloneOtherDynamic.Deactivate();
        }
    }

}
