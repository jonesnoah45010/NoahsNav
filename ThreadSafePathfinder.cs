using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NoahsNav
{
    public class ThreadSafePathfinder
    {
        public static List<Vector2Int> FindPath(PathfindingData data)
        {
            // Extract data
            Vector2Int startPos = data.StartPosition;
            Vector2Int endPos = data.EndPosition;
            bool[,] walkableGrid = data.WalkableGrid;

            // Priority queue for open set
            PriorityQueue<Vector2Int> openSet = new PriorityQueue<Vector2Int>();
            HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

            // Path tracking
            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            Dictionary<Vector2Int, float> gCost = new Dictionary<Vector2Int, float>();
            Dictionary<Vector2Int, float> fCost = new Dictionary<Vector2Int, float>();

            gCost[startPos] = 0;
            fCost[startPos] = GetHeuristicCost(startPos, endPos);
            openSet.Enqueue(startPos, fCost[startPos]);

            while (openSet.Count > 0)
            {
                Vector2Int current = openSet.Dequeue();

                // Check if we reached the destination
                if (current == endPos)
                {
                    return ReconstructPath(cameFrom, current);
                }

                closedSet.Add(current);

                foreach (Vector2Int neighbor in GetNeighbors(walkableGrid, current))
                {
                    if (closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    float tentativeGCost = gCost[current] + GetMovementCost(current, neighbor);

                    if (!gCost.ContainsKey(neighbor) || tentativeGCost < gCost[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gCost[neighbor] = tentativeGCost;
                        fCost[neighbor] = tentativeGCost + GetHeuristicCost(neighbor, endPos);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Enqueue(neighbor, fCost[neighbor]);
                        }
                    }
                }
            }

            return null; // No path found
        }

        private static float GetHeuristicCost(Vector2Int a, Vector2Int b)
        {
            // Octile distance
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);
            return dx + dy + (Mathf.Sqrt(2) - 2) * Mathf.Min(dx, dy);
        }

        private static float GetMovementCost(Vector2Int current, Vector2Int neighbor)
        {
            // Return diagonal or straight cost
            return (current.x != neighbor.x && current.y != neighbor.y) ? Mathf.Sqrt(2) : 1;
        }

        private static List<Vector2Int> GetNeighbors(bool[,] grid, Vector2Int tile)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>(8); // Preallocate space
            Vector2Int[] directions = {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                new Vector2Int(1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(-1, 1)
            };

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = tile + dir;
                if (neighbor.x >= 0 && neighbor.y >= 0 &&
                    neighbor.x < grid.GetLength(0) && neighbor.y < grid.GetLength(1) &&
                    grid[neighbor.x, neighbor.y]) // Only add walkable tiles
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
        {
            List<Vector2Int> path = new List<Vector2Int> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }
            path.Reverse();
            return path;
        }
    }

    public class PriorityQueue<T>
    {
        private SortedDictionary<float, Queue<T>> queue = new SortedDictionary<float, Queue<T>>();

        public void Enqueue(T item, float priority)
        {
            if (!queue.ContainsKey(priority))
                queue[priority] = new Queue<T>();

            queue[priority].Enqueue(item);
        }

        public T Dequeue()
        {
            var first = queue.First();
            var item = first.Value.Dequeue();

            if (first.Value.Count == 0)
                queue.Remove(first.Key);

            return item;
        }

        public bool Contains(T item)
        {
            return queue.Any(x => x.Value.Contains(item));
        }

        public int Count => queue.Sum(x => x.Value.Count);
    }

}



