using UnityEngine;

namespace ZepLink.RiceNinja.Manageables.Abstract
{
    public abstract class ColorManageable : Manageable<Color>
    {
        public override Color Id { get; } 
    }
}
