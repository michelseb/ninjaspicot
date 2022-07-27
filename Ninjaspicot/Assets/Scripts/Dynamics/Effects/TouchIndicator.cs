﻿using System.Collections;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Effects
{
    public class TouchIndicator : Dynamic, IPoolable
    {
        public bool Active { get; private set; }

        private SpriteRenderer _renderer;
        private Coroutine _appear;
        private const float APPEAR_SPEED = 4f;
        private const float FADE_SPEED = 1.5f;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            var col = _renderer.color;
            _renderer.color = new Color(col.r, col.g, col.b, 0);
        }

        public void StartFading()
        {
            if (_appear != null)
            {
                StopCoroutine(_appear);
            }
            StartCoroutine(FadeAway());
        }


        private IEnumerator FadeAway()
        {
            Color col = _renderer.color;
            while (col.a > 0)
            {
                col = _renderer.color;
                col.a -= Time.deltaTime * FADE_SPEED;
                _renderer.color = col;
                yield return null;
            }
            Sleep();
        }

        private IEnumerator Appear()
        {
            Color col = _renderer.color;
            while (col.a < 1)
            {
                col = _renderer.color;
                col.a += Time.deltaTime * APPEAR_SPEED;
                _renderer.color = col;
                yield return null;
            }
            _appear = null;
        }

        public void Pool(Vector3 position, Quaternion rotation, float size)
        {
            Transform.position = new Vector3(position.x, position.y, -5);
            Transform.rotation = rotation;
            _appear = StartCoroutine(Appear());
        }

        public void Sleep()
        {
            Active = false;
            gameObject.SetActive(false);
        }

        public void Wake()
        {
            gameObject.SetActive(true);
            Active = true;
        }

        public void DoReset()
        {
            Sleep();
        }
    }
}