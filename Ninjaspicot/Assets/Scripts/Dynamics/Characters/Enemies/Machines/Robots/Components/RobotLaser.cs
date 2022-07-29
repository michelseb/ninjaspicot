using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Robots.Components
{
    public class RobotLaser : Dynamic, IActivable
    {
        protected LineRenderer _laser;
        protected ParticleSystem _dust;
        protected Enemy _enemy;
        private IAudioService _audioService;
        private AudioFile _electrocutionSound;
        protected int _pointsAmount;
        protected bool _active;
        public bool Active => _active;

        protected virtual void Awake()
        {
            _laser = GetComponent<LineRenderer>();
            _dust = GetComponentInChildren<ParticleSystem>();
            _enemy = GetComponentInParent<Enemy>();
            _pointsAmount = _laser.positionCount;
            _audioService = ServiceFinder.Get<IAudioService>();
        }

        protected virtual void Start()
        {
            _electrocutionSound = _audioService.FindByName("Electrocution");
        }

        protected void Update()
        {
            Cast();
        }

        private void Cast()
        {
            var cast = CastUtils.RayCast(Transform.position, Transform.right, ignore: _enemy.Id, includeTriggers: false);

            if (cast)
            {
                for (int i = 1; i < _pointsAmount; i++)
                {
                    var pos = transform.InverseTransformPoint(cast.point) / (_pointsAmount - i);
                    _laser.SetPosition(i, new Vector2(pos.x, pos.y) + new Vector2(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f)));
                }
                _dust.transform.position = cast.point;

                //Expensive ?
                if (cast.collider.TryGetComponent(out IKillable killable))
                {
                    killable.Die(sound: _electrocutionSound, volume: .4f);
                }
            }

        }

        public void SetActive(bool active)
        {
            _active = active;
            if (_active)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        public void Activate(IActivator activator = default)
        {
            gameObject.SetActive(true);
        }

        public void Deactivate(IActivator activator = default)
        {
            gameObject.SetActive(false);
        }
    }
}