using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class CharacterService : GameService, ICharacterService
    {
        public int Money { get; private set; }

        public void Gain(int value)
        {
            Money += value;
        }

        public bool Spend(int value)
        {
            if (value > Money)
                return false;

            Money -= value;
            return true;
        }
    }
}