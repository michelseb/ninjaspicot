using UnityEngine;

public abstract class Dynamic : MonoBehaviour
{
    protected Transform _transform;
    public virtual Transform Transform { get { if (Utils.IsNull(_transform)) _transform = transform; return _transform; } }
}
