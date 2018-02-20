using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectoire : MonoBehaviour {

    public GameObject ninja;
    Camera c;
    CameraBehaviour cam;
    LineRenderer line;
    private bool disappeared;
    private int verts, vertsMax, vertsMaxConst = 80;
    TimeManager t;

    private void Awake()
    {
        c = GetComponent<Camera>();
        cam = FindObjectOfType<CameraBehaviour>();
        line = this.GetComponent<LineRenderer>();
        t = FindObjectOfType<TimeManager>();
    }

    private void Start()
    {
        vertsMax = vertsMaxConst;
    }

    public void DrawTraject(Vector2 startPos, Vector2 startVelocity, Vector2 click, Vector2 startClick, float speed)
    {
        Vector2 grav = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y);
        Vector2 pos = startPos;
        Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(click.x, click.y, 0));
        Vector2 startClickToWorld = c.ScreenToWorldPoint(new Vector3(startClick.x, startClick.y, 0));
        Vector2 strength = startClickToWorld - clickToWorld;
        Vector2 vel = strength.normalized * speed;

        Appear();
        if (disappeared)
        {
            StopCoroutine(FadeAway());
            disappeared = false;
        }

        if (verts < vertsMax + 3)
        {
            verts+=2;
        }
        if (verts > vertsMax - 2)
        {
            verts = vertsMax;
        }
        line.SetVertexCount(verts);
        vertsMax = vertsMaxConst;
        

        for (var i = 0; i < verts; i++)
        {
            vel = vel + grav * Time.fixedUnscaledDeltaTime;
            pos = pos + (vel * Time.fixedUnscaledDeltaTime);
            line.SetPosition(i, new Vector3(pos.x, pos.y, 0));
            if (i > 1)
            {
                RaycastHit2D hit = Physics2D.CircleCast(line.GetPosition(i - 1), 1, line.GetPosition(i-2) - line.GetPosition(i-1), .1f, LayerMask.GetMask("Default"));
                
                if (hit)
                {
                    if (hit.collider.gameObject.tag == "Wall")
                    {
                        vertsMax = i;
                        verts = i;
                        line.SetVertexCount(verts);
                        break;
                    }
                    
                }
                
            }

            
        }
    }

    public IEnumerator FadeAway()
    {
        StartCoroutine(t.RestoreTime());
        verts = 0;
        vertsMax = vertsMaxConst;
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
        t.StopTime();
        StartCoroutine(cam.zoomOut(60));
    }

    public void ClearTraject()
    {
        line.SetVertexCount(0);
    }
}