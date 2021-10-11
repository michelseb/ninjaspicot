using TMPro;
using UnityEngine;

public enum ReactionType
{
    Sleep = 0,
    Wonder = 1,
    Find = 2,
    Patrol = 3,
    Guard = 4
}

public class Reaction : MonoBehaviour, IPoolable
{
    [SerializeField] private Transform _text;

    public PoolableType PoolableType => PoolableType.None;

    public ReactionType ReactionType { get; private set; }
    private Transform _transform;
    private TextMeshPro _textMesh;
    public Transform Transform { get { if (_transform == null) _transform = transform; return _transform; } }

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



    public void SetReaction(ReactionType reactionType)
    {
        string reactionText = string.Empty;

        switch (reactionType)
        {
            case ReactionType.Sleep:
                reactionText = "Zzz";
                break;
            case ReactionType.Wonder:
                reactionText = "??";
                break;
            case ReactionType.Find:
                reactionText = "!!";
                break;
            case ReactionType.Patrol:
                reactionText = ">-<";
                break;
            case ReactionType.Guard:
                reactionText = "O-O";
                break;
        }

        ReactionType = reactionType;
        _textMesh.text = reactionText;
    }
}
