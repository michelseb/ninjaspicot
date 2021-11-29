using System.Collections;
using UnityEngine;

public interface IBreakable : IFocusable
{
    bool Broken { get; set; }
    bool Break(Transform killer = null, Audio sound = null, float volume = 1f);
}
