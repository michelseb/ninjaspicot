using ZepLink.RiceNinja.Dynamics.Characters.Components;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class TrajectoryService : PoolService<Trajectory>, ITrajectoryService
    {
        public override string Name => "TrajectoryService";

        public TrajectoryService() { }
    }
}