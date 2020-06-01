using UnityEngine;
using UnityEngine.UI;

public class MenuLevels : MonoBehaviour {

    ScrollRect s;
    ScenesManager sc;

    private void Start () {
        sc = FindObjectOfType<ScenesManager>();
        s = GetComponent<ScrollRect>();
    }

    public void GenerateMenuLevels()
    {
        
        for (int a = 0; a < sc.GetNumberOfLevels(); a++)
        {
            
        }
    }
}
