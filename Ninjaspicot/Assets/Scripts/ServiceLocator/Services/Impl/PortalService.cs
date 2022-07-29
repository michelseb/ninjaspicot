using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Portals;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class PortalService : CoroutineService<int, Portal>, IPortalService
    {
        private IDictionary<int, int> _doorEntranceExitPairs = new Dictionary<int, int>();

        public bool Connecting { get; private set; }

        private readonly ICameraService _cameraService;
        private readonly IZoneService _zoneService;
        private readonly IScenesService _scenesService;



        public const int TRANSFER_SPEED = 3; //Seconds needed to go between 2 portals
        public const float EJECT_SPEED = 100; //How strongly transferred entity is ejected

        public PortalService(ICameraService cameraService, IZoneService zoneService, IScenesService scenesService)
        {
            _cameraService = cameraService;
            _zoneService = zoneService;
            _scenesService = scenesService;
        }

        private int? GetExitIndexByEntranceId(int entranceId)
        {
            return _doorEntranceExitPairs.ContainsKey(entranceId) ? _doorEntranceExitPairs[entranceId] : null;
        }

        public bool ConnectionExists(int portalId)
        {
            return GetExitIndexByEntranceId(portalId) != null;
        }

        public Portal GetPortalById(int portalId)
        {
            return Collection.FirstOrDefault(t => t.Id == portalId);
        }

        private IEnumerator CreateConnection(Portal entrance, int exitId)
        {
            Connecting = true;

            _scenesService.StartLoadingByPortalId(exitId);

            while (_scenesService.IsSceneLoading)
                yield return null;

            var exit = GetPortalById(exitId);

            if (exit == null)
            {
                TerminateConnection(entrance);
                yield break;
            }

            entrance.Entrance = true;
            exit.Exit = true;

            entrance.SetOtherPortal(exit);
            exit.SetOtherPortal(entrance);

            Connecting = false;
        }

        public void LaunchConnection(Portal entrance)
        {
            if (Connecting)
                return;

            var otherId = GetExitIndexByEntranceId(entrance.Id);

            if (otherId == null)
                return;

            CoroutineServiceBehaviour.StartCoroutine(CreateConnection(entrance, otherId.Value));
        }

        public void ClosePreviousScene(int entranceId)
        {
            var sceneId = int.Parse(entranceId.ToString().Substring(0, 2));

            //RemoveZonePortalsFromList(zoneId);
            _scenesService.Unload(sceneId);
        }

        public void TerminateConnection(Portal entrance, Portal exit = null)
        {
            entrance.Free();
            exit?.Free();
        }

        //private void RemoveZonePortalsFromList(int zoneId)
        //{
        //    Collection.Where(p => p.Id.ToString().Substring(0, 2) == zoneId.ToString()).ToList().ForEach(x => Collection.Remove(x));
        //}

        public void Teleport(ITeleportable teleportable, Portal entrance, Portal exit)
        {
            CoroutineServiceBehaviour.StartCoroutine(DoTeleport(teleportable, entrance, exit));
        }

        public IEnumerator DoTeleport(ITeleportable teleportable, Portal entrance, Portal exit)
        {
            _zoneService.SetZone(exit.Zone.Id);

            yield return new WaitForSecondsRealtime(1);

            _cameraService.MainCamera.MoveTo(teleportable.Transform.position);
            _cameraService.UiCamera.CameraAppear();
            entrance.Deactivate();
            ClosePreviousScene(entrance.Id);

            yield return new WaitForSecondsRealtime(2);

            teleportable.Appear();
            teleportable.Rigidbody.isKinematic = false;
            teleportable.Rigidbody.velocity = exit.transform.right * EJECT_SPEED;
            exit.Deactivate();
            TerminateConnection(entrance, exit);
        }
    }
}