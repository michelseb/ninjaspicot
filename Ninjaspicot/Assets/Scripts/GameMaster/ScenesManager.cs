<<<<<<< HEAD:Ninjaspicot/Assets/Scripts/GameMaster/ScenesManager.cs
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour {

    private static int currentScene = 0, checkpoint = 0;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ResetScene();
    }

    public static void ResetScene()
    {
        SceneManager.LoadScene("level" + currentScene);
    }

    public static void BackToBeginning()
    {
        currentScene = 1;
        SceneManager.LoadScene("level" + currentScene);
    }
    public static void BackToCheckpoint()
    {
        currentScene = checkpoint;
        SceneManager.LoadScene("level" + currentScene);
    }
    public static void NextScene()
    {
        currentScene++;
        SceneManager.LoadScene("level" + currentScene);
    }
    public void SetCheckpoint(int check)
    {
        checkpoint = check;
    }

}
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[System.Serializable]
public class Scene
{
    public int sceneIndex;
    public Sprite icon;
}


public class ScenesManager : MonoBehaviour {

    private int numberOfLevels;
    List<Scene> sceneList;
    private static int currentScene = 0, checkpoint = 0;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        numberOfLevels = SceneManager.sceneCountInBuildSettings - 2;
        BackToBeginning();
    }

    public static void LevelSelectMenu()
    {

        SceneManager.LoadScene("LevelMenu");
    }

    public static void ResetScene()
    {
        SceneManager.LoadScene("level" + currentScene);
    }

    public static void BackToBeginning()
    {
        currentScene = 0;
        SceneManager.LoadScene("level" + currentScene);
    }
    public static void BackToCheckpoint()
    {
        currentScene = checkpoint;
        SceneManager.LoadScene("level" + currentScene);
    }
    public static void NextScene()
    {
        currentScene++;
        SceneManager.LoadScene("level" + currentScene);
    }
    public void SetCheckpoint(int check)
    {
        checkpoint = check;
    }

    public int GetNumberOfLevels()
    {
        return numberOfLevels;
    }

}
>>>>>>> 0ef3c2388dae75248a06e0197436361176b4abb9:ScenesManager.cs
