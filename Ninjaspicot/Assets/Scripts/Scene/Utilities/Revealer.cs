public class Revealer : Lamp, IRevealer
{
    private Hero _hero;
    public Hero Hero { get { if (_hero == null) _hero = Hero.Instance; return _hero; } }

    private VisibilityManager _visibilityManager;
    public VisibilityManager VisibilityManager { get { if (Utils.IsNull(_visibilityManager)) _visibilityManager = VisibilityManager.Instance; return _visibilityManager; } }
    public float ActivationDistance => 40f;
}