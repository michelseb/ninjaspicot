using System.Collections;
using UnityEngine;

public interface IDestructable
{
    void Die(Transform killer);
    IEnumerator Dying();
}
