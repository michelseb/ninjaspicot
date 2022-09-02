﻿namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface ISeeable : IDynamic
    {
        /// <summary>
        /// Is seeable hiding or is it possible to see it
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// Amount of lights that reveal ISeeable
        /// </summary>
        int RevealerCount { get; }

        /// <summary>
        /// Hides seeable (visible = false)
        /// </summary>
        void Hide();

        /// <summary>
        /// Reveals seeable (visible = true)
        /// </summary>
        void Reveal();
    }
}