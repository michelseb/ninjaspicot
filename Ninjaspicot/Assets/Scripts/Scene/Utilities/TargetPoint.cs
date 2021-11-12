using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    [SerializeField] private bool _isAim;
    [SerializeField] private float _pauseAmount;
    public float PauseAmount => _pauseAmount;
    public bool IsAim => _isAim;
}