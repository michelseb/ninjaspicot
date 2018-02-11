using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectoire : MonoBehaviour {

    public GameObject ninja;
    Camera c;
    LineRenderer line;
    private bool disappeared;

    private void Awake()
    {
        c = GetComponent<Camera>();
        line = this.GetComponent<LineRenderer>();
    }


    public void DrawTraject(Vector2 startPos, Vector2 startVelocity, Vector2 impulse, float speed)
    {

        Appear();
        if (disappeared)
        {
            StopCoroutine(FadeAway());
            disappeared = false;
            Debug.Log("appear");
        }
        int verts = 80;
        
        line.SetVertexCount(verts);

        Vector2 pos = startPos;
        Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(impulse.x, impulse.y, 0));
        Vector2 strength = pos - clickToWorld;
        Vector2 grav = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y);
        Vector2 vel = strength.normalized * speed;
        
        
        for (var i = 0; i < verts; i++)
        {
            line.SetPosition(i, new Vector3(pos.x, pos.y, 0));
            vel = vel + grav * Time.fixedDeltaTime;
            pos = pos + vel * Time.fixedDeltaTime;
        }

    }

    public IEnumerator FadeAway()
    {
        disappeared = true;
        Color col = line.material.color;
        while (col.a > 0)
        {
            col = line.material.color;
            col.a -= 0.01f;
            line.material.color = col;
            yield return null;
        }
        
    }

    private void Appear()
    {
        Color col = line.material.color;
        col.a = 1f;
        line.material.color = col;
    }

    public void ClearTraject()
    {
        line.SetVertexCount(0);
    }
}
