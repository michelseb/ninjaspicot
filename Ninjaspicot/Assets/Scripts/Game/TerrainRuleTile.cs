using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Terrain Rule Tile", menuName = "Zeplink/Tile")]
public class TerrainRuleTile : RuleTile<TerrainRuleTile.Neighbor>
{
    [SerializeField] private TileBase[] _tilesToConnect;

    public class Neighbor : TilingRuleOutput.Neighbor
    {
        public const int Any = 3;
        public const int Specified = 4;
        public const int Nothing = 5;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.This: return CheckThis(tile);
            case Neighbor.NotThis: return CheckNotThis(tile);
            case Neighbor.Any: return CheckAny(tile);
            case Neighbor.Specified: return CheckSpecified(tile);
            case Neighbor.Nothing: return CheckNothing(tile);
        }

        return base.RuleMatch(neighbor, tile);
    }

    private bool CheckThis(TileBase tile)
    {
        return tile == this;
    }

    private bool CheckNotThis(TileBase tile)
    {
        return tile != this;

    }

    private bool CheckAny(TileBase tile)
    {
        return tile != null;
    }

    private bool CheckSpecified(TileBase tile)
    {
        return _tilesToConnect.Contains(tile);
    }

    private bool CheckNothing(TileBase tile)
    {
        return tile == null;
    }
}