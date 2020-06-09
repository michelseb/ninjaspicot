using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Tutorial
{
    public string[] Instructions;
    public float Duration;
}

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<Tutorial> _tutorials;
    [SerializeField] private GameObject _instructionsContainer;
    [SerializeField] private TextMeshProUGUI _instructionText;
    [SerializeField] private GameObject _clickText;
    [SerializeField] private List<GameObject> _itemsToAppear;
    private float _actionDuration;
    private int _itemIndex, _previousItemIndex;
    private int _tutorialIndex;

    private float _initialDistanceToInstruction;
    private Queue<string> _instructions;
    private Coroutine _tutorialLauncher;
    private Image _containerImage;

    private Hero _hero;
    private TouchManager _touchManager;
    private CameraBehaviour _cameraBehaviour;

    private bool _started;

    private void Awake()
    {
        _hero = Hero.Instance;
        _touchManager = TouchManager.Instance;
        _cameraBehaviour = CameraBehaviour.Instance;
        _containerImage = _instructionsContainer.GetComponent<Image>();
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
            var dist = Mathf.Sqrt((_hero.transform.position - _instructionsContainer.transform.position).magnitude) +.01f;
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
        if (_tutorialLauncher == null && !_started && collision.CompareTag("hero") && _hero.Stickiness.Attached)
        {
            _tutorialLauncher = StartCoroutine(LaunchTutorial());
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
            _initialDistanceToInstruction = Mathf.Sqrt((_hero.transform.position - _instructionsContainer.transform.position).magnitude);
            _hero.SetJumpingActivation(true);
            _hero.SetWalkingActivation(true);
            _clickText.SetActive(false);
        }

    }

    private void SetInstruction()
    {
        var pos = _hero.transform.position + Vector3.down;
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        _instructionText.text = _instructions.Peek();
    }

    private bool CheckComplete(ref int index, ref float duration)
    {
        if (duration <= 0)
            return true;


        if (index == 0) // Déplacement
        {
            if (_touchManager.Touching)
            {
                duration -= Time.deltaTime;
            }
        }
        else if (index == 1) // Saut
        {
            if (_touchManager.Dragging && duration > 1)
            {
                duration = 1;
            }

            if (!_touchManager.Touching && duration == 1)
            {
                duration = 0;
            }

        }
        else if (index == 2) // Saut vers l'autre plateforme
        {
            _itemIndex = 1;
            if (_hero.Stickiness.Attached && _hero.Stickiness.CurrentAttachment == _itemsToAppear[_itemIndex - 1].transform)
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
        else if (index == 4) // Monter tout en haut
        {
            _itemIndex = 5;
            if (_hero.Triggered && _hero.Stickiness.CurrentAttachment == _itemsToAppear[_itemIndex - 3].transform)
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
        _tutorialLauncher = null;
    }

    private void InitTutorial(int tutorialId)
    {
        if (tutorialId >= _tutorials.Count)
            return;

        _containerImage.color = Color.white;
        _hero.SetJumpingActivation(false);
        _hero.SetWalkingActivation(false);
        _instructionsContainer.SetActive(true);
        _clickText.SetActive(true);
        _actionDuration = _tutorials[tutorialId].Duration;
        _instructions = new Queue<string>(_tutorials[tutorialId].Instructions);
        SetInstruction();
    }

}