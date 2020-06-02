using UnityEngine;

public class Perimeter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hero"))
        {
            collision.GetComponent<Hero>().Die(transform);
        }
    }
}
