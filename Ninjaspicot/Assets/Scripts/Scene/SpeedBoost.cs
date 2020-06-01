using UnityEngine;

public class SpeedBoost : MonoBehaviour {

    Movement d;

    private void Start()
    {
        d = FindObjectOfType<Movement>();
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            StartCoroutine(d.SpeedBoost(3));
        }
    }
}
