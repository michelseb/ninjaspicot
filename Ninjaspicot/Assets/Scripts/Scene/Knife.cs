using UnityEngine;

public class Knife : MonoBehaviour {

    public bool touched;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Obstacle>() != null)
        {
            touched = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Obstacle>() != null)
        {
            touched = false;
        }
    }
}
