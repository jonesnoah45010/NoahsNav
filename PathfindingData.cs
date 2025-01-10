using UnityEngine;

public class PathfindingData
{
    public Vector2Int StartPosition { get; set; }
    public Vector2Int EndPosition { get; set; }
    public bool[,] WalkableGrid { get; set; } // A simplified representation of walkable tiles
}