public interface INinja
{
    Stickiness Stickiness { get; }
    void SetGrapplingActivation(bool active);
    void SetStickinessActivation(bool active);
    void SetWalkingActivation(bool active, bool grounded);
    void SetAllBehavioursActivation(bool active, bool grounded);
    bool NeedsToWalk();
}
