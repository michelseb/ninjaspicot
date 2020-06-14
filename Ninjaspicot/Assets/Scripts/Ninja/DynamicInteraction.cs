using UnityEngine;

public class DynamicInteraction : MonoBehaviour
{
    public bool Active { get; set; }
    public bool Interacting { get; set; }
    public Stickiness CloneHeroStickiness { get; private set; }

    private IDynamic _tempDynamic;
    private Hero _hero;

    private DynamicCollider _cloneHero;
    private DynamicCollider _cloneDynamic;

    private Vector3 _previousPosition;
    private Vector3 _clonePosition = new Vector3(10000, 10000);

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
            _hero.transform.localPosition += _cloneHero.transform.localPosition - _previousPosition;
            _hero.transform.rotation = _cloneHero.transform.rotation;
            _previousPosition = _cloneHero.transform.localPosition;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Active)
            return;

        if (_tempDynamic != null)
            return;

        var dynamicEntity = collision.collider.GetComponent<IDynamic>();

        if (dynamicEntity == null || !dynamicEntity.DynamicActive)
            return;

        StartInteraction(dynamicEntity);
    }

    private void StartInteraction(IDynamic otherDynamic)
    {
        Interacting = true;
        _tempDynamic = otherDynamic;
        _hero.SetWalkingActivation(false);
        _hero.SetStickinessActivation(false);
        _hero.Stickiness.Rigidbody.isKinematic = true;

        var otherCollider = ((MonoBehaviour)otherDynamic).GetComponent<Collider2D>();


        _cloneDynamic = _poolManager.GetPoolable<DynamicCollider>(otherCollider.transform.position + _clonePosition, otherCollider.transform.rotation, otherDynamic.PoolableType);
        _cloneDynamic.transform.localScale = otherCollider.transform.localScale;

        //new GameObject("Clone dynamic");
        //_cloneDynamic.transform.localScale = otherCollider.transform.localScale;
        //_cloneDynamic.transform.position = otherCollider.transform.position + _clonePosition;
        //_cloneDynamic.transform.rotation = otherCollider.transform.rotation;
        //Utils.CopyComponent(otherCollider, _cloneDynamic);

        _cloneHero = _poolManager.GetPoolable<DynamicCollider>(transform.position + _clonePosition, transform.rotation, PoolableType.HeroCollider);
        CloneHeroStickiness = _cloneHero.GetComponent<Stickiness>();

        CloneHeroStickiness.Awake();
        CloneHeroStickiness.Start();

        CloneHeroStickiness.ReactToObstacle(_cloneDynamic);
        CloneHeroStickiness.SetContactPosition(_hero.Stickiness.GetContactPosition());

        _cloneHero.transform.SetParent(_cloneDynamic.transform);
        _previousPosition = _cloneHero.transform.localPosition;

        _hero.transform.SetParent(otherCollider.transform);

        _hero.Stickiness.Rigidbody.angularVelocity = 0;
        _hero.Stickiness.Rigidbody.velocity = Vector2.zero;
    }

    public void StopInteraction(bool reinit)
    {
        _tempDynamic = null;
        _hero.transform.SetParent(null);

        if (reinit)
        {
            _hero.SetAllBehavioursActivation(true);
        }

        _hero.Stickiness.Rigidbody.isKinematic = false;
        Interacting = false;

        if (_cloneDynamic != null && _cloneDynamic.Active)
        {
            CloneHeroStickiness = null;
            _cloneHero.transform.SetParent(null);
            _cloneHero.Deactivate();
            _cloneDynamic.Deactivate();
        }
    }

}
