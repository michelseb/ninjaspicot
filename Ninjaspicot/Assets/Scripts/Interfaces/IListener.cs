using UnityEngine;

public interface IListener
{
    float Range { get; }
    void Hear(Vector3 source);
}
