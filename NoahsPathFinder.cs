using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoahsPathFinder : MonoBehaviour
{

    public static GameObject[,] GenerateNavGrid(Vector3 gridCenter, int gridHeight, int gridWidth, GameObject tilePrefab, GameObject obstacles)
    {
        GameObject[,] grid = new GameObject[gridWidth, gridHeight];

        // Calculate the center offsets for the grid
        float offsetX = (gridWidth - 1) / 2f;
        float offsetY = (gridHeight - 1) / 2f;

        Vector3 startPosition = gridCenter;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Calculate position relative to gridStart's position
                Vector3 position = new Vector3(
                    startPosition.x + (x - offsetX),
                    startPosition.y,
                    startPosition.z + (y - offsetY)
                );

                // Instantiate tile prefab and set up grid
                GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity);
                grid[x, y] = tileObject;
                tileObject.name = "Tile_" + x + "_" + y;

                NoahsTile tile = tileObject.GetComponent<NoahsTile>();

                // Check for intersections with obstacles
                if (IsIntersectingWithObstacles(tileObject, obstacles))
                {
                    tile.is_walkable = false;
                    tile.makeWall();
                }
            }
        }
        return grid;
    }



    private static bool IsIntersectingWithObstacles(GameObject tile, GameObject obstacles)
    {
        // Get the MeshRenderer bounds of the tile
        MeshRenderer tileRenderer = tile.GetComponentInChildren<MeshRenderer>();

        if (tileRenderer == null)
        {
            return false;
        }

        Bounds tileBounds = tileRenderer.bounds;

        // Iterate through all child MeshRenderers of the obstacles object
        MeshRenderer[] obstacleRenderers = obstacles.GetComponentsInChildren<MeshRenderer>();

        if (obstacleRenderers.Length == 0)
        {
            return false;
        }

        foreach (MeshRenderer obstacleRenderer in obstacleRenderers)
        {
            Bounds obstacleBounds = obstacleRenderer.bounds;

            // Check if the tile's bounds intersect with the current obstacle's bounds
            if (tileBounds.Intersects(obstacleBounds))
            {
                return true;
            }
        }

        return false;
    }






}
