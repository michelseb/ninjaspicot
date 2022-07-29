﻿using UnityEngine;
using ZepLink.RiceNinja.Manageables;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IScriptableObjectService<T, U> : ICollectionService<T, U> where U : ScriptableObject, IManageable<T>
    {   
        string ObjectsPath { get; }
    }
}