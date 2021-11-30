using UnityEngine;

public interface IViewer
{
    void See(Transform target);
    bool Seeing { get; set; }
}
