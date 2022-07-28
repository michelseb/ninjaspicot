using System.Collections.Generic;
using UnityEngine;
using ZepLink.RiceNinja.Manageables;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IScriptableObjectService<T> : ICollectionService<T> where T : ScriptableObject, IManageable
    {   
        string ObjectsPath { get; }
    }
}