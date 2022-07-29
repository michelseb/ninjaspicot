﻿using UnityEngine;

namespace ZepLink.RiceNinja.Manageables
{
    public abstract class ColorManageable : Manageable<Color>
    {
        public override Color Id { get; } 
    }
}
