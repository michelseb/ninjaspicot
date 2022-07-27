using TMPro;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters;
using ZepLink.RiceNinja.Dynamics.Characters.Enemies;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Effects
{
    public class StateEffect : Dynamic, IPoolable
    {
        [SerializeField] private Transform _text;

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
}