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

    protected const float TRANSPARENCY = .5f;

    protected virtual void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _filter = GetComponent<MeshFilter>();
        _collider = GetComponent<PolygonCollider2D>();
        _parent = transform.parent?.GetComponent<IRaycastable>();

        if (_customColor != CustomColor.None)
        {
            _color = ColorUtils.GetColor(_customColor, TRANSPARENCY);
            SetColor(_color);
        }
    }

    protected virtual void Start()
    {
        Active = true;
        _mesh = InitMesh(_size, _viewAngle, _detailAmount, _offset);
        _filter.sharedMesh = _mesh;
        SetCollider(_mesh.vertices);
    }

    protected virtual void LateUpdate()
    {
        if (!Active)
            return;

        _mesh = GenerateMesh(_size, _viewAngle, _detailAmount, _offset);
        _filter.sharedMesh = _mesh;
    }

    private Mesh GenerateMesh(float size, float angle, int pointCount, Vector3 offset)
    {
        var mesh = new Mesh();
        pointCount = Mathf.Max(pointCount, 2);

        var vertices = new Vector3[pointCount + 2];
        var uv = new Vector2[vertices.Length];
        var triangles = new int[pointCount * 3];

        offset = transform.TransformPoint(offset);

        float angleStep = (float)angle / pointCount;

        vertices[0] = transform.InverseTransformPoint(offset);

        var currentAngle = Utils.GetAngleFromVector(transform.InverseTransformDirection(transform.up)) + angle / 3f;
        var vertexIndex = 1;
        var triangleIndex = 0;

        for (int i = 0; i < pointCount; i++)
        {
            Vector3 vertex;
            //Debug.DrawRay(offset, transform.TransformDirection(Utils.GetVectorFromAngle(currentAngle)) * size, Color.yellow);
            var hits = Physics2D.RaycastAll(offset, transform.TransformDirection(Utils.GetVectorFromAngle(currentAngle)), size);
            var hit = hits.FirstOrDefault(x => x && !x.collider.isTrigger && !x.collider.CompareTag("hero"));
            if (hit)
            {
                vertex = transform.InverseTransformPoint(hit.point);
            }
            else
            {
                vertex = Utils.GetVectorFromAngle(currentAngle) * (size + transform.InverseTransformPoint(offset).magnitude);
            }

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }

            vertexIndex++;
            currentAngle -= angleStep;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        return mesh;
    }


    private Mesh InitMesh(float size, float angle, int pointCount, Vector3 offset)
    {
        var mesh = new Mesh();
        pointCount = Mathf.Max(pointCount, 2);

        var vertices = new Vector3[pointCount + 2];
        var uv = new Vector2[vertices.Length];
        var triangles = new int[pointCount * 3];

        offset = transform.TransformPoint(offset);
        float angleStep = angle / pointCount;

        vertices[0] = transform.InverseTransformPoint(offset);

        var currentAngle = Utils.GetAngleFromVector(transform.InverseTransformDirection(transform.up)) + angle / 2f;
        var vertexIndex = 1;
        var triangleIndex = 0;

        for (int i = 0; i < pointCount; i++)
        {

            vertices[vertexIndex] = Utils.GetVectorFromAngle(currentAngle) * (size + transform.InverseTransformPoint(offset).magnitude);

            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }

            vertexIndex++;
            currentAngle -= angleStep;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        return mesh;
    }


    private void SetCollider(Vector3[] vertices)
    {
        var points = new List<Vector2>();
        points.Add(vertices[0]);
        for (int i = 1; i < vertices.Length - 3; i += 6)
        {
            points.Add(vertices[i]);
        }
        points.Add(vertices[vertices.Length - 2]);
        points.Add(vertices[0]);

        var collPoints = points.ToArray();
        _collider.points = collPoints;
        _collider.pathCount = 1;
        _collider.SetPath(0, collPoints);
    }

    protected void SetColor(Color color)
    {
        _renderer.material.SetColor("_Color", _color);
        _renderer.material.SetColor("_EmissionColor", _color);
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
}
