using UnityEngine;

public class Door : MonoBehaviour, IActivable
{
    private Animator _animator;
    private bool _opened;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Activate()
    {
        if (_opened)
            return;

        _opened = true;
        _animator.SetTrigger("Open");
    }

    public void Deactivate()
    {
        if (!_opened)
            return;

        _opened = false;
        _animator.SetTrigger("Close");
    }
}
