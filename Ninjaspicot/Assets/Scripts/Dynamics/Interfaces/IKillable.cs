using System.Collections;
using UnityEngine;
using ZepLink.RiceNinja.Manageables.Audios;

namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface IKillable : IDynamic
    {
        bool Dead { get; }
        void Die(Transform killer = null, AudioFile sound = null, float volume = 1f);
        IEnumerator Dying();
    }
}