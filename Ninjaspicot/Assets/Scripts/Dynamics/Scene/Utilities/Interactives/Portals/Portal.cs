using System.Collections;
using TMPro;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Cameras;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Portals
{
    public class Portal : Dynamic, IFocusable, IAudio
    {
        [SerializeField] private SpriteRenderer _imgInside;
        [SerializeField] private int _index;
        [SerializeField] private TextMeshProUGUI _title;

        public override int Id => _index;
        public bool Exit { get; set; }
        public bool Entrance { get; set; }
        public bool IsSilent => true;
        public bool Taken { get; set; }
        public Portal Other { get; private set; }

        private Zone _zone;
        public Zone Zone { get { if (BaseUtils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

        private AudioSource _audioSource;
        public AudioSource AudioSource { get { if (_audioSource == null) _audioSource = GetComponent<AudioSource>(); return _audioSource; } }

        public LayerMask TeleportedLayer;
        public SpriteRenderer[] TeleportedRenderers;
        public SpriteMaskInteraction SpriteMaskInteraction;
        private UICamera _uiCamera;
        private Coroutine _connect;
        private Coroutine _updateColor;
        private Animator _animator;
        private AudioFile _enterClip;
        private AudioFile _exitClip;
        private ITeleportable _currentTeleportable;

        private bool _titleVisible;


        private IPortalService _portalService;
        private IAudioService _audioService;

        private const float TITLE_DISTANCE = 50f;

        protected void Awake()
        {
            _portalService = ServiceFinder.Get<IPortalService>();
            _audioService = ServiceFinder.Get<IAudioService>();

            _uiCamera = UICamera.Instance;
            _animator = GetComponent<Animator>();
            _titleVisible = true;

            _enterClip = _audioService.FindByName("EnterPortal");
            _exitClip = _audioService.FindByName("ExitPortal");
        }

        protected void Update()
        {
            //if (Time.frameCount % BaseUtils.EXPENSIVE_FRAME_INTERVAL == 0)
            //{
            //    var distFromHero = Vector3.Distance(Hero.Instance.Transform.position, transform.position);

            //    if (distFromHero < TITLE_DISTANCE && !_titleVisible)
            //    {
            //        if (_updateColor != null)
            //        {
            //            StopCoroutine(_updateColor);
            //        }
            //        _updateColor = StartCoroutine(TitleAppear(_title, 2));
            //        _titleVisible = true;
            //    }
            //    else if (distFromHero >= TITLE_DISTANCE && _titleVisible)
            //    {
            //        if (_updateColor != null)
            //        {
            //            StopCoroutine(_updateColor);
            //        }
            //        _updateColor = StartCoroutine(TitleDisappear(_title, 2));
            //        _titleVisible = false;
            //    }
            //}
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (Taken)
                return;

            if (_connect != null)
                return;

            if (!_portalService.ConnectionExists(Id))
                return;

            if (!collision.TryGetComponent(out ITeleportable teleportable))
                return;

            _currentTeleportable = teleportable;
            _animator.SetTrigger("Wake");
            Taken = true;
            _uiCamera.CameraFade();
            _connect = StartCoroutine(Connect(teleportable));
        }

        private IEnumerator TitleAppear(TextMeshProUGUI text, float duration)
        {
            float t = 0;
            var col = text.color;
            var endColor = new Color(col.r, col.g, col.b, 1);
            while (t < duration)
            {
                t += Time.deltaTime;
                text.color = Color.Lerp(col, endColor, t);
                yield return null;
            }
            _updateColor = null;
        }

        private IEnumerator TitleDisappear(TextMeshProUGUI text, float duration)
        {
            float t = 0;
            var col = text.color;
            var endColor = new Color(col.r, col.g, col.b, 0);
            while (t < duration)
            {
                t += Time.deltaTime;
                text.color = Color.Lerp(col, endColor, t);
                yield return null;
            }
            _updateColor = null;
        }

        private IEnumerator Connect(ITeleportable teleportable)
        {
            _audioService.PlaySound(this, _enterClip);

            yield return new WaitForSecondsRealtime(2);

            _portalService.LaunchConnection(this);

            while (_portalService.Connecting)
                yield return null;

            if (Other == null || Exit)
            {
                Deactivate();
                yield break;
            }

            TeleportedRenderers = teleportable.Renderers;
            Other.TeleportedLayer = TeleportedLayer;
            SpriteMaskInteraction = TeleportedRenderers[0].maskInteraction;
            Other.SpriteMaskInteraction = SpriteMaskInteraction;

            foreach (var renderer in TeleportedRenderers)
            {
                renderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }

            TeleportedLayer = teleportable.InitialLayerMask;
            teleportable.Transform.gameObject.layer = LayerMask.NameToLayer("Teleported");

            _portalService.Teleport(teleportable, this, Other);
            _connect = null;
        }

        public void SetOtherPortal(Portal other)
        {
            Other = other;
        }

        public void Deactivate()
        {
            _animator.SetTrigger("Sleep");

            if (Entrance)
            {
                Entrance = false;
                return;
            }

            if (TeleportedRenderers != null)
            {
                foreach (var renderer in TeleportedRenderers)
                {
                    renderer.maskInteraction = SpriteMaskInteraction;
                }
            }

            _currentTeleportable.SetLayer(TeleportedLayer);
            Exit = false;
            Free();
        }

        public void Reserve()
        {
            Taken = true;
        }

        public void Free()
        {
            Taken = false;
        }
    }
}