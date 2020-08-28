using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldOfView : MonoBehaviour, IActivable
{
    [SerializeField] protected float _size;
    [SerializeField] protected float _viewAngle;
    [SerializeField] protected int _detailAmount;
    [SerializeField] protected Vector3 _offset;
    [SerializeField] protected float _startAngle;
    [SerializeField] protected CustomColor _customColor;

    public bool Active { get; protected set; }
    public float Size => _size;
    public Vector3 Offset => _offset;
    protected Mesh _mesh;
    protected MeshFilter _filter;
    protected IRaycastable _parent;
    protected PolygonCollider2D _collider;
    protected MeshRenderer _renderer;
    protected Color _color;
    protected Transform _transform;
    private Vertex[] _vertices;
    private ContactFilter2D _contactFilter;
    private bool _isVisible;
    private bool _isColliding;
    private int _collidingAmount;
    private int _colorPropertyId;
    private int _emissionPropertyId;

    protected const float TRANSPARENCY = .5f;

    protected virtual void Awake()
    {
        _transform = transform;
        _renderer = GetComponent<MeshRenderer>();
        _filter = GetComponent<MeshFilter>();
        _collider = GetComponent<PolygonCollider2D>();
        _parent = _transform.parent?.GetComponent<IRaycastable>();

        if (_customColor != CustomColor.None)
        {
            _color = ColorUtils.GetColor(_customColor, TRANSPARENCY);
            SetColor(_color);
        }

        _colorPropertyId = Shader.PropertyToID("_Color");
        _emissionPropertyId = Shader.PropertyToID("_EmissionColor");

        Active = true;
        _mesh = InitMesh(_size, _viewAngle, _detailAmount, _offset);
        _filter.sharedMesh = _mesh;
        _contactFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("DynamicObstacle") | 1 << LayerMask.NameToLayer("Enemy"),
            useTriggers = false
        };

    }

    protected virtual void Start()
    {
        if (UpdateVertices(_size, _viewAngle, _detailAmount, _offset))
        {
            _mesh.vertices = _vertices.Select(x => x.ModifiedPos).ToArray();
            _filter.sharedMesh = _mesh;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        _collidingAmount++;
        _isColliding = true;
    }

    protected virtual void OnTriggerExit2D(Collider2D collider)
    {
        if (_collidingAmount-- == 0)
        {
            _isColliding = false;
        }
    }

    protected virtual void LateUpdate()
    {
        if (!Active || !_isColliding || !_isVisible)
            return;

        if (UpdateVertices(_size, _viewAngle, _detailAmount, _offset))
        {
            _mesh.vertices = _vertices.Select(x => x.ModifiedPos).ToArray();
            _filter.sharedMesh = _mesh;
        }
    }

    private bool UpdateVertices(float size, float angle, int pointCount, Vector3 offset)
    {
        var updated = false;

        float angleStep = angle / pointCount;

        offset = _transform.TransformPoint(offset);
        var direction = _transform.TransformDirection(Quaternion.AngleAxis(angle / 2f, Vector3.forward) * _transform.InverseTransformDirection(_transform.up));

        RaycastHit2D[] results = new RaycastHit2D[1];

        //Debug.DrawRay(offset, direction * size, Color.yellow); //=> gourmand
        var hits = Physics2D.Raycast(offset, direction, _contactFilter, results, size);
        if (hits > 0)
        {
            _vertices[1].Modified = true;
            _vertices[1].ModifiedPos = _transform.InverseTransformPoint(results[0].point);
            updated = true;
        }
        else if (_vertices[1].Modified)
        {
            _vertices[1].ModifiedPos = _vertices[1].DefaultPos;
            _vertices[1].Modified = false;
            updated = true;
        }


        for (int i = 2; i <= pointCount; i++)
        {
            direction = Quaternion.AngleAxis(-angleStep, Vector3.forward) * direction;
            //Debug.DrawRay(offset, direction * size, Color.yellow); //=> gourmand
            hits = Physics2D.Raycast(offset, direction, _contactFilter, results, size);
            if (hits > 0)
            {
                _vertices[i - 1].Modified = true;
                _vertices[i - 1].ModifiedPos = _transform.InverseTransformPoint(results[0].point);
                updated = true;
            }
            else if (_vertices[i].Modified)
            {
                _vertices[i].ModifiedPos = _vertices[i].DefaultPos;
                _vertices[i].Modified = false;
                updated = true;
            }
        }
        return updated;
    }


    private Mesh InitMesh(float size, float angle, int pointCount, Vector3 offset)
    {
        var mesh = new Mesh();

        _vertices = new Vertex[pointCount + 1];
        var uvs = new Vector2[_vertices.Length];
        var triangles = new int[pointCount * 3];
        var points = new List<Vector2>();
        points.Add(offset);

        float angleStep = angle / pointCount;

        _vertices[0] = new Vertex(offset);

        var direction = _transform.TransformDirection(Quaternion.AngleAxis(angle / 2f, Vector3.forward) * _transform.InverseTransformDirection(_transform.up));

        var triangleIndex = 0;
        var colliderInterval = 3;

        for (int i = 1; i < pointCount - 1; i++)
        {
            direction = Quaternion.AngleAxis(-angleStep, Vector3.forward) * direction;
            _vertices[i] = new Vertex(direction * (size + offset.magnitude));
            if (i % colliderInterval == 1)
            {
                points.Add(direction * (size + offset.magnitude));
            }

            triangles[triangleIndex] = 0;
            triangles[triangleIndex + 1] = i - 1;
            triangles[triangleIndex + 2] = i;
            triangleIndex += 3;
        }
        points.Add(direction * (size + offset.magnitude));
        points.Add(offset);

        mesh.vertices = _vertices.Select(x => x.DefaultPos).ToArray();
        mesh.triangles = triangles;
        mesh.uv = uvs;


        var collPoints = points.ToArray();
        _collider.points = collPoints;
        _collider.pathCount = 1;
        _collider.SetPath(0, collPoints);

        return mesh;
    }

    protected void SetColor(Color color)
    {
        _renderer.material.SetColor(_colorPropertyId, color);
        _renderer.material.SetColor(_emissionPropertyId, color);
    }

    public void Activate()
    {
        _renderer.enabled = true;
        _collider.enabled = true;
        Active = true;
    }

    public void Deactivate()
    {
        _renderer.enabled = false;
        _collider.enabled = false;
        Active = false;
    }

    private void OnBecameVisible()
    {
        _isVisible = true;
    }

    private void OnBecameInvisible()
    {
        _isVisible = false;
    }

    internal struct Vertex
    {
        public Vector3 DefaultPos { get; set; }
        public Vector3 ModifiedPos { get; set; }
        public bool Modified { get; set; }
        public Vertex(Vector3 defaultPos)
        {
            Modified = false;
            ModifiedPos = defaultPos;
            DefaultPos = defaultPos;
        }
    }
}
