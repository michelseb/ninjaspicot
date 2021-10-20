using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ambiant : MonoBehaviour, ISceneryWakeable
{
    protected Animator _animator;

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Wake()
    {
        _animator.SetTrigger("Wake");
    }

    public void Sleep()
    {
        _animator.SetTrigger("Sleep");
    }
}
