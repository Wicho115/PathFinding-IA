using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class PathFindingAlgorithm
{
    private static Queue<Vector3Int> _frontier = new Queue<Vector3Int>();
    private static PriorityQueue<Vector3Int> _frontierPriority = new PriorityQueue<Vector3Int>();
    private static Dictionary<Vector3Int, Vector3Int?> _cameFrom = new Dictionary<Vector3Int, Vector3Int?>();
    private static Dictionary<Vector3Int, int> _costSoFar = new Dictionary<Vector3Int, int>();
    private static HashSet<Vector3Int> _reached = new HashSet<Vector3Int>();

    public static List<Vector3Int> PathFinding(Vector3Int originPos, Vector3Int objectivePos, Tilemap tileMap)
    {
        RestartPathFinding();

        _frontierPriority.Enqueue(originPos, 0);
        _cameFrom.Add(originPos, null);
        _costSoFar.Add(originPos, 0);

        while (_frontierPriority.Count > 0)
        {
            bool endEarly = false;

            var current = _frontierPriority.Dequeue();

            foreach (var next in tileMap.GetNeighbours(current))
            {
                //Si no es customTile, no tiene peso, no usar
                var nextTile = tileMap.GetTile(next) as CustomTile;
                if (nextTile == null) continue;

                var newCost = _costSoFar[current] + nextTile.Cost;


                if (!_costSoFar.ContainsKey(next) || newCost < _costSoFar[next])
                {
                    _costSoFar.Add(next, newCost);
                    var priority = newCost + Heuristic(objectivePos, next);
                    _frontierPriority.Enqueue(next, priority);
                    _cameFrom.Add(next, current);
                }

                //Si el vecino es el final, se  termina
                if (next == objectivePos)
                {
                    endEarly = true;
                    break;
                }
            }

            if (endEarly) break;
        }

        //Last Thing
        Vector3Int? from = objectivePos;
        List<Vector3Int> pathToFollow = new();

        while (from != null)
        {
            pathToFollow.Add(from.Value);
            from = _cameFrom[from.Value];
        }

        return new List<Vector3Int>(pathToFollow);
    }

    public static List<Vector3Int> FillLimited(Vector3Int originPos ,int tilesToFill, Tilemap tileMap)
    {
        
        RestartPathFinding();

        if (tilesToFill == 1)
        {
            Debug.Log("origin");
            Debug.Log(originPos);
            _reached.Add(originPos);
            var neightbours = tileMap.GetNeighbours(originPos, true);

            for(int i = 0; i < neightbours.Count - 1; i++)
            {
                Debug.Log("vecinos");
                Debug.Log(neightbours[i]);
                _reached.Add(neightbours[i]);
            }

            return new List<Vector3Int>(_reached);
        }

        _frontier.Enqueue(originPos);
        _reached.Add(originPos);
        _costSoFar.Add(originPos, tilesToFill);
        
        while (_frontier.Count > 0)
        {
            var current = _frontier.Dequeue();
            foreach (var neighbour in tileMap.GetNeighbours(current))
            {
                //Si no se ha alcanzado el vecino, se a�ade a la frontera y a los tiles alcanzados
                if (_reached.Contains(neighbour)) continue;
                
                var tile = tileMap.GetTile(neighbour) as CustomTile;
                var tileCost = tile == null ? 1 : tile.Cost;

                var newCost = _costSoFar[current] - tileCost;
                if(newCost < 0) continue;
                
                _frontier.Enqueue(neighbour);
                _reached.Add(neighbour);
                _costSoFar[neighbour] = newCost;
            }
        }
        
        return new List<Vector3Int>(_reached);
    }

    private static int Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static void RestartPathFinding() 
    {
        _frontierPriority.Clear();
        _reached.Clear();
        _cameFrom.Clear();
        _costSoFar.Clear();
    }
}
