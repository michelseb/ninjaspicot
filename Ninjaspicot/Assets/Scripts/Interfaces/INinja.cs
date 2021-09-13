public interface INinja
{
    Jumper Jumper { get; }
    Stickiness Stickiness { get; }
    void SetJumpingActivation(bool active);
    void SetStickinessActivation(bool active);
    void SetWalkingActivation(bool active, bool grounded);
    void SetAllBehavioursActivation(bool active, bool grounded);
    bool NeedsToWalk();
}
