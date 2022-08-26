namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies.Actions
{
    public abstract class EnemyAction
    {
        public EnemyAction() {}

        public abstract void Initialize();
        public abstract void Execute();
    }
}