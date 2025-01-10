using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using NoahsNav;


public class NoahsAgent : MonoBehaviour
{
    public GameObject primary_target;
    private Vector3 primary_target_last_position;
    public float turn_speed = 5;
    public float move_speed = 5;

    public float sense_dist = 5;

    public float stop_at = 4;

    public float avoid_scaler = 10;


    private List<GameObject> current_path = new List<GameObject>();  // Path to the target
    public float TileDetection = 2f;

    public bool useNav=false;
    public float navChecksPerSec = 2;
    private float nav_increment;
    private float nav_timer = 0;

    private GameObject current_target;

    private GameObject[] flat_grid;
    private GameObject[,] grid;

    public bool show_path = false;

    public bool calculating_path = false;

    public float randomness = 0f;

    private bool primary_target_moved;
    private bool ok_to_show_path;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nav_increment = 1/navChecksPerSec;
        if (primary_target != null)
        {
            primary_target_last_position = primary_target.transform.position;
        }
    }


    // Update is called once per frame
    void Update()
    {

        
        if (primary_target != null)
        {
            if (primary_target.transform.position != primary_target_last_position)
            {
                primary_target_moved = true;
                primary_target_last_position = primary_target.transform.position;
            }
        }


        


        bool close_to_target = false;
        if (Vector3.Distance(primary_target.transform.position, this.transform.position) <= sense_dist*1.05f || Vector3.Distance(primary_target.transform.position, this.transform.position) <= stop_at*1.05f)
        {
            close_to_target = true;
        }

        nav_timer += Time.deltaTime;

        if(grid == null)
        {
            grid = NoahsGridManager.grid;
            flat_grid = NoahsNavHelper.FlattenGrid(grid);
        }

        if (useNav && grid != null && !close_to_target)
        {

            if(nav_timer > nav_increment && current_path != null)
            {
                // use when using CalculatePathAsync(grid, my_tile, target_tile);
                if(show_path && ok_to_show_path)
                {
                    NoahsGridManager.RevertShowPath();
                    NoahsGridManager.ShowPath(current_path);
                    ok_to_show_path = false;
                }

                // // Use if using StartCoroutine(FindPathAsync(grid, my_tile, target_tile)); for Web single thread
                // NoahsGridManager.RevertShowPath();
                // NoahsGridManager.ShowPath(current_path);
                
            }

            GameObject my_tile = NoahsNavHelper.FindClosestWalkableTile(this.gameObject, flat_grid);
            GameObject target_tile = NoahsNavHelper.FindClosestWalkableTile(primary_target, flat_grid);

            if (nav_timer > nav_increment || current_target == null || (!calculating_path && primary_target_moved))
            {
                nav_timer = 0;



                // Generate a new path to the target
                if (calculating_path == false && primary_target_moved == true)
                {
                    calculating_path = true;
                    primary_target_moved = false;
                    ok_to_show_path = true;
                    // StartCoroutine(FindPathAsync(grid, my_tile, target_tile)); // web single thread only
                    CalculatePathAsync(grid, my_tile, target_tile);
                }


            }

            if (current_path != null)
            {
                if (current_path.Count > 0)
                {
                    if (current_target == null || !current_path.Contains(current_target))
                    {
                        // Set current_target to the first entry in the path
                        current_target = current_path[0];
                    }
                    if(current_target == current_path[current_path.Count - 1])
                    {
                        current_target = NoahsNavHelper.NextTile(current_path,my_tile);
                    }
                }

                if (current_target != null)
                {
                    if (Vector3.Distance(this.gameObject.transform.position, current_target.transform.position) <= TileDetection)
                    {
                        int currentIndex = current_path.IndexOf(current_target);
                        if (currentIndex != -1 && currentIndex + 1 < current_path.Count)
                        {
                            current_target = current_path[currentIndex + 1];
                        }
                    }
                }
                

                GoTo(current_target, 0);

            }
            else
            {
                GoTo(primary_target, stop_at);
            }
            
        }
        else
        {
            GoTo(primary_target, stop_at);
        }
        

    }





    public void GoTo(GameObject target, float stop_at)
    {
        if (target == null)
        {
            return;
        }

        NoahsNavHelper.TurnAgent(this.gameObject,"right",turn_speed*NoahsNavHelper.RandomFloat(-randomness,randomness));

        if (Vector3.Distance(target.transform.position, this.transform.position) <= sense_dist || Vector3.Distance(target.transform.position, this.transform.position) <= stop_at)
        {
            NoahsNavHelper.TurnTowards(this.gameObject, target.transform.position, turn_speed);
            NoahsNavHelper.AllowRotation(this.gameObject, false, true, false);
            if (Vector3.Distance(target.transform.position, this.transform.position) > stop_at)
            {
                NoahsNavHelper.MoveAgent(this.gameObject, this.gameObject.transform.forward, move_speed);
            }
        }
        else
        {
            
            if(NoahsNavHelper.Sensor(this.gameObject,sense_dist*0.5f,0))
            {
                if(NoahsNavHelper.RandomChoice(new int[] {1,2}) == 1)
                {
                    NoahsNavHelper.Turn(this.gameObject,"right",150);
                }
                else
                {
                    NoahsNavHelper.Turn(this.gameObject,"right",-150);
                }
            }
            if (NoahsNavHelper.Sensor(this.gameObject,sense_dist,45))
            {
                NoahsNavHelper.TurnAgent(this.gameObject,"left",turn_speed*avoid_scaler);
            }
            if (NoahsNavHelper.Sensor(this.gameObject,sense_dist,-45))
            {
                NoahsNavHelper.TurnAgent(this.gameObject,"right",turn_speed*avoid_scaler);
            }

            NoahsNavHelper.TurnTowards(this.gameObject, target.transform.position, turn_speed);
            NoahsNavHelper.AllowRotation(this.gameObject, false, true, false);
            NoahsNavHelper.MoveAgent(this.gameObject, this.gameObject.transform.forward, move_speed);

        }
    }


    IEnumerator FindPathAsync(GameObject[,] grid, GameObject startTile, GameObject targetTile)
    {
        // Prepare thread-safe pathfinding data
        PathfindingData pathfindingData = new PathfindingData
        {
            StartPosition = GetTilePosition(grid, startTile),
            EndPosition = GetTilePosition(grid, targetTile),
            WalkableGrid = GetWalkableGrid(grid)
        };

        List<Vector2Int> path = null;

        // Run pathfinding as a coroutine
        yield return new WaitForSeconds(nav_increment);
        path = ThreadSafePathfinder.FindPath(pathfindingData);

        // Update current_path after pathfinding completes
        if (path != null)
        {
            current_path = ConvertPathToGameObjects(path);
        }
        else
        {
            Debug.Log("NO PATH FOUND!!!");
        }

        calculating_path = false;
    }







    public async void CalculatePathAsync(GameObject[,] grid, GameObject startTile, GameObject targetTile)
    {
        // run pathfinding on a semerate thread from the main thread
        // this is in use because it performs better, i.e. no lag to main thread
        
        // Prepare thread-safe pathfinding data
        PathfindingData pathfindingData = new PathfindingData
        {
            StartPosition = GetTilePosition(grid, startTile),
            EndPosition = GetTilePosition(grid, targetTile),
            WalkableGrid = GetWalkableGrid(grid)
        };

        // Run pathfinding in a background thread
        List<Vector2Int> path = await Task.Run(() => ThreadSafePathfinder.FindPath(pathfindingData));

        if (path != null)
        {
            // Map thread-safe path back to GameObjects on the main thread
            current_path = ConvertPathToGameObjects(path);
        }

        calculating_path = false;
    }

    private Vector2Int GetTilePosition(GameObject[,] grid, GameObject obj)
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == obj)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        throw new System.Exception("Object not found in grid.");
    }

    private bool[,] GetWalkableGrid(GameObject[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        bool[,] walkableGrid = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                NoahsTile tile = grid[x, y].GetComponent<NoahsTile>();
                walkableGrid[x, y] = tile != null && tile.is_walkable;
            }
        }

        return walkableGrid;
    }

    private List<GameObject> ConvertPathToGameObjects(List<Vector2Int> path)
    {
        List<GameObject> gameObjectPath = new List<GameObject>();

        foreach (Vector2Int pos in path)
        {
            gameObjectPath.Add(grid[pos.x, pos.y]);
        }

        return gameObjectPath;
    }

    







}
