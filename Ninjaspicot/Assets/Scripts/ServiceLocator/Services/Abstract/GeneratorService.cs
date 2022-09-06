using UnityEngine;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Abstract
{
    public abstract class GeneratorService : GameService
    {
        protected abstract void GenerateAt(PoolSetting setting, Color color, Transform zone = default);

        protected Vector3Int PixelToCoord(int index, int imgWidth)
        {
            var x = index % imgWidth;
            var y = index / imgWidth;

            return new Vector3Int(x, y);
        }

        public class PoolSetting
        {
            public Vector3Int Position { get; private set; }
            public Quaternion Rotation { get; private set; }

            public PoolSetting(Vector3Int position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }
    }
}
