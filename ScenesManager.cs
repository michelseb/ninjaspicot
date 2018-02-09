using System.Collections;
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
