﻿//
// Lightning Bolt for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections.Generic;

namespace DigitalRuby.LightningBolt
{
    /// <summary>
    /// Types of animations for lightning bolts
    /// </summary>
    public enum LightningBoltAnimationMode
    {
        /// <summary>
        /// No animation
        /// </summary>
        None,

        /// <summary>
        /// Pick a random frame
        /// </summary>
        Random,

        /// <summary>
        /// Loop through each frame and restart at the beginning
        /// </summary>
        Loop,

        /// <summary>
        /// Loop through each frame then go backwards to the beginning then forward, etc.
        /// </summary>
        PingPong
    }

    /// <summary>
    /// Allows creation of simple lightning bolts
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class LightningBoltScript : MonoBehaviour
    {
        [Tooltip("The game object where the lightning will emit from. If null, StartPosition is used.")]
        public Transform StartObject;

        [Tooltip("The start position where the lightning will emit from. This is in world space if StartObject is null, otherwise this is offset from StartObject position.")]
        public Vector3 StartPosition;

        [Tooltip("The game object where the lightning will end at. If null, EndPosition is used.")]
        public Transform EndObject;

        [Tooltip("The end position where the lightning will end at. This is in world space if EndObject is null, otherwise this is offset from EndObject position.")]
        public Vector3 EndPosition;

        [Range(0, 8)]
        [Tooltip("How manu generations? Higher numbers create more line segments.")]
        public int Generations = 6;

        [Range(0.01f, 1.0f)]
        [Tooltip("How long each bolt should last before creating a new bolt. In ManualMode, the bolt will simply disappear after this amount of seconds.")]
        public float Duration = 0.05f;
        private float timer;

        [Range(0.0f, 1.0f)]
        [Tooltip("How chaotic should the lightning be? (0-1)")]
        public float ChaosFactor = 0.15f;

        [Tooltip("In manual mode, the trigger method must be called to create a bolt")]
        public bool ManualMode;

        [Range(1, 64)]
        [Tooltip("The number of rows in the texture. Used for animation.")]
        public int Rows = 1;

        [Range(1, 64)]
        [Tooltip("The number of columns in the texture. Used for animation.")]
        public int Columns = 1;

        [Tooltip("The animation mode for the lightning")]
        public LightningBoltAnimationMode AnimationMode = LightningBoltAnimationMode.PingPong;

        /// <summary>
        /// Assign your own random if you want to have the same lightning appearance
        /// </summary>
        [HideInInspector]
        [System.NonSerialized]
        public System.Random RandomGenerator = new System.Random();

        private LineRenderer _lineRenderer;
        private BoxCollider2D _collider;
        private List<KeyValuePair<Vector3, Vector3>> _segments = new List<KeyValuePair<Vector3, Vector3>>();
        private int _startIndex;
        private Vector2 _size;
        private Vector2[] _offsets;
        private int _animationOffsetIndex;
        private int _animationPingPongDirection = 1;
        private bool _orthographic;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
            _orthographic = (Camera.main != null && Camera.main.orthographic);
            _lineRenderer = GetComponent<LineRenderer>();
            _collider = GetComponent<BoxCollider2D>();
            _lineRenderer.positionCount = 0;
            UpdateFromMaterialChange();

            float distX = Mathf.Abs(StartObject.position.x - EndObject.position.x);
            float distY = Mathf.Abs(StartObject.position.y - EndObject.position.y);

            if (distX < distY)
            {
                _collider.size = new Vector2(5, distY);
                
                _collider.offset = StartObject.localPosition + new Vector3(0, distY / 2);
            }
            else
            {
                _collider.size = new Vector2(distX, 5);
                _collider.offset = StartObject.localPosition + new Vector3(distX / 2, 0);
            }
        }

