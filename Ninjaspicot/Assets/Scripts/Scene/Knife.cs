using UnityEngine;

public class Knife : MonoBehaviour
{
    public bool Touched { get; private set; }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Obstacle>() != null)
        {
            Touched = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Obstacle>() != null)
        {
            Touched = false;
        }
    }
}
