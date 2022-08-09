using UnityEngine;

namespace ZepLink.RiceNinja.Interfaces
{
    // Interface to make objects sleep outside of zones
    public interface IWakeable
    {
        void Sleep()
        {
            if (this is MonoBehaviour m)
            {
                m.gameObject.SetActive(false);
            }
        }

        void Wake()
        {
            if (this is MonoBehaviour m)
            {
                m.gameObject.SetActive(true);
            }
        }
    }
}