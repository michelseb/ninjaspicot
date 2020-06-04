using System.Collections;
using UnityEngine;

public interface IKillable
{
    void Die(Transform killer);
    IEnumerator Dying();
}
