﻿using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ISpawnService : IGameService
    {
        /// <summary>
        /// Respawns spawnable at latest respawn position
        /// </summary>
        /// <param name="spawnable"></param>
        void SpawnAtLastSpawningPosition(ISpawnable spawnable);

        /// <summary>
        /// Respawns spawnable at beginning
        /// </summary>
        /// <param name="spawnable"></param>
        void SpawnAtBeginning(ISpawnable spawnable);

        /// <summary>
        /// Store spawn positions of current scene and sets current checkPoint
        /// </summary>
        /// <param name="checkPoint"></param>
        void InitActiveSceneSpawns(int checkPoint = 0);

        /// <summary>
        /// Sets spawn position to given checkpoint and modifiy checkpoint flag
        /// </summary>
        /// <param name="checkPoint"></param>
        void SetLatestSpawn(CheckPoint checkPoint);
    }
}