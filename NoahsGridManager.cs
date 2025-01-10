using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoahsGridManager : MonoBehaviour
{
    public int gridHeight = 100;
    public int gridWidth = 100;
    public GameObject tilePrefab;
    public GameObject obstacles;

    public bool grid_is_visable = true;

    public Material instanceShowPathMaterial; // Assign this in the Unity editor
    public Material instanceWalkableMaterial; // Assign this in the Unity editor

    public static Material show_path_material;
    public static Material walkable_material;

    public static GameObject[,] grid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        show_path_material = instanceShowPathMaterial;
        walkable_material = instanceWalkableMaterial;
        grid = NoahsPathFinder.GenerateNavGrid(this.gameObject.transform.position, gridHeight, gridWidth, tilePrefab, obstacles);
        if (grid_is_visable)
        {
            ShowGrid();
        }
        else
        {
            HideGrid();
        }
    }


    public static void HideGrid()
    {
        if (grid == null) return;

        foreach (GameObject tile in grid)
        {
            if (tile != null)
            {
                MeshRenderer renderer = tile.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }
            }
        }
    }

    public static void ShowGrid()
    {
        if (grid == null) return;

        foreach (GameObject tile in grid)
        {
            if (tile != null)
            {
                MeshRenderer renderer = tile.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.enabled = true;
                }
            }
        }
    }




    public static void ShowPath(List<GameObject> path)
    {
        if(path != null && path.Count > 0)
        {
            foreach (GameObject tile in path)
            {
                // Ensure the tile has a Renderer component
                MeshRenderer tileRenderer = tile.GetComponent<MeshRenderer>();
                if (tileRenderer != null)
                {
                    // Apply the show_path_material to the tile's material
                    tileRenderer.material = show_path_material;
                }
            }
        }
        
    }

  

    public static void RevertShowPath()
    {
        if (grid != null)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    GameObject tile = grid[x, y];
                    if (tile != null && tile.GetComponent<NoahsTile>().is_walkable == true)
                    {
                        // Ensure the tile has a Renderer component
                        MeshRenderer tileRenderer = tile.GetComponent<MeshRenderer>();
                        if (tileRenderer != null)
                        {
                            // Apply the walkable_material to the tile's material
                            tileRenderer.material = walkable_material;
                        }
                    }
                }
            }
        }
    }






}
