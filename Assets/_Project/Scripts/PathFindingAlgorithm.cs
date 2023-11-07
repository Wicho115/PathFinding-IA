using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFindingAlgorithm : MonoBehaviour
{
    private PriorityQueue<Vector3Int> _frontier = new PriorityQueue<Vector3Int>();
    private Dictionary<Vector3Int, Vector3Int?> _cameFrom = new Dictionary<Vector3Int, Vector3Int?>();
    private Dictionary<Vector3Int, int> _costSoFar = new Dictionary<Vector3Int, int>();

    public List<Vector3Int> PathFinding(Vector3Int originPos, Vector3Int objectivePos, Tilemap tileMap)
    {
        RestartPathFinding();

        _frontier.Enqueue(originPos, 0);
        _cameFrom.Add(originPos, null);
        _costSoFar.Add(originPos, 0);

        while (_frontier.Count > 0)
        {
            bool endEarly = false;

            var current = _frontier.Dequeue();

            foreach (var next in tileMap.GetNeighbours(current))
            {
                //Si no es customTile, no tiene peso, no usar
                var nextTile = tileMap.GetTile(next) as CustomTile;
                if (nextTile == null) continue;

                if (next == objectivePos)
                {
                    endEarly = true;
                    break;
                }

                var newCost = _costSoFar[current] + nextTile.Cost;


                if (!_costSoFar.ContainsKey(next) || newCost < _costSoFar[next])
                {
                    _costSoFar.Add(next, newCost);
                    var priority = newCost + Heuristic(objectivePos, next);
                    _frontier.Enqueue(next, priority);
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

        return pathToFollow;
    }

    public void FillLimited(int tilesToFill)
    {

    }

    private int Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private void RestartPathFinding() 
    {
        _frontier.Clear();
        _cameFrom.Clear();
        _costSoFar.Clear();
    }
}
