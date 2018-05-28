using System.Collections;
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
    public static bool alreadyPlayed;
    private int numberOfLevels;
    public int startScene;
    List<Scene> sceneList;
    private static int currentScene = 0, checkpoint = 0;


    private void Awake()
    {
        
        DontDestroyOnLoad(gameObject);
        numberOfLevels = SceneManager.sceneCountInBuildSettings - 2;
        if (startScene == 0)
        {
            BackToBeginning();
        }else
        {
            currentScene = startScene;
            ResetScene();
        }
    }

    public static void LevelSelectMenu()
    {

        SceneManager.LoadScene("LevelMenu");
    }

    public static void ResetScene()
    {
        alreadyPlayed = true;
        SceneManager.LoadScene("level" + currentScene);
    }

    public static void BackToBeginning()
    {
        currentScene = -1;
        SceneManager.LoadScene("intro");

    }
    public static void BackToCheckpoint()
    {
        alreadyPlayed = true;
        currentScene = checkpoint;
        SceneManager.LoadScene("level" + currentScene);
    }
    public static void NextScene()
    {
        alreadyPlayed = false;
        currentScene++;
        SceneManager.LoadScene("level" + currentScene);
    }
    public static void SetCheckpoint()
    {
        checkpoint = currentScene;
    }

    public int GetNumberOfLevels()
    {
        return numberOfLevels;
    }



}
