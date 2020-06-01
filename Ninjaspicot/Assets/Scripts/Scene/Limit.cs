using UnityEngine;

public class Limit : MonoBehaviour {

    private Hero _hero;

    private void Awake()
    {
        _hero = Hero.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Ninjaspicot")
        {
            _hero.Die(null);
        }
        
    }
}
