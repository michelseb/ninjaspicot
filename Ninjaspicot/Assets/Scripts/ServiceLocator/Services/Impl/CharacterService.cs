using UnityEngine;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class CharacterService : ICharacterService
    {
        public virtual GameObject ServiceObject { get; protected set; }

        public int Money { get; private set; }
        public void Init(Transform parent) 
        {
            ServiceObject = new GameObject(nameof(CharacterService));
            ServiceObject.transform.SetParent(parent);
        }

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