using UnityEngine;

public class UICamera : MonoBehaviour
{
    [SerializeField]
    private Animator _zoneChanger;
    public Camera Camera { get; private set; }
    public Canvas Canvas { get; private set; }

    private static UICamera _instance;
    public static UICamera Instance { get { if (_instance == null) _instance = FindObjectOfType<UICamera>(); return _instance; } }

    private void Awake()
    {
        Camera = GetComponent<Camera>();
        Canvas = GetComponentInChildren<Canvas>();
    }

    public void CameraFade()
    {
        _zoneChanger.SetTrigger("LevelEnd");
    }

    public void CameraAppear()
    {
        _zoneChanger.SetTrigger("LevelStart");
    }

}
