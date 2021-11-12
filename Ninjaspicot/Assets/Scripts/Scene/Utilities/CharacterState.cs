using TMPro;
using UnityEngine;

public enum StateType
{
    Sleep = 0,
    Wonder = 1,
    Check = 2,
    Chase = 3,
    Return = 4,
    Patrol = 5,
    Guard = 6,
    LookFor = 7,
    Communicate = 8
}

public class CharacterState : Dynamic, IPoolable
{
    [SerializeField] private Transform _text;

    public PoolableType PoolableType => PoolableType.None;

    public StateType StateType { get; private set; }
    public StateType NextState { get; private set; }
    private TextMeshPro _textMesh;

    private void Awake()
    {
        _textMesh = GetComponentInChildren<TextMeshPro>();
    }

    private void LateUpdate()
    {
        _text.rotation = Quaternion.identity;
    }

    public void Pool(Vector3 position, Quaternion rotation, float size = 1)
    {
        Transform.position = new Vector3(position.x, position.y, -5);
        Transform.rotation = rotation;
        Transform.localScale = size * Vector3.one;
    }

    public void Wake()
    {
        gameObject.SetActive(true);
        var color = _textMesh.color;
        _textMesh.color = new Color(color.r, color.g, color.b, 1);
    }

    public void Sleep()
    {
        gameObject.SetActive(false);
    }



    public void SetState(StateType stateType)
    {
        string stateText = string.Empty;

        switch (stateType)
        {
            case StateType.Sleep:
                stateText = "Zzz";
                break;
            case StateType.Wonder:
                stateText = "??";
                break;
            case StateType.LookFor:
                stateText = ":O";
                break;
            case StateType.Chase:
                stateText = "!!";
                break;
            case StateType.Patrol:
                stateText = ">-<";
                break;
            case StateType.Guard:
                stateText = "O-O";
                break;
        }

        StateType = stateType;
        _textMesh.text = stateText;
    }

    public void SetNextState(StateType stateType)
    {
        NextState = stateType;
    }

    public void DoReset()
    {
        if (GetComponentInParent<Enemy>() == null)
        {
            Sleep();
        }
    }
}
