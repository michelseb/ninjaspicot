using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldOfView : MonoBehaviour, IActivable
{
    [SerializeField] protected float _size;
    [SerializeField] protected float _viewAngle;
    [SerializeField] protected int _detailAmount;
    [SerializeField] protected float _startAngle;
    [SerializeField] protected CustomColor _customColor;

    public bool Active { get; protected set; }
    public float Size => _size;
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
    private float _angleStep;
    private Quaternion _angleAxis;

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


        _angleStep = _viewAngle / _detailAmount;
        _angleAxis = Quaternion.AngleAxis(_viewAngle / 2f, Vector3.forward) * Quaternion.Euler(0, 0, _startAngle);

        Active = true;

        _mesh = InitMesh(_size, _detailAmount);
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
        if (UpdateVertices(_size, _detailAmount))
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

        if (UpdateVertices(_size, _detailAmount))
        {
            _mesh.vertices = _vertices.Select(x => x.ModifiedPos).ToArray();
            _filter.sharedMesh = _mesh;
        }
    }

    private Mesh InitMesh(float size, int pointCount)
    {
        var mesh = new Mesh();

        var initRotation = _transform.rotation.normalized;

        _vertices = new Vertex[pointCount + 1];
        var uvs = new Vector2[_vertices.Length];
        var triangles = new int[pointCount * 6];
        var colliderPoints = new List<Vector2>();

        colliderPoints.Add(Vector2.zero);
        _vertices[0] = new Vertex(Vector2.zero);

        var direction = (_angleAxis * _transform.up).normalized;

        var triangleIndex = 0;
        var colliderInterval = 3;

        for (int i = 1; i <= pointCount; i++)
        {
            if (i > 1)
            {
                direction = Quaternion.AngleAxis(-_angleStep, Vector3.forward).normalized * direction;
            }

            _vertices[i] = new Vertex(initRotation * direction * size);

            if (i % colliderInterval == 1)
            {
                colliderPoints.Add(initRotation * direction * size);
            }

            if (i < pointCount)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = i;
                triangles[triangleIndex + 2] = i + 1;
                triangles[triangleIndex + 3] = 0;
                triangles[triangleIndex + 4] = i + 1;
                triangles[triangleIndex + 5] = i;

                triangleIndex += 6;
            }
        }

        if (!colliderPoints.Contains(initRotation * direction * size))
        {
            colliderPoints.Add(initRotation * direction * size);
        }

        mesh.vertices = _vertices.Select(x => x.DefaultPos).ToArray();
        mesh.triangles = triangles;
        mesh.uv = uvs;


        var collPoints = colliderPoints.ToArray();
        _collider.points = collPoints;
        _collider.pathCount = 1;
        _collider.SetPath(0, collPoints);

        return mesh;
    }

    private bool UpdateVertices(float size, int pointCount)
    {
        var updated = false;
        var direction = (_angleAxis * _transform.up).normalized;

        for (int i = 1; i <= pointCount; i++)
        {
            if (i > 1)
            {
                direction = Quaternion.AngleAxis(-_angleStep, Vector3.forward).normalized * direction;
            }

            Debug.DrawRay(_transform.position, direction * size, Color.yellow); //=> gourmand

            var results = new RaycastHit2D[1];
            var hits = Physics2D.Raycast(_transform.position, direction, _contactFilter, results, size);
            if (hits > 0)
            {
                _vertices[i].Modified = true;
                _vertices[i].ModifiedPos = _transform.InverseTransformPoint(results[0].point);
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
