using System;
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
    protected Mesh _mesh;
    protected MeshFilter _filter;
    protected IRaycastable _parent;
    protected PolygonCollider2D _collider;
    protected MeshRenderer _renderer;
    protected Color _color;
    protected Transform _transform;
    private Vertex[] _vertices;
    private ContactFilter2D _contactFilter;
    private bool _calculateModifications;
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
    }

    protected virtual void Start()
    {
        Active = true;
        _mesh = InitMesh(_size, _viewAngle, _detailAmount, _offset);
        _filter.sharedMesh = _mesh;
        SetCollider(_mesh.vertices);
        _contactFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("DynamicObstacle") | 1 << LayerMask.NameToLayer("Enemy"),
            useTriggers = false
        };

        if (UpdateVertices(_size, _viewAngle, _detailAmount, _offset))
        {
            _mesh.vertices = _vertices.Select(x => x.ModifiedPos).ToArray();
            _filter.sharedMesh = _mesh;
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collider)
    {
        if (!Active || !_calculateModifications)
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

        var currentAngle = Utils.GetAngleFromVector(_transform.InverseTransformDirection(_transform.up)) + angle / 2f;

        RaycastHit2D[] results = new RaycastHit2D[1];

        for (int i = 1; i <= pointCount; i++)
        {
            Debug.DrawRay(_transform.TransformPoint(offset), transform.TransformDirection(Utils.GetVectorFromAngle(currentAngle)) * size, Color.yellow);
            var hits = Physics2D.Raycast(_transform.TransformPoint(offset), _transform.TransformDirection(Utils.GetVectorFromAngle(currentAngle)), _contactFilter, results, size);
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

            currentAngle -= angleStep;
        }
        return updated;
    }


    private Mesh InitMesh(float size, float angle, int pointCount, Vector3 offset)
    {
        var mesh = new Mesh();

        _vertices = new Vertex[pointCount + 1];
        var uvs = new Vector2[_vertices.Length];
        var triangles = new int[pointCount * 3];

        float angleStep = angle / pointCount;

        _vertices[0] = new Vertex(offset);
        var currentAngle = Utils.GetAngleFromVector(_transform.InverseTransformDirection(_transform.up)) + angle / 2f;
        var triangleIndex = 0;

        for (int i = 1; i <= pointCount; i++)
        {
            _vertices[i] = new Vertex(Utils.GetVectorFromAngle(currentAngle) * (size + offset.magnitude));

            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = i - 1;
                triangles[triangleIndex + 2] = i;
                triangleIndex += 3;
            }

            currentAngle -= angleStep;
        }

        mesh.vertices = _vertices.Select(x => x.DefaultPos).ToArray();
        mesh.triangles = triangles;
        mesh.uv = uvs;

        return mesh;
    }


    private void SetCollider(Vector3[] vertices)
    {
        var points = new List<Vector2>();
        points.Add(vertices[0]);
        for (int i = 1; i < vertices.Length - 1; i += 6)
        {
            points.Add(vertices[i]);
        }
        points.Add(vertices[vertices.Length - 1]);
        points.Add(vertices[0]);

        var collPoints = points.ToArray();
        _collider.points = collPoints;
        _collider.pathCount = 1;
        _collider.SetPath(0, collPoints);
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
        _calculateModifications = true;
    }

    private void OnBecameInvisible()
    {
        _calculateModifications = false;
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
