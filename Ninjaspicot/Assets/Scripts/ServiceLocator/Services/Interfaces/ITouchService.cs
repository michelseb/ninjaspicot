using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Inputs;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ITouchService : IPoolService<Joystick>
    {
        /// <summary>
        /// Object being controlled
        /// </summary>
        IControllable CurrentControllable { get; }
        
        /// <summary>
        /// Is left side of the screen being touched
        /// </summary>
        bool LeftSideTouching { get; }

        /// <summary>
        /// Is right side of the screen being touched
        /// </summary>
        bool RightSideTouching { get; }
        
        /// <summary>
        /// Left side of the screen being touched and not yet initialized
        /// </summary>
        bool LeftSideTouchStarting { get; }

        /// <summary>
        /// Left side of the screen not being touched anymore and action not yet ended
        /// </summary>
        bool LeftSideTouchEnding { get; }

        /// <summary>
        /// Is screen being touched
        /// </summary>
        bool Touching { get; }

        /// <summary>
        /// Scrolling from the left side of the screen
        /// </summary>
        bool LeftSideTouchDragging { get; }

        /// <summary>
        /// Get direction of left drag
        /// </summary>
        Vector3 LeftDragDirection { get; }

        /// <summary>
        /// Scrolling from the right side of the screen
        /// </summary>
        bool RightSideTouchDragging { get; }

        /// <summary>
        /// Scrolling from the right side of the screen and not yet initialized
        /// </summary>
        bool RightSideTouchStarting { get; }

        /// <summary>
        /// Scroll from right side of the screen ended and action not yet ended
        /// </summary>
        bool RightSideTouchEnding { get; }

        /// <summary>
        /// Right and left side of the screen being both touched
        /// </summary>
        bool DoubleTouching { get; }

        /// <summary>
        /// Set current controllable
        /// </summary>
        /// <param name="controllable"></param>
        void SetControllable(IControllable controllable);
    }
}