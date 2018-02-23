using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Ghost : MonoBehaviour {

    List<Vector2> oldPos;
    List<Vector2> positions;
    public GameObject ghost, instantiatedGhost;
    //Ninja n;
    float highest;
    int step;
    TextAsset ghostPos;
    // Use this for initialization
    void Start() {
        //n = 
        oldPos = new List<Vector2>();
        positions = new List<Vector2>();
        ghostPos = Resources.Load<TextAsset>(@"Data/ghost");
        loadAll();
        if (oldPos.Count > 0)
        {
            Debug.Log("Ghost");
            instantiatedGhost = Instantiate(ghost, transform.position, Quaternion.identity);
        }
        step = 0;

        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        positions.Add(transform.position);

        if (instantiatedGhost != null)
        {
            if (oldPos.Count > step)
            {
                instantiatedGhost.transform.position = oldPos[step];
                step++;
            }
            
        }
    }

    public void saveAll(float highest)
    {
        if (highest > this.highest)
        {
            StreamWriter sw = new StreamWriter(@"Assets/Resources/Data/ghost.txt");
            Debug.Log(sw);
            sw.WriteLine("" + highest);
            foreach (Vector3 p in positions)
            {
                sw.WriteLine("" + p.x);
                sw.WriteLine("" + p.y);
            }
            sw.Close();
        }
        
    }

    void loadAll()
    {
        if (ghostPos != null)
        {
            Debug.Log("Loading");
            using (StreamReader sr = new StreamReader(new MemoryStream(ghostPos.bytes)))
            {
                highest = float.Parse(sr.ReadLine());
                while (!sr.EndOfStream)
                {
                    float x = float.Parse(sr.ReadLine());
                    float y = float.Parse(sr.ReadLine());
                    oldPos.Add(new Vector2(x, y));
                }
            }
        }
    }

}
