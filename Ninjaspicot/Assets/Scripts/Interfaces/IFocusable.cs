using UnityEngine;

public interface IFocusable
{
    bool IsSilent { get; }
    bool Taken { get; }
    Transform Transform { get; }
    void GoTo();
    bool Charge { get; }
}
