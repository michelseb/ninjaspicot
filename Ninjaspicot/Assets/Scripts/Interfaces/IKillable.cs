using System.Collections;
using UnityEngine;

public interface IKillable
{
    bool Dead { get; }
    void Die(Transform killer);
    IEnumerator Dying();
    Transform Transform { get; }
}
