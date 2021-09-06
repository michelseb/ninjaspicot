using System.Collections;
using UnityEngine;

public interface IKillable
{
    bool Dead { get; }
    void Die(Transform killer = null, Audio sound = null, float volume = 1f);
    IEnumerator Dying();
    Transform Transform { get; }
}
