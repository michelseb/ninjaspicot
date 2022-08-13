namespace ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives
{
    public class StartingPoint : CheckPoint
    {
        public override string ParentName => $"{GetType().BaseType.Name}s";
    }
}