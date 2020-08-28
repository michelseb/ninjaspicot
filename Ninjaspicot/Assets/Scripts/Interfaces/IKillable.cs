using System.Collections;
using UnityEngine;

public interface IKillable
{
    Transform Transform { get; }
    void Die(Transform killer);
    IEnumerator Dying();
}
