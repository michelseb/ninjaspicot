using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZepLink.RiceNinja.Dynamics.Characters.Ninjas.MainCharacter;
using ZepLink.RiceNinja.Manageables.Abstract;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    [Serializable]
    public class Tutorial : GuidManageable
    {
        public string[] Instructions;
        public float Duration;
    }

    public class TutorialService : CollectionService<Guid, Tutorial>, ITutorialService
    {
        [SerializeField] private List<Tutorial> _tutorials;
        [SerializeField] private TextMeshProUGUI _instructionText;
        [SerializeField] private GameObject _clickText;
        [SerializeField] private List<GameObject> _itemsToAppear;
        private float _actionDuration;
        private int _itemIndex, _previousItemIndex;
        private int _tutorialIndex;

        private float _initialDistanceToInstruction;
        private Queue<string> _instructions;
        private Guid _tutorialLauncher;
        private Image _containerImage;

        private Hero _hero;

        private readonly ITouchService _touchService;
        private readonly ICameraService _cameraService;
        private readonly ICoroutineService _coroutineService;

        private bool _started;
        private Canvas _canvas;

        public TutorialService(ITouchService touchService, ICameraService cameraService, ICoroutineService coroutineService)
        {
            _touchService = touchService;
            _cameraService = cameraService;
            _coroutineService = coroutineService;
        }

        public override void Init(Transform parent)
        {
            base.Init(parent);

            _hero = UnityEngine.Object.FindObjectOfType<Hero>();
            
            _containerImage = ServiceObject.AddComponent<Image>();
            _canvas = ServiceObject.AddComponent<Canvas>();
            _containerImage.enabled = false;

            _canvas.worldCamera = _cameraService.MainCamera.Camera;
        }

        private void Update()
        {
            if (!_started)
                return;

            if (_itemIndex > _previousItemIndex)
            {
                ActivateItems(_previousItemIndex, _itemIndex - _previousItemIndex);
                _previousItemIndex = _itemIndex;
            }

            if (_instructions.Count() > 1)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    SetNextInstruction();
                }
            }
            else
            {
                var dist = Mathf.Sqrt((_hero.transform.position - _containerImage.transform.position).magnitude) + .01f;
                _containerImage.color = new Color(1, 1, 1, _initialDistanceToInstruction / dist);
                if (CheckComplete(ref _tutorialIndex, ref _actionDuration))
                {
                    _tutorialIndex++;
                    InitTutorial(_tutorialIndex);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (_tutorialLauncher == default && !_started && collision.CompareTag("hero") && _hero.ClimbSkill.Attached)
            {
                _coroutineService.StartCoroutine(LaunchTutorial(), out _tutorialLauncher);
            }
        }

        private void ActivateItems(int startIndex, int count)
        {
            _itemsToAppear.Skip(startIndex).Take(count).ToList().ForEach(i => i.SetActive(true));
        }

        private void SetNextInstruction()
        {
            if (_instructions.Count == 0)
                return;

            _instructions.Dequeue();
            SetInstruction();

            if (_instructions.Count == 1)
            {
                _initialDistanceToInstruction = Mathf.Sqrt((_hero.transform.position - _containerImage.transform.position).magnitude);
                _hero.SetJumpingActivation(true);
                _hero.SetWalkingActivation(true, false);
                _clickText.SetActive(false);
            }

        }

        private void SetInstruction()
        {
            var pos = _hero.transform.position + Vector3.down;
            ServiceObject.transform.position = new Vector3(pos.x, pos.y, ServiceObject.transform.position.z);
            _instructionText.text = _instructions.Peek();
        }

        private bool CheckComplete(ref int index, ref float duration)
        {
            if (duration <= 0)
                return true;


            if (index == 0) // Déplacement
            {
                if (_touchService.LeftSideTouching)
                {
                    duration -= Time.deltaTime;
                }
            }
            else if (index == 1) // Saut
            {
                if (_touchService.RightSideTouchDragging && duration > 1)
                {
                    duration = 1;
                }

                if (!_touchService.RightSideTouching && duration == 1)
                {
                    duration = 0;
                }

            }
            else if (index == 2) // Saut vers l'autre plateforme
            {
                _itemIndex = 1;
                if (_hero.ClimbSkill.Attached && _hero.ClimbSkill.CurrentAttachment?.transform == _itemsToAppear[_itemIndex - 1].transform)
                {
                    duration = 0;
                }
            }
            else if (index == 3) // Pièce
            {
                _itemIndex = 2;
                if (_itemsToAppear[_itemIndex - 1] == null)
                {
                    duration = 0;
                }
            }
            else if (index == 4) // Tourelle
            {
                _itemIndex = 5;
                if (_hero.Triggered && _hero.ClimbSkill.CurrentAttachment?.transform == _itemsToAppear[_itemIndex - 3].transform)
                {
                    duration = 0;
                }
            }
            else if (index == 5) // Monter tout en haut
            {
                _itemIndex = 6;
                if (_hero.ClimbSkill.CurrentAttachment?.transform == _itemsToAppear[_itemIndex - 1].transform)
                {
                    duration = 0;
                }
            }
            else if (index == 6) // Checkpoint
            {
                if (_hero.Triggered && _hero.ClimbSkill.CurrentAttachment?.transform == _itemsToAppear[_itemIndex - 1].transform)
                {
                    duration = 0;
                }
            }
            else if (index == 7) // Extra jump
            {
                _itemIndex = 8;
                if (_hero.ClimbSkill.CurrentAttachment?.transform == _itemsToAppear[_itemIndex - 2].transform)
                {
                    duration = 0;
                }
            }
            else if (index == 8) // GG
            {
                if (_hero.ClimbSkill.CurrentAttachment?.transform == _itemsToAppear[_itemIndex - 1].transform)
                {
                    duration = 0;
                }
            }
            else if (index == 9) // FallingCloud
            {
                _itemIndex = 10;
                if (_hero.ClimbSkill.CurrentAttachment?.transform == _itemsToAppear[_itemIndex - 2].transform)
                {
                    duration = 0;
                }
            }
            else if (index == 10) // FallingCloud
            {
                _itemIndex = 11;
                if (_hero.Triggered && _hero.ClimbSkill.CurrentAttachment?.transform == _itemsToAppear[_itemIndex - 2].transform)
                {
                    duration = 0;
                }
            }
            return false;
        }

        private IEnumerator LaunchTutorial()
        {
            foreach (var item in _itemsToAppear)
            {
                item.SetActive(false);
            }
            yield return new WaitForSeconds(2);

            InitTutorial(0);
            _started = true;
            _tutorialLauncher = default;
        }

        private void InitTutorial(int tutorialId)
        {
            if (tutorialId >= _tutorials.Count)
                return;

            _containerImage.color = Color.white;
            _hero.SetJumpingActivation(false);
            _hero.SetWalkingActivation(false, false);
            _containerImage.gameObject.SetActive(true);
            _clickText.SetActive(true);
            _actionDuration = _tutorials[tutorialId].Duration;
            _instructions = new Queue<string>(_tutorials[tutorialId].Instructions);
            SetInstruction();
        }

    }
}