<<<<<<< HEAD:Ninjaspicot/Assets/Scripts/Ninja/Trajectoire.cs
<<<<<<< HEAD:Ninjaspicot/Assets/Scripts/Ninja/Trajectoire.cs
﻿using System.Collections;
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
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectoire : MonoBehaviour {

    public GameObject ninja;
    Camera c;
    LineRenderer line;
    private bool disappeared;
    private int verts, vertsMax, vertsMaxConst = 80;

    private void Awake()
    {
        c = GetComponent<Camera>();
        line = this.GetComponent<LineRenderer>();
    }

    private void Start()
    {
        vertsMax = vertsMaxConst;
    }

    public void DrawTraject(Vector2 startPos, Vector2 startVelocity, Vector2 click, float speed)
    {

        Appear();
        if (disappeared)
        {
            StopCoroutine(FadeAway());
            disappeared = false;
            Debug.Log("appear");
        }
        
        if (verts < vertsMax + 2)
        {
            verts++;
        }
        if (verts > vertsMax - 1)
        {
            verts = vertsMax;
        }
        vertsMax = vertsMaxConst;
        line.SetVertexCount(verts);

        Vector2 grav = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y);
        Vector2 pos = startPos;
        Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(click.x, click.y, 0));
        Vector2 strength = pos - clickToWorld;
        Vector2 vel = strength.normalized * speed;
        
        
        for (var i = 0; i < verts; i++)
        {
            vel = vel + grav * Time.fixedDeltaTime;
            pos = pos + (vel * Time.fixedDeltaTime);
            line.SetPosition(i, new Vector3(pos.x, pos.y, 0));
            if (i > 1)
            {
                RaycastHit2D hit = Physics2D.CircleCast(line.GetPosition(i - 1), 1, line.GetPosition(i-2) - line.GetPosition(i-1), .1f);
                
                if (hit)
                {
                    if (hit.collider.gameObject.tag == "Wall")
                    {
                        vertsMax = i;
                    }
                    
                }
            }
        }
    }

    public IEnumerator FadeAway()
    {
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
    }

    public void ClearTraject()
    {
        line.SetVertexCount(0);
    }
}
>>>>>>> 0ef3c2388dae75248a06e0197436361176b4abb9:Trajectoire.cs
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectoire : MonoBehaviour {

    public GameObject ninja;
    Camera c;
    LineRenderer line;
    private bool disappeared;
    private int verts, vertsMax, vertsMaxConst = 80;
    TimeManager t;

    private void Awake()
    {
        c = GetComponent<Camera>();
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
            Debug.Log("appear");
        }

        if (verts < vertsMax + 3)
        {
            verts+=2;
        }
        if (verts > vertsMax - 1)
        {
            verts = vertsMax;
        }
        vertsMax = vertsMaxConst;
        line.SetVertexCount(verts);

        for (var i = 0; i < verts; i++)
        {
            vel = vel + grav * Time.fixedUnscaledDeltaTime;
            pos = pos + (vel * Time.fixedUnscaledDeltaTime);
            line.SetPosition(i, new Vector3(pos.x, pos.y, 0));
            if (i > 1)
            {
                RaycastHit2D hit = Physics2D.CircleCast(line.GetPosition(i - 1), 1, line.GetPosition(i-2) - line.GetPosition(i-1), .1f);
                
                if (hit)
                {
                    if (hit.collider.gameObject.tag == "Wall")
                    {
                        vertsMax = i;
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
        t.SlowDown();
    }

    public void ClearTraject()
    {
        line.SetVertexCount(0);
    }
}
>>>>>>> origin/master:Trajectoire.cs
