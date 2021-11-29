using UnityEngine;

public class DynamicNinja : MonoBehaviour
{
    private INinja _linkedNinja;
    public DynamicStickiness Stickiness { get; private set; }

    protected virtual void Awake()
    {
        Stickiness = GetComponent<DynamicStickiness>();
    }

    protected virtual void Update()
    {
        if (Stickiness.Walking && !NeedsToWalk())
        {
            Stickiness.StopWalking(true);
        }
    }

    public virtual bool NeedsToWalk()
    {
        return _linkedNinja.NeedsToWalk();
    }

    public void SetNinja(INinja ninja)
    {
        _linkedNinja = ninja;
    }
}
