using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies
{
    public interface ISmart : IDynamic
    {
        void Sleep();
        void Patrol();
        //MovementType GetMovementType();
        //void MoveTo(Vector3 target);
        //void RotateTo(Vector3 target);
        //void LaunchMovement(MovementType movementType);
    }
}