namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies.Actions
{
    public abstract class MovementAction : EnemyAction
    {
        protected readonly float _speed;

        public MovementAction(float speed) : base()
        {
            _speed = speed;
        }
    }
}