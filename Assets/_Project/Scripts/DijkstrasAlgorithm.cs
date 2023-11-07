using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class DijkstrasAlgorithm : MonoBehaviour
{
    [FormerlySerializedAs("moveSelector")] [SerializeField] private PathSelector pathSelector;

    [Space]
    [SerializeField] private TileBase defaultTile;
    [SerializeField] private TileBase originTile;
    [SerializeField] private TileBase objectiveTile;
    [SerializeField] private TileBase fillTile;
    [SerializeField] private TileBase pathToFollowTile;

    [Space]
    [SerializeField] private TileDrawer tileDrawer;

    [Space]
    [SerializeField] private Tilemap tileMap;

    [Header("Versions")]
    [SerializeField] private Version version = Version.Weight;
    enum Version
    {
        Weight,
        Heuristics,
        AStar
    }

    [SerializeField] private bool _paintIt;

    [SerializeField] private bool _endEarly = true;

    [Header("Visual Vars")]
    [SerializeField, Min(0)] private float _secondsPerTileChange;
    [SerializeField, Min(0)] private float _secondsPerNeighbourChange;

    private Vector3Int _originPos, _objectivePos;

    private PriorityQueue<Vector3Int> _frontier;
    private Dictionary<Vector3Int, Vector3Int?> _cameFrom;
    private Dictionary<Vector3Int, int> _costSoFar;

    private void OnPathSelection(OnMoveSelectionArgs args)
    {
        if (algorithmCor != null) return;

        _originPos = args.originPos;
        _objectivePos = args.objectivePos;

        DoAlgorithim();
    }

    private void DoAlgorithim()
    {
        _frontier = new PriorityQueue<Vector3Int>();
        _cameFrom = new Dictionary<Vector3Int, Vector3Int?>();
        _costSoFar = new Dictionary<Vector3Int, int>();

        algorithmCor = StartCoroutine(CameFromAlgorithmCoroutine());
    }

    private Coroutine algorithmCor = null;

    private IEnumerator CameFromAlgorithmCoroutine()
    {
        yield return null;
        _frontier.Enqueue(_originPos, 0);
        _cameFrom.Add(_originPos, null);
        _costSoFar.Add(_originPos, 0);

        while (_frontier.Count > 0)
        {
            bool endEarly = false;

            var current = _frontier.Dequeue();

            foreach (var next in tileMap.GetNeighbours(current))
            {
                if (_cameFrom.ContainsKey(next)) continue;
                var nextTile = tileMap.GetTile(next) as CustomTile;
                if (nextTile == null) continue;

                var newCost = _costSoFar[current] + nextTile.Cost;
                if (!_costSoFar.ContainsKey(next) || newCost < _costSoFar[next])
                {
                    _costSoFar.Add(next, newCost);
                    var priority = newCost;

                    priority = version switch 
                    {
                        Version.Heuristics => Heuristic(_objectivePos, next),
                        Version.AStar => priority + Heuristic(_objectivePos, next),
                        _ => priority
                    };

                    _frontier.Enqueue(next, priority);
                    _cameFrom.Add(next, current);
                    //Si se quiere pintar, pintalo
                    if (_paintIt)
                    {
                        tileDrawer.Draw(next, fillTile);
                        yield return new WaitForSeconds(_secondsPerNeighbourChange);
                    }
                }

                //Si el vecino es el final, se  termina
                if (next == _objectivePos && _endEarly)
                {
                    endEarly = true;
                    break;
                }
                //yield return new WaitForSeconds(_secondsPerTileChange);
            }

            if (endEarly) break;

            //yield return new WaitForSeconds(_secondsPerNeighbourChange);
        }

        PaintFollowPathAndEndAlgorithm();
    }

    private void PaintFollowPathAndEndAlgorithm()
    {
        Vector3Int? from = _objectivePos;
        List<Vector3Int> pathToFollow = new();

        while (from != null)
        {
            pathToFollow.Add(from.Value);
            from = _cameFrom[from.Value];
        }

        foreach (var cell in pathToFollow)
        {
            tileDrawer.Draw(cell, pathToFollowTile);
        }
        
        tileDrawer.Draw(_originPos, pathToFollowTile);
        tileDrawer.Draw(_objectivePos, pathToFollowTile);
        
        algorithmCor = null;
    }
    
    private int Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private void OnEnable()
    {
        pathSelector.OnMoveSelectionDone += OnPathSelection;
    }

    private void OnDisable()
    {
        pathSelector.OnMoveSelectionDone -= OnPathSelection;
    }
}
