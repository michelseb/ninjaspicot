using UnityEngine;
using ZepLink.RiceNinja.Manageables;
using ZepLink.RiceNinja.Manageables.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IScriptableObjectService<T, U> : ICollectionService<T, U> where U : ScriptableObject, IManageable<T>
    {   
        string ObjectsPath { get; }
    }
}