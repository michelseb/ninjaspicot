using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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
    private float _duration;
    private int _index;
    private Queue<string> _instructions;

    private Hero _hero;
    private TouchManager _touchManager;
    private CameraBehaviour _cameraBehaviour;

    private bool _started;

    private void Awake()
    {
        _hero = Hero.Instance;
        _touchManager = TouchManager.Instance;
        _cameraBehaviour = CameraBehaviour.Instance;
    }

    private void Update()
    {
        if (!_started)
            return;

        if (_instructions.Count() > 1)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                SetNextInstruction();
            }
        }
        else if(CheckComplete(ref _index, ref _duration))
        {
            _index++;
            LaunchTutorial(_index);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_started && collision.CompareTag("hero") && _hero.Stickiness.Attached)
        {
            LaunchTutorial(0);
            _started = true;
        }
    }

    private void SetNextInstruction()
    {
        if (_instructions.Count == 0)
            return;

        _instructions.Dequeue();
        SetInstruction();

        if (_instructions.Count == 1)
        {
            _hero.SetMovementActivation(true);
            _clickText.SetActive(false);
        }

    }

    private void SetInstruction()
    {
        var pos = _cameraBehaviour.Camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 3));
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        _instructionText.text = _instructions.Peek();
    }

    private bool CheckComplete(ref int index, ref float duration)
    {
        if (duration <= 0)
            return true;
            

        if (index == 0)
        {
            if (_touchManager.Touching)
            {
                duration -= Time.deltaTime;
            }
        }
        else if (index == 1)
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
        else if (index == 2)
        {
            if (!_hero.Stickiness.Attached)
            {
                duration = 0;
            }
        }

        return false;
    }

    private void LaunchTutorial(int tutorialId)
    {
        if (tutorialId >= _tutorials.Count)
            return;

        _hero.SetMovementActivation(false);
        _instructionsContainer.SetActive(true);
        _clickText.SetActive(true);
        _duration = _tutorials[tutorialId].Duration;
        _instructions = new Queue<string>(_tutorials[tutorialId].Instructions);
        SetInstruction();
    }

}