using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.ServiceLocator.Services;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Bonuses
{
    public class Coin : Bonus
    {
        [SerializeField] private int _value;

        private ICharacterService _characterService;

        protected override void Awake()
        {
            base.Awake();
            _characterService = ServiceFinder.Get<ICharacterService>();
        }

        public override void TakeBy(IPicker picker)
        {
            _characterService.Gain(_value);
            _audioService.PlaySound(this, "Coin");

            base.TakeBy(picker);
        }
    }
}
