using UnityEngine;

namespace ZepLink.RiceNinja.Managers
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {

#if UNITY_ANDROID
        Application.targetFrameRate = 80;

        QualitySettings.vSyncCount = 0;

        QualitySettings.antiAliasing = 0;

        QualitySettings.shadowCascades = 0;
        QualitySettings.shadowDistance = 15;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif

#if UNITY_STANDALONE_WIN
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            QualitySettings.antiAliasing = 0;
#endif
        }

    }
}