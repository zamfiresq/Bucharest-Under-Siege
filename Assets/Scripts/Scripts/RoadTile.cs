using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Road Tile", menuName = "Tiles/RoadTile")]
public class RoadTile : Tile
{
    public bool allowNorth;
    public bool allowSouth;
    public bool allowEast;
    public bool allowWest;

    public bool AllowsDirection(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return allowNorth;
        if (direction == Vector2Int.down) return allowSouth;
        if (direction == Vector2Int.right) return allowEast;
        if (direction == Vector2Int.left) return allowWest;
        return false;
    }
}