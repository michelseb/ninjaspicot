using System.Collections;
using UnityEngine;

public class RandomMoves : MonoBehaviour {

    private Rigidbody2D _rigidBody;

    private void Start () {
        _rigidBody = GetComponent<Rigidbody2D>();
        StartCoroutine(Move());
    }
	
    private IEnumerator Move()
    {
        _rigidBody.AddForce(new Vector2(Random.Range(-10000, 10000), Random.Range(-10000, 10000)), ForceMode2D.Impulse);
        yield return new WaitForSeconds(2);
        StartCoroutine(Move());
    }
}
