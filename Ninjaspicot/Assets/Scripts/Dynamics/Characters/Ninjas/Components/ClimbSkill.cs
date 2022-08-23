using System.Collections;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.Components.Skills;
using ZepLink.RiceNinja.Dynamics.Scenery.Obstacles;

namespace ZepLink.RiceNinja.Dynamics.Characters.Ninjas.Components
{
    public class ClimbSkill : SkillBase
    {
        [SerializeField] protected float _speed;

        protected Collider2D _collider;

        public HingeJoint2D WallJoint { get; set; }
        public Obstacle CurrentAttachment { get; set; }
        public int FramesSinceDetached { get; private set; }
        public bool AttachDeactivated => FramesSinceDetached < 2;
        public bool Attached => WallJoint?.enabled == true;
        public bool CanWalk { get; set; }
        public bool Walking => _walkOnWalls != null;
        public float CurrentSpeed { get; set; }
        public float Direction => Mathf.Sign(_speedFactor);
        public Transform ContactPoint { get; private set; }
        public Collider2D Collider { get { if (_collider == null) _collider = GetComponent<Collider2D>(); return _collider; } }
        
        //TODO => move to hero class (should not be in skill properties)
        public Vector3 CollisionNormal { get; private set; }
        public float ImpactVelocity { get; private set; }
        public bool Running { get; protected set; }

        protected float _speedFactor;
        protected Vector3 _previousContactPoint;
        protected Coroutine _walkOnWalls;
        private float _velocityBeforePhysicsUpdate;

        public virtual void Awake()
        {
            if (Active)
                return;

            Active = true;
            WallJoint = GetComponent<HingeJoint2D>();
            _collider = _collider ?? GetComponent<Collider2D>();
            ContactPoint = new GameObject("ContactPoint").transform;
            ContactPoint.position = Transform.position;
            ContactPoint.SetParent(Transform);
            _previousContactPoint = ContactPoint.position;
        }

        public virtual void Start()
        {
            if (CanWalk)
                return;

            CanWalk = true;
            ReinitSpeed();
        }

        private void FixedUpdate()
        {
            _velocityBeforePhysicsUpdate = Rigidbody.velocity.magnitude;
        }

        private void Update()
        {
            FramesSinceDetached++;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            ImpactVelocity = _velocityBeforePhysicsUpdate;
            OnCollisionStay2D(collision);
        }

        public void OnCollisionStay2D(Collision2D collision)
        {
            var contact = GetContactPoint(collision.contacts, _previousContactPoint);
            SetContactPosition(contact.point);
            CollisionNormal = contact.normal;
        }

        public virtual bool Attach(Obstacle obstacle)
        {
            if (Attached || AttachDeactivated)
                return false;

            var anchorPos = Transform.InverseTransformPoint(GetContactPosition());

            WallJoint.enabled = true;
            WallJoint.useMotor = false;
            WallJoint.anchor = anchorPos;
            WallJoint.connectedAnchor = WallJoint.anchor;

            Rigidbody.gravityScale = 0;
            Rigidbody.velocity = Vector2.zero;

            return true;
        }

        public virtual void Detach()
        {
            if (!Attached)
                return;

            FramesSinceDetached = 0;
            WallJoint.enabled = false;
            CurrentAttachment = null;
            Rigidbody.isKinematic = false;
            Rigidbody.gravityScale = 1;
        }

        public ContactPoint2D GetContactPoint(ContactPoint2D[] contacts, Vector3 previousPos) //WOOOOHOOO ça marche !!!!!
        {
            return contacts.OrderBy(c => Vector3.Distance(previousPos, c.point)).Last();
        }

        public Vector3 GetContactPosition()
        {
            return ContactPoint.position;
        }

        public void SetContactPosition(Vector3 position)
        {
            _previousContactPoint = ContactPoint.position;
            ContactPoint.position = position;
        }

        public virtual void StopWalking(bool stayGrounded)
        {
            if (_walkOnWalls != null)
            {
                StopCoroutine(_walkOnWalls);
                _walkOnWalls = null;
            }

            if (!Attached)
                return;

            _speedFactor = 0;
            Rigidbody.velocity = Vector2.zero;
            Rigidbody.angularVelocity = 0;
            WallJoint.useMotor = false;
            Rigidbody.isKinematic = stayGrounded;

        }

        public virtual void StartWalking()
        {
            if (_walkOnWalls != null || !Active)
                return;

            Rigidbody.isKinematic = false;
            _walkOnWalls = StartCoroutine(WalkOnWalls(WallJoint));
        }

        protected virtual IEnumerator WalkOnWalls(HingeJoint2D hinge)
        {
            if (hinge == null)
                yield break;

            var jointMotor = hinge.motor;
            hinge.useMotor = true;

            while (true)
            {
                jointMotor.motorSpeed = CurrentSpeed;
                hinge.motor = jointMotor;
                hinge.anchor = Transform.InverseTransformPoint(GetContactPosition());

                yield return null;
            }
        }

        public void StartRunning()
        {
            Running = true;
            CurrentSpeed = _speed * 2.5f;
        }

        public void ReinitSpeed()
        {
            Running = false;
            CurrentSpeed = _speed;
        }

        public override void LandOn(Obstacle obstacle, Vector3 contactPoint)
        {
            if (!Active || obstacle == CurrentAttachment)
                return;

            if (!Attach(obstacle))
                return;

            CurrentAttachment = obstacle;
            CurrentAttachment = obstacle;
            SetContactPosition(contactPoint);
        }
    }
}