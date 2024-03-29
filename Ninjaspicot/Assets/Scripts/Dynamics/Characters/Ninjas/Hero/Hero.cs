﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Cameras;
using ZepLink.RiceNinja.Dynamics.Characters.Components.Skills;
using ZepLink.RiceNinja.Dynamics.Characters.Hero.Components;
using ZepLink.RiceNinja.Dynamics.Characters.Ninjas.Components;
using ZepLink.RiceNinja.Dynamics.Effects;
using ZepLink.RiceNinja.Dynamics.Effects.Sounds;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Obstacles;
using ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives;
using ZepLink.RiceNinja.Helpers;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Characters.Ninjas.MainCharacter
{
    public class Hero : Character, IPhysic, ITriggerable, IPoolable, IShootable, ITeleportable, IControllable,
        ITracker, ISeeable, IHearable, ISpawnable, ISkilled, IActivator, IPicker
    {
        [SerializeField] float _ghostSpacing;
        public bool Triggered { get; private set; }
        public int LastTrigger { get; private set; }
        public bool Detected { get; private set; }
        public bool Visible { get; private set; }
        public int RevealerCount { get; private set; }

        #region Skills
        public HopSkill HopSkill { get; private set; }
        public ChargeSkill ChargeSkill { get; private set; }
        public HeroClimbSkill ClimbSkill { get; private set; }
        #endregion

        public EnergyBar EnergyBar { get; private set; }

        private ITimeService _timeService;
        private ISpawnService _spawnService;
        private IZoneService _zoneService;
        private ISkillService _skillService;
        private ILightService _lightService;
        private IComponentService _componentService;

        private Cloth _cape;
        private Coroutine _displayGhosts;
        private Coroutine _fade;
        private Coroutine _trigger;
        private UICamera _uiCamera;

        public SpriteRenderer[] Renderers => GetComponentsInChildren<SpriteRenderer>();
        public LayerMask InitialLayerMask { get; private set; }
        public Vector3 NormalVector => ClimbSkill.CollisionNormal;
        public Vector3 Direction => !ClimbSkill.Walking ? Vector3.zero : Quaternion.Euler(0, 0, -ClimbSkill.Direction * 90) * NormalVector;
        public Rigidbody2D Rigidbody => ClimbSkill.Rigidbody;
        public bool CanTeleport => !Dead;
        public bool CanTake => !Dead;

        public IList<ISkill> Skills { get; } = new List<ISkill>();

        private const int GHOST_SOUND_FREQUENCY = 3;
        private const float FADE_SPEED = 5f;

        private float _baseSize;
        private float _stretchSize;
        private float _meanSize;

        protected override void Awake()
        {
            base.Awake();

            _timeService = ServiceFinder.Get<ITimeService>();
            _spawnService = ServiceFinder.Get<ISpawnService>();
            _zoneService = ServiceFinder.Get<IZoneService>();
            _skillService = ServiceFinder.Get<ISkillService>();
            _lightService = ServiceFinder.Get<ILightService>();
            _componentService = ServiceFinder.Get<IComponentService>();

            _uiCamera = UICamera.Instance;
            _cape = GetComponentInChildren<Cloth>();
            InitialLayerMask = gameObject.layer;
            Visible = true;

            HopSkill = _skillService.EquipSkill<HopSkill>(Transform);
            ClimbSkill = _skillService.EquipSkill<HeroClimbSkill>(Transform);
            ChargeSkill = _skillService.EquipSkill<ChargeSkill>(Transform);

            EnergyBar = _componentService.Equip<EnergyBar>(Transform);
        }

        protected override void Start()
        {
            base.Start();

            // TODO => Go through all use cases
            EnergyBar.SetMaxValue(2f);
            EnergyBar.RegainAll();

            _baseSize = Transform.localScale.x;
            _stretchSize = Transform.localScale.y;
            _meanSize = (_baseSize + _stretchSize) / 2;
        }

        protected virtual void OnEnable()
        {
            //EnergyBar.OnEnergyConsumed += ClimbSkill.StopWalking;
            ClimbSkill.OnWalkStart += Shrink;
            ClimbSkill.OnWalkEnd += Stretch;
        }

        protected virtual void OnDisable()
        {
            //EnergyBar.OnEnergyConsumed -= ClimbSkill.StopWalking;
            ClimbSkill.OnWalkStart -= Shrink;
            ClimbSkill.OnWalkEnd -= Stretch;
        }

        protected virtual void Update()
        {
            if (ClimbSkill.Walking)
            {
                if (Vector3.Dot(NormalVector, Vector3.up) <= 0)
                {
                    EnergyBar.UseEnergy();
                }
                else
                {
                    EnergyBar.RegainAll();
                }
            }
        }

        public override void Die(Transform killer = null, AudioFile sound = null, float volume = 1f)
        {
            if (Dead)
                return;

            Dead = true;
            ClimbSkill.Detach();
            ChargeSkill?.CancelJump();

            if (killer != null)
            {
                ClimbSkill.Rigidbody.AddForce((Transform.position - killer.position).normalized * 10000);
            }

            if (sound != null)
            {
                _audioService.PlaySound(_audioSource, sound, volume);
            }

            SetAllBehavioursActivation(false, false);
            StopDisplayGhosts();

            Renderer.color = ColorUtils.Red;

            _timeService.SetTimeScaleProgressive(.3f);

            StartCoroutine(Dying());
        }

        public override IEnumerator Dying()
        {
            _audioService.PauseGlobal();

            yield return new WaitForSecondsRealtime(.5f);

            _uiCamera.CameraFade();

            yield return new WaitForSecondsRealtime(1.5f);

            _zoneService.CurrentZone.ResetItems();
            _timeService.SetNormalTime();
            SetCapeActivation(false);
            _spawnService.SpawnAtLastSpawningPosition(this);
            SetAllBehavioursActivation(true, false);
            SetCapeActivation(true);
            _uiCamera.CameraAppear();
            _audioService.ResumeGlobal();
        }

        public void SetCapeActivation(bool active)
        {
            _cape.GetComponent<SkinnedMeshRenderer>().enabled = active;
            _cape.enabled = active;
        }

        public IEnumerator Trigger(EventTrigger trigger)
        {
            Triggered = true;
            LastTrigger = trigger.Id;

            yield return new WaitForSeconds(3);

            Triggered = false;

            if (trigger.SingleTime)
            {
                Destroy(trigger.gameObject);
            }
            _trigger = null;
        }

        public void Shrink()
        {
            //Transform.localScale = Vector3.one * _meanSize;
            InterpolationHelper<Transform, Vector3>.Execute(new TransformScaleInterpolation(Transform, Vector3.one * _meanSize, .1f));
        }

        public void Stretch()
        {
            InterpolationHelper<Transform, Quaternion>.Execute(new TransformRotationInterpolation(Transform, Quaternion.Euler(NormalVector.x, NormalVector.y, NormalVector.z), .1f));
            InterpolationHelper<Transform, Vector3>.Execute(new TransformScaleInterpolation(Transform, new Vector3(_baseSize, _stretchSize), .1f));
        }

        public bool IsTriggeredBy(int id)
        {
            return LastTrigger == id;
        }

        public void StartDisplayGhosts()
        {
            if (_displayGhosts != null)
                return;

            _displayGhosts = StartCoroutine(DisplayGhosts(_ghostSpacing));
        }

        public void StopDisplayGhosts()
        {
            if (_displayGhosts == null)
                return;

            StopCoroutine(_displayGhosts);
            _displayGhosts = null;
        }

        private IEnumerator DisplayGhosts(float delay)
        {
            int iteration = 0;
            while (true)
            {
                PoolHelper.Pool<Ghost>(Transform.position, Transform.rotation, 1f);

                if (iteration % GHOST_SOUND_FREQUENCY == 0)
                {
                    PoolHelper.Pool<SoundEffect>(Transform.position, Quaternion.identity, 2);
                }

                iteration++;

                yield return new WaitForSeconds(delay);
            }
        }

        private IEnumerator FadeAway()
        {
            Color col = Renderer.color;
            while (col.a > 0)
            {
                col = Renderer.color;
                col.a -= Time.deltaTime * FADE_SPEED;
                Renderer.color = col;
                yield return null;
            }
            ClimbSkill.Rigidbody.velocity = Vector2.zero;
            ClimbSkill.Rigidbody.isKinematic = true;
            _fade = null;
        }

        private IEnumerator DoAppear()
        {
            Color col = Renderer.color;
            while (col.a < 1)
            {
                col = Renderer.color;
                col.a += Time.deltaTime * FADE_SPEED;
                Renderer.color = col;
                yield return null;
            }
        }

        public void StartTrigger(EventTrigger trigger)
        {
            if (_trigger == null)
            {
                _trigger = StartCoroutine(Trigger(trigger));
            }
        }

        public void SetJumpingActivation(bool active)
        {
            HopSkill.SetActive(active);
        }

        public void SetClimbingActivation(bool active)
        {
            ClimbSkill.SetActive(active);
        }

        public void SetWalkingActivation(bool active, bool grounded)
        {
            if (!active)
            {
                ClimbSkill.StopWalking(grounded);
            }

            ClimbSkill.CanWalk = active;
        }

        public virtual void SetAllBehavioursActivation(bool active, bool grounded)
        {
            SetJumpingActivation(active);
            SetClimbingActivation(active);
            SetWalkingActivation(active, grounded);
        }

        public virtual void Hide()
        {
            RevealerCount--;

            if (RevealerCount <= 0)
            {
                Renderer.color = ColorUtils.Grey;
                _lightService.DimmAmbiant();
                Visible = false;
                RevealerCount = 0;
            }
        }

        public virtual void Reveal()
        {
            RevealerCount++;
            Renderer.color = ColorUtils.White;
            _lightService.BrightenAmbiant();
            Visible = true;
        }

        public override void Pool(Vector3 position, Quaternion rotation, float size = 1)
        {
            Transform.position = position;
        }

        public void Sleep()
        {
            gameObject.SetActive(false);
        }

        public void Wake()
        {
            gameObject.SetActive(true);
        }

        public override void DoReset()
        {
            Sleep();
        }

        public bool IsVisibleFrom(Vector3 position, int layerMask = 0)
        {
            return Visible && CastUtils.LineCast(position, Transform.position, layerMask: layerMask);
        }

        public void SetLayer(int layer)
        {
            gameObject.layer = layer;
        }

        #region IControllable
        public void OnLeftSideTouchInit() { }

        public void OnLeftSideTouch() { }

        public void OnLeftSideDrag()
        {
            ChargeSkill.AirMove();

            if (ClimbSkill.Attached && ClimbSkill.CanWalk) // && EnergyBar.HasEnergy)
            {
                ClimbSkill.StartWalking();
            }
        }

        public void OnLeftSideTouchEnd()
        {
            ClimbSkill.ReinitSpeed();
            ClimbSkill.StopWalking(true);
            StopDisplayGhosts();

            Rigidbody.velocity = new Vector2(0, Rigidbody.velocity.y);
        }

        public void OnRightSideTouchInit()
        {
            if (!ChargeSkill.CanJump())
                return;

            ChargeSkill.SetJumpPressing(true);
            ChargeSkill.RestoreGravity();
            ChargeSkill.ResetAirSpeedFactor();

            ClimbSkill.Detach();
            ChargeSkill.InitJump(Direction, NormalVector);
            Stretch();
        }

        public void OnRightSideTouch()
        {
            ChargeSkill.SlowDown();
        }

        public void OnRightSideTouchEnd()
        {
            ChargeSkill.RestoreGravity();
            ChargeSkill.ResetAirSpeedFactor();
            ChargeSkill.SetJumpPressing(false);
        }

        public void OnRightSideDrag(Vector2 direction)
        {
            if (ChargeSkill.CanJump() && !ClimbSkill.Attached)
            {
                _timeService.SlowDownImmediate();
                _characterLight.Wake();
                ChargeSkill.DisplayTrajectory(direction);
            }
            else
            {
                ChargeSkill?.CancelJump();
            }
        }

        public void OnRightSideDragEnd(Vector2 direction)
        {
            _characterLight.Sleep();
            _timeService.SetNormalTime();
            ClimbSkill.ReinitSpeed();
            ChargeSkill.RestoreGravity();
            ChargeSkill.ResetAirSpeedFactor();
            StopDisplayGhosts();

            if (ChargeSkill.Ready)
            {
                ClimbSkill.StopWalking(false);
                ChargeSkill.Jump(direction);
                _lightService.ChromaBlast();
                _cameraService.MainCamera.Shake();
            }
        }

        public void OnDoubleTouchRightSideDrag(Vector2 direction)
        {
            OnRightSideDrag(direction);
        }

        public void OnDoubleTouchLeftSideDrag(Vector2 direction)
        {
            ChargeSkill.AirMove();

            //ClimbSkill.StartRunning();
            //StartDisplayGhosts();
        }

        public void OnDoubleTouchRightSideDragEnd(Vector2 direction)
        {
            ClimbSkill.ReinitSpeed();
            ChargeSkill.RestoreGravity();
            ChargeSkill.ResetAirSpeedFactor();
            StopDisplayGhosts();

            if (ChargeSkill.Ready)
            {
                ClimbSkill.StopWalking(false);
                ChargeSkill.Jump(direction);
            }
        }

        #endregion

        public void SetTeleportState()
        {
            ClimbSkill.Rigidbody.velocity = Vector2.zero;
            ClimbSkill.Rigidbody.isKinematic = true;

            Disappear();
        }

        public void Disappear()
        {
            _fade = StartCoroutine(FadeAway());
        }

        public void Appear()
        {
            if (_fade != null)
            {
                StopCoroutine(_fade);
            }

            StartCoroutine(DoAppear());
        }

        public void InitSpawn()
        {
            ClimbSkill.StopWalking(false);
            ClimbSkill.Rigidbody.velocity = Vector2.zero;
            ClimbSkill.Rigidbody.rotation = 0;
            Dead = false;
            Renderer.color = Color.white;
        }

        public void LandOn(Obstacle obstacle, Vector3 contactPoint)
        {
            if (ClimbSkill.AttachDeactivated)
                return;

            ClimbSkill.LandOn(obstacle, contactPoint);
            //HopSkill.LandOn(obstacle, contactPoint);
            ChargeSkill.LandOn(obstacle, contactPoint);

            if (ClimbSkill.Walking)
            {
                Shrink();
            }
        }
    }
}