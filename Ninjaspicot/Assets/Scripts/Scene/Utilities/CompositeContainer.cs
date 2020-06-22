using UnityEngine;

public class CompositeContainer : MonoBehaviour, IRaycastable
{
    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }
}
