using UnityEngine;

public interface IViewer : IRaycastable
{
    void See(Transform target);
    bool Seeing { get; set; }
}
