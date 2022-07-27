using UnityEngine;

namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface IControllable : IDynamic
    {
        #region LeftTouch
        void OnLeftSideTouchInit();

        void OnLeftSideTouch();

        void OnLeftSideDrag();

        void OnLeftSideTouchEnd();
        #endregion

        #region RightTouch
        void OnRightSideTouchInit();

        void OnRightSideTouch();

        void OnRightSideDrag(Vector2 direction);

        void OnRightSideDragEnd(Vector2 direction);
        #endregion

        #region DoubleTouch
        void OnDoubleTouchRightSideDrag(Vector2 direction);

        void OnDoubleTouchLeftSideDrag(Vector2 direction);

        void OnDoubleTouchRightSideDragEnd(Vector2 direction);
        #endregion
    }
}