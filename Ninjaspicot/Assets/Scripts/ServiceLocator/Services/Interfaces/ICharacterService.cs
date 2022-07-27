namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ICharacterService : IGameService
    {
        /// <summary>
        /// Gain money
        /// </summary>
        /// <param name="value"></param>
        void Gain(int value);

        /// <summary>
        /// Spend money
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Spend(int value);
    }
}