using UnityEngine;

public class DynamicInteraction : MonoBehaviour
{
    [SerializeField] private GameObject _clone;

    public bool Interacting { get; set; }
    private IDynamic _tempDynamic;
    private Hero _hero;

    private GameObject _cloneHero;
    private GameObject _cloneDynamic;
    
    private Vector3 _clonePosition = new Vector3(1000, 1000);

    private void Awake()
    {
        _hero = GetComponent<Hero>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_tempDynamic != null)
            return;

        var dynamicEntity = collision.collider.GetComponent<IDynamic>();
        
        if (dynamicEntity == null)
            return;

        StartInteraction(dynamicEntity);
    }

    private void StartInteraction(IDynamic otherDynamic)
    {
        Interacting = true;
        _tempDynamic = otherDynamic;
        _hero.SetWalkingActivation(false);
        _hero.SetStickinessActivation(false);
        _hero.Rigidbody.isKinematic = true;

        var otherCollider = ((MonoBehaviour)otherDynamic).GetComponent<Collider2D>();

        _cloneHero = Instantiate(_clone, transform.position + _clonePosition, transform.rotation);

        _cloneDynamic = new GameObject("Clone dynamic");
        _cloneDynamic.transform.position = otherCollider.transform.position + _clonePosition;
        _cloneDynamic.transform.rotation = otherCollider.transform.rotation;
        Utils.CopyComponent(otherCollider, _cloneDynamic);

        _hero.transform.SetParent(_cloneHero.transform, true);
    }

    public void StopInteraction()
    {
        _tempDynamic = null;
        _hero.SetAllBehavioursActivation(true);
        _hero.Rigidbody.isKinematic = false;
        Interacting = false;

        _hero.transform.SetParent(null);

        Destroy(_cloneHero);
        Destroy(_cloneDynamic);

    }

}
