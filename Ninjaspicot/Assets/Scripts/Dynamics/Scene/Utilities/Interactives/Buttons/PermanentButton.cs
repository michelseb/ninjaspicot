namespace ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives.Buttons
{
    public class PermanentButton : ActivationButton
    {
        protected override void ToggleActivation(bool active)
        {
            if (!_active)
            {
                _audioService.PlaySound(_audioSource, "Bip", .3f);
            }
            _active = true;
            SetActive(_active);
        }
    }
}