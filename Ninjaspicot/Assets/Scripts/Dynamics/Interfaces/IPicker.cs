using System.Collections.Generic;
using System.Linq;
using ZepLink.RiceNinja.Dynamics.Characters.Components;

namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface IPicker : IDynamic
    {
        /// <summary>
        /// Is picker able to pick
        /// </summary>
        bool CanTake { get; }
    }
}