        private void Update()
        {
            _orthographic = (Camera.main != null && Camera.main.orthographic);
            if (timer <= 0.0f)
            {
                if (ManualMode)
                {
                    timer = Duration;
                    _lineRenderer.positionCount = 0;
                }
                else
                {
                    Trigger();
                }
            }
            timer -= Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("hero"))
            {
                Hero.Instance.Die(_transform);
            }
        }

        private void GetPerpendicularVector(ref Vector3 directionNormalized, out Vector3 side)
        {
            if (directionNormalized == Vector3.zero)
            {
                side = Vector3.right;
            }
            else
            {
                // use cross product to find any perpendicular vector around directionNormalized:
                // 0 = x * px + y * py + z * pz
                // => pz = -(x * px + y * py) / z
                // for computational stability use the component farthest from 0 to divide by
                float x = directionNormalized.x;
                float y = directionNormalized.y;
                float z = directionNormalized.z;
                float px, py, pz;
                float ax = Mathf.Abs(x), ay = Mathf.Abs(y), az = Mathf.Abs(z);
                if (ax >= ay && ay >= az)
                {
                    // x is the max, so we can pick (py, pz) arbitrarily at (1, 1):
                    py = 1.0f;
                    pz = 1.0f;
                    px = -(y * py + z * pz) / x;
                }
                else if (ay >= az)
                {
                    // y is the max, so we can pick (px, pz) arbitrarily at (1, 1):
                    px = 1.0f;
                    pz = 1.0f;
                    py = -(x * px + z * pz) / y;
                }
                else
                {
                    // z is the max, so we can pick (px, py) arbitrarily at (1, 1):
                    px = 1.0f;
                    py = 1.0f;
                    pz = -(x * px + y * py) / z;
                }
                side = new Vector3(px, py, pz).normalized;
            }
        }

        private void GenerateLightningBolt(Vector3 start, Vector3 end, int generation, int totalGenerations, float offsetAmount)
        {
            if (generation < 0 || generation > 8)
            {
                return;
            }
            else if (_orthographic)
            {
                start.z = end.z = Mathf.Min(start.z, end.z);
            }

            _segments.Add(new KeyValuePair<Vector3, Vector3>(start, end));
            if (generation == 0)
            {
                return;
            }

            Vector3 randomVector;
            if (offsetAmount <= 0.0f)
            {
                offsetAmount = (end - start).magnitude * ChaosFactor;
            }

            while (generation-- > 0)
            {
                int previousStartIndex = _startIndex;
                _startIndex = _segments.Count;
                for (int i = previousStartIndex; i < _startIndex; i++)
                {
                    start = _segments[i].Key;
                    end = _segments[i].Value;

                    // determine a new direction for the split
                    Vector3 midPoint = (start + end) * 0.5f;

                    // adjust the mid point to be the new location
                    RandomVector(ref start, ref end, offsetAmount, out randomVector);
                    midPoint += randomVector;

                    // add two new segments
                    _segments.Add(new KeyValuePair<Vector3, Vector3>(start, midPoint));
                    _segments.Add(new KeyValuePair<Vector3, Vector3>(midPoint, end));
                }

                // halve the distance the lightning can deviate for each generation down
                offsetAmount *= 0.5f;
            }
        }

        public void RandomVector(ref Vector3 start, ref Vector3 end, float offsetAmount, out Vector3 result)
        {
            if (_orthographic)
            {
                Vector3 directionNormalized = (end - start).normalized;
                Vector3 side = new Vector3(-directionNormalized.y, directionNormalized.x, directionNormalized.z);
                float distance = ((float)RandomGenerator.NextDouble() * offsetAmount * 2.0f) - offsetAmount;
                result = side * distance;
            }
            else
            {
                Vector3 directionNormalized = (end - start).normalized;
                Vector3 side;
                GetPerpendicularVector(ref directionNormalized, out side);

                // generate random distance
                float distance = (((float)RandomGenerator.NextDouble() + 0.1f) * offsetAmount);

                // get random rotation angle to rotate around the current direction
                float rotationAngle = ((float)RandomGenerator.NextDouble() * 360.0f);

                // rotate around the direction and then offset by the perpendicular vector
                result = Quaternion.AngleAxis(rotationAngle, directionNormalized) * side * distance;
            }
        }

        private void SelectOffsetFromAnimationMode()
        {
            int index;

            if (AnimationMode == LightningBoltAnimationMode.None)
            {
                _lineRenderer.material.mainTextureOffset = _offsets[0];
                return;
            }
            else if (AnimationMode == LightningBoltAnimationMode.PingPong)
            {
                index = _animationOffsetIndex;
                _animationOffsetIndex += _animationPingPongDirection;
                if (_animationOffsetIndex >= _offsets.Length)
                {
                    _animationOffsetIndex = _offsets.Length - 2;
                    _animationPingPongDirection = -1;
                }
                else if (_animationOffsetIndex < 0)
                {
                    _animationOffsetIndex = 1;
                    _animationPingPongDirection = 1;
                }
            }
            else if (AnimationMode == LightningBoltAnimationMode.Loop)
            {
                index = _animationOffsetIndex++;
                if (_animationOffsetIndex >= _offsets.Length)
                {
                    _animationOffsetIndex = 0;
                }
            }
            else
            {
                index = RandomGenerator.Next(0, _offsets.Length);
            }

            if (index >= 0 && index < _offsets.Length)
            {
                _lineRenderer.material.mainTextureOffset = _offsets[index];
            }
            else
            {
                _lineRenderer.material.mainTextureOffset = _offsets[0];
            }
        }

        private void UpdateLineRenderer()
        {
            int segmentCount = (_segments.Count - _startIndex) + 1;
            _lineRenderer.positionCount = segmentCount;

            if (segmentCount < 1)
            {
                return;
            }

            int index = 0;
            _lineRenderer.SetPosition(index++, _segments[_startIndex].Key);

            for (int i = _startIndex; i < _segments.Count; i++)
            {
                _lineRenderer.SetPosition(index++, _segments[i].Value);
            }

            _segments.Clear();

            SelectOffsetFromAnimationMode();
        }

        /// <summary>
        /// Trigger a lightning bolt. Use this if ManualMode is true.
        /// </summary>
        public void Trigger()
        {
            Vector3 start, end;
            timer = Duration + Mathf.Min(0.0f, timer);
            if (StartObject == null)
            {
                start = StartPosition;
            }
            else
            {
                start = StartObject.position + StartPosition;
            }
            if (EndObject == null)
            {
                end = EndPosition;
            }
            else
            {
                end = EndObject.position + EndPosition;
            }
            _startIndex = 0;
            GenerateLightningBolt(start, end, Generations, Generations, 0.0f);
            UpdateLineRenderer();
        }

        /// <summary>
        /// Call this method if you change the material on the line renderer
        /// </summary>
        public void UpdateFromMaterialChange()
        {
            _size = new Vector2(1.0f / (float)Columns, 1.0f / (float)Rows);
            _lineRenderer.material.mainTextureScale = _size;
            _offsets = new Vector2[Rows * Columns];
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    _offsets[x + (y * Columns)] = new Vector2((float)x / Columns, (float)y / Rows);
                }
            }
        }
    }
}