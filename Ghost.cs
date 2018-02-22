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
    // Use this for initialization
    void Start() {
        //n = 
        oldPos = new List<Vector2>();
        positions = new List<Vector2>();
        loadAll();
        if (oldPos.Count > 0)
        {
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
            instantiatedGhost.transform.position = oldPos[step];
            step++;
        }
    }

    public void saveAll(float highest)
    {
        if (highest > this.highest)
        {
            if (!Directory.Exists(@"C:\tmp"))
            {
                Directory.CreateDirectory(@"C:\tmp");
            }
            StreamWriter sw = new StreamWriter(@"C:\tmp\ghost.txt");
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
        if (File.Exists(@"C:\tmp\ghost.txt"))
        {
            StreamReader sr = new StreamReader(@"C:\tmp\ghost.txt");
            highest = float.Parse(sr.ReadLine());
            int i = 0;
            while (!sr.EndOfStream)
            {
                float x = float.Parse(sr.ReadLine());
                float y = float.Parse(sr.ReadLine());
                oldPos.Add(new Vector2(x, y));
                i++;
            }
        }
    }

}
