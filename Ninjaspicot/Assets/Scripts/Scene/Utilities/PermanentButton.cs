public class PermanentButton : ActivationButton
{
    protected override void ToggleActivation(bool active)
    {
        if (!_active)
        {
            _audioManager.PlaySound(_audioSource, "Bip", .3f);
        }
        _active = true;
        SetActive(_active);
    }
}
