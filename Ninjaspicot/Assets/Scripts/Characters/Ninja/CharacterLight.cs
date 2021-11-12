using UnityEngine;

public class CharacterLight : Lamp
{
    protected virtual void LateUpdate()
    {
        Transform.rotation = Quaternion.identity;
    }
}
