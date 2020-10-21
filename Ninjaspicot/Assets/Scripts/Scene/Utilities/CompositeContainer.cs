using System.Collections.Generic;
using UnityEngine;

public class CompositeContainer : MonoBehaviour, IRaycastable
{
    [SerializeField] private LocationPoint _locationPointModel;
    private CompositeCollider2D _collider;

    private List<LocationPoint> _locationPoints;

    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    private const float DISTANCE_BETWEEN_POINTS = 12f;

    private void Awake()
    {
        _collider = GetComponent<CompositeCollider2D>();
        _locationPoints = new List<LocationPoint>();
        SetPaths();
    }

    private void SetPaths()
    {
        for (int i = 0; i < _collider.pathCount; i++)
        {
            var points = new List<Vector2>();//[_collider.GetPathPointCount(i)];
            _collider.GetPath(i, points);

            var previousPoint = points[0];


            //Insert inBetween points
            foreach (var point in points.ToArray())
            {
                var pointsDistance = Vector2.Distance(point, previousPoint);

                if (pointsDistance > DISTANCE_BETWEEN_POINTS * 2)
                {
                    var inBetweenPointCount = (int)(pointsDistance / DISTANCE_BETWEEN_POINTS);

                    for (int p = 1; p <= inBetweenPointCount; p++)
                    {
                        var position = previousPoint + (point - previousPoint) * p/inBetweenPointCount;
                        points.Insert(points.IndexOf(point) + p, position);
                    }
                }

                previousPoint = point;
            }

            var iteration = 0;
            previousPoint = points[0];

            foreach (var point in points)
            {
                var pointsDistance = Vector2.Distance(point, previousPoint);

                if (_locationPoints.Count > 0 && pointsDistance < DISTANCE_BETWEEN_POINTS)
                    continue;

                var newPoint = Instantiate(_locationPointModel, new Vector3(point.x, point.y, -5) + transform.position, Quaternion.identity, transform);
                previousPoint = point;
                _locationPoints.Add(newPoint);
                newPoint.Init(iteration, Id);

                iteration++;
            }

        }
    }

    public LocationPoint GetClosestLocationPoint(Vector3 position)
    {
        var dist = float.MaxValue;
        LocationPoint p = null;

        foreach (var point in _locationPoints)
        {
            var newDist = Vector2.Distance(point.transform.position, position);
            if (newDist < dist)
            {
                dist = newDist;
                p = point;
            }
        }

        return p;
    }
}
