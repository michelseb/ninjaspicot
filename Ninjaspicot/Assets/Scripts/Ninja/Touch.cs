using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour {

    public int segments;
    public float xradius;
    public float yradius;
    LineRenderer line;
    Camera c;

    void Start()
    {
        c = GetComponent<Camera>();
        line = GetComponent<LineRenderer>();

        line.SetVertexCount(0);
        line.useWorldSpace = false;

    }


    public IEnumerator CreatePoints(Vector3 mousePos)
    {

        
        Vector3 pos = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        pos -= transform.position; 
        float x;
        float y;
        float z = 5f;
        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            
            line.SetVertexCount(i+1);
            x = pos.x + Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            y = pos.y + Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            line.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / segments);
            yield return new WaitForSeconds(.001f);
        }
    }

    public void Erase()
    {
        line.SetVertexCount(0);
    }
}
