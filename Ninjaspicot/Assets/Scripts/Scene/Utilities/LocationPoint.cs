using System.Collections.Generic;
using UnityEngine;

public class LocationPoint : MonoBehaviour
{  
    public int Id { get; private set; }
    public int ColliderId { get; private set; }

    //private List<LocationPoint> _neighbours;

    public void Init(int id, int colliderId)
    {
        Id = id;
        ColliderId = colliderId;
        //_neighbours = new List<LocationPoint>();
    }

    //public void AddNeighbour(LocationPoint neighbour)
    //{
    //    _neighbours.Add(neighbour);
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hero") || collision.CompareTag("Enemy"))
        {
            var stickiness = collision.GetComponent<Stickiness>();
            
            if (Utils.IsNull(stickiness))
                return;

            //stickiness.LocationPoint = this;
        }
    }
}
