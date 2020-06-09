using UnityEngine;

[DisallowMultipleComponent]
public class Poppable : DynamicObstacle 
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hero"))
            return;

        Rigidbody.isKinematic = true;
    }
}
