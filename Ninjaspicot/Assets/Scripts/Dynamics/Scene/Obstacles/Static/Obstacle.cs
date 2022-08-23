using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Obstacles
{
    public class Obstacle : SceneryElement, IRaycastable//, IActivable
    {
        private Collider2D _collider;
        public Collider2D Collider => _collider;

        protected virtual void Awake()
        {
            if (!TryGetComponent(out _collider))
            {
                _collider = GetComponentInChildren<Collider2D>() ?? GetComponentInParent<Collider2D>();
            }
            if (!BaseUtils.IsNull(Collider) && (Collider is PolygonCollider2D || Collider is BoxCollider2D))
            {
                Collider.usedByComposite = true;
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out IPhysic physic))
            {
                physic.LandOn(this, collision.contacts[0].point);
            }
        }

        //public virtual void Activate()
        //{
        //    var heroCollider = Hero.Instance?.Stickiness?.Collider;
        //    if (heroCollider != null)
        //    {
        //        Physics2D.IgnoreCollision(Collider, heroCollider, false);
        //    }
        //}

        //public virtual void Deactivate()
        //{
        //    var heroCollider = Hero.Instance?.Stickiness?.Collider;
        //    if (heroCollider != null)
        //    {
        //        Physics2D.IgnoreCollision(Collider, heroCollider);
        //    }
        //}

        //public void LaunchQuickDeactivate()
        //{
        //    StartCoroutine(QuickDeactivate());
        //}

        //private IEnumerator QuickDeactivate()
        //{
        //    Deactivate();
        //    yield return new WaitForSeconds(.1f);
        //    Activate();
        //}
    }
}