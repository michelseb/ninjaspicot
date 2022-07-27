using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters;
using ZepLink.RiceNinja.Dynamics.Characters.Enemies;
using ZepLink.RiceNinja.Dynamics.Effects.Lights;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Zones
{
    public class Zone : Dynamic, IWakeable
    {
        [SerializeField] protected GameObject _centerObject;
        private Vector3? _center;
        public Vector3? Center
        {
            get
            {
                if (_centerObject == null || !_centerObject.activeInHierarchy)
                    return null;

                if (_center == null) _center = _centerObject.transform.position;
                return _center;
            }
        }

        protected List<ISceneryWakeable> _wakeables;
        // Hack for door that is in the next zone
        [SerializeField] private List<GameObject> _additionalResettables;
        protected List<IResettable> _resettables;
        protected List<Enemy> _enemies;
        protected Animator _animator;
        protected CheckPoint _checkpoint;
        protected ISpawnService _spawnService;
        protected IZoneService _zoneService;
        protected AmbiantLight _ambiant;

        public bool DeathOccured => _enemies?.Any(e => e.Dead) ?? false;

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _spawnService = ServiceFinder.Get<ISpawnService>();
            _ambiant = GetComponentInChildren<AmbiantLight>();
        }

        protected virtual void Start()
        {
            _wakeables = GetComponentsInChildren<ISceneryWakeable>().ToList();
            _resettables = GetComponentsInChildren<IResettable>().ToList();
            _enemies = GetComponentsInChildren<Enemy>().ToList();
            _checkpoint = GetComponentInChildren<CheckPoint>();
        }

        public virtual void Open()
        {
            _animator.SetTrigger("Open");
            Wake();
        }

        public virtual void Close()
        {
            _animator.SetTrigger("Close");
            Sleep();
        }

        public virtual void CloseForever()
        {
            _animator.SetTrigger("Close");
            Sleep();
            Destroy(gameObject, 2f);
        }

        public void Wake()
        {
            for (int i = 0; i < _wakeables.Count; i++)
            {
                var item = _wakeables[i];

                if (BaseUtils.IsNull(item))
                {
                    _wakeables.RemoveAt(i);
                    continue;
                }

                item.Wake();
            }

            SetSpawn();
        }

        public void Sleep()
        {
            for (int i = 0; i < _wakeables.Count; i++)
            {
                var item = _wakeables[i];

                if (BaseUtils.IsNull(item))
                {
                    _wakeables.RemoveAt(i);
                    continue;
                }

                item.Sleep();
            }
        }

        public void ActivateAlarm()
        {
            _ambiant.SetColor(CustomColor.DarkRed, 1.5f);
            _enemies.ForEach(e => e.SetState(StateType.LookFor));
        }


        protected virtual void SetSpawn()
        {
            _spawnService.SetLatestSpawn(_checkpoint);
        }

        public void ResetItems()
        {
            _resettables.ForEach(r => r.DoReset());
            _additionalResettables.ForEach(r => { if (r.TryGetComponent(out IResettable resettable)) resettable.DoReset(); });
            var poolables = FindObjectsOfType<MonoBehaviour>().OfType<IPoolable>().ToList();
            poolables.ForEach(p => p.DoReset());
        }
    }
}