using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DijkstrasAlgorithm : MonoBehaviour
{
    [SerializeField] private MoveSelector moveSelector;

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
        Weight
    }

    [SerializeField] private bool _endEarly = true;

    [Header("Visual Vars")]
    [SerializeField, Min(0)] private float _secondsPerTileChange;
    [SerializeField, Min(0)] private float _secondsPerNeighbourChange;

    private Vector3Int? originPos;
    private Vector3Int? objectivePos;

    private PriorityQueue<Vector3Int> _frontier;
    private Dictionary<Vector3Int, Vector3Int?> _cameFrom;
    private Dictionary<Vector3Int, int> _costSoFar;

    private void OnMoveSelection(OnMoveSelectionArgs args)
    {
        if (algorithmCor != null) return;
        originPos = args.origin.Position;
        objectivePos = args.objective.Position;

        Debug.Log($"Origen: {originPos}");
        Debug.Log($"Objetivo: {objectivePos}");

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
        _frontier.Enqueue(originPos.Value, 0);
        _cameFrom.Add(originPos.Value, null);
        _costSoFar.Add(originPos.Value, 0);

        while (_frontier.Count > 0)
        {
            bool endEarly = false;

            var current = _frontier.Dequeue();

            foreach (var next in tileMap.GetNeighbours(current))
            {
                var nextTile = tileMap.GetTile(next) as CustomTile;
                if (nextTile == null) continue;

                var newCost = _costSoFar[current] + nextTile.Cost;
                if (!_costSoFar.ContainsKey(next) || newCost < _costSoFar[next])
                {
                    _costSoFar.Add(next, newCost);
                    var priority = newCost;
                    _frontier.Enqueue(next, priority);
                    _cameFrom.Add(next, current);
                }

                //Si no se ha alcanzado el vecino, se añade a la frontera y a los tiles de donde viene

                //Si el vecino es el final, se  termina
                if (next == objectivePos && _endEarly)
                {
                    endEarly = true;
                    break;
                }
                yield return new WaitForSeconds(_secondsPerTileChange);
            }

            if (endEarly) break;

            yield return new WaitForSeconds(_secondsPerNeighbourChange);
        }

        PaintFollowPathAndEndAlgorithm();

    }

    private void PaintFollowPathAndEndAlgorithm()
    {
        Vector3Int? from = objectivePos.Value;
        List<Vector3Int> pathToFollow = new();

        while (from != null)
        {
            pathToFollow.Add(from.Value);
            from = _cameFrom[from.Value];
        }

        foreach (var cell in pathToFollow)
        {
            Debug.Log("Draw on cell: " + cell.ToString());
            tileDrawer.Draw(cell, pathToFollowTile);
        }

        ResetPositions();
        algorithmCor = null;
    }

    private void ResetPositions()
    {
        tileDrawer.Draw(originPos.Value, pathToFollowTile);
        tileDrawer.Draw(objectivePos.Value, pathToFollowTile);

        originPos = null;
        objectivePos = null;
    }

    private void OnEnable()
    {
        moveSelector.OnMoveSelectionDone += OnMoveSelection;
    }

    private void OnDisable()
    {
        moveSelector.OnMoveSelectionDone -= OnMoveSelection;
    }
}
