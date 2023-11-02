using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreadthFirstSearch : MonoBehaviour
{
    [SerializeField] private TileSelector selector;
    
    [Space]
    [SerializeField] private TileBase defaultTile;
    [SerializeField] private TileBase originTile;
    [SerializeField] private TileBase objectiveTile;
    [SerializeField] private TileBase fillTile;
    [SerializeField] private TileBase pathToFollowTile;

    [Space]
    [SerializeField] private Tilemap tileMap;

    [Header("Versions")]
    [SerializeField] private Version version = Version.CameFrom;

    enum Version{
        JustPoints,
        ReachedAndFill,
        CameFrom,
        EndEarly,
        EndEarlyAndFill,
    }

    [SerializeField] private bool useHeuristics;
    
    [Header("Visual Vars")]
    [SerializeField, Min(0)] private float _secondsPerTileChange;
    [SerializeField, Min(0)] private float _secondsPerNeighbourChange;

    private Vector3Int? originPos;
    private Vector3Int? objectivePos;

    private Queue<Vector3Int> _frontier;
    private HashSet<Vector3Int> _reached;
    private Dictionary<Vector3Int, Vector3Int?> _cameFrom;

    private TileBase CurrentTile=> _isFilled ? defaultTile : fillTile;
    private TileBase AlternativeCurrentTile => _isFilled ? fillTile : defaultTile;
    private bool _isFilled = false;

    private void OnSelectTile(TileBase tileSelected, Vector3Int cellPos)
    {
        if (algorithmCor != null) return;

        if (originPos == null)
        {
            originPos = cellPos;
            tileMap.SetTile(cellPos, originTile);
            Debug.Log($"Origen: {cellPos}");
            return;
        }

        if (objectivePos == null)
        {
            objectivePos = cellPos;
            tileMap.SetTile(cellPos, objectiveTile);
            Debug.Log($"Origen: {cellPos}");

            if(version == Version.JustPoints) return;
        }

        if (version == Version.JustPoints)
            ResetPositions();

        DoAlgorithim();
    }

    private void DoAlgorithim()
    {
        _frontier = new Queue<Vector3Int>();

        if(version == Version.ReachedAndFill)
        {
            _reached = new HashSet<Vector3Int>();
            algorithmCor = StartCoroutine(FillAlgorithmCoroutine());
        }

        if (version == Version.CameFrom || version == Version.EndEarly || version == Version.EndEarlyAndFill)
        {
            _cameFrom = new Dictionary<Vector3Int, Vector3Int?>();
            algorithmCor = StartCoroutine(CameFromAlgorithmCoroutine());
        }

    }

    private Coroutine algorithmCor = null;
    private IEnumerator FillAlgorithmCoroutine()
    {
        AddToFrontier(originPos.Value);
        AddToReached(originPos.Value);

        while (_frontier.Count > 0)
        {
            var current = _frontier.Dequeue();
            foreach (var neighbour in tileMap.GetNeighbours(current))
            {
                if (_reached.Contains(neighbour)) continue;
                //Si no se ha alcanzado el vecino, se a�ade a la frontera y a los tiles alcanzados
                AddToFrontier(neighbour);
                AddToReached(neighbour);

                yield return new WaitForSeconds(_secondsPerTileChange);
            }

            yield return new WaitForSeconds(_secondsPerNeighbourChange);
        }

        EndAlgorithm();

        void AddToFrontier(Vector3Int pos)
        {
            _frontier.Enqueue(pos);
            tileMap.SetTile(pos, CurrentTile);
        }

        void AddToReached(Vector3Int pos)
        {
            _reached.Add(pos);
            //tileMap.SetTile(pos, GetInfectedTile());
        }
    }

    private IEnumerator CameFromAlgorithmCoroutine()
    {
        AddToFrontier(originPos.Value);
        AddToCameFrom(originPos.Value, null);

        while (_frontier.Count > 0)
        {
            bool endEarly = false;

            var current = _frontier.Dequeue();
            foreach (var neighbour in tileMap.GetNeighbours(current))
            {
                if (_cameFrom.ContainsKey(neighbour)) continue;
                //Si no se ha alcanzado el vecino, se a�ade a la frontera y a los tiles de donde viene
                AddToFrontier(neighbour);
                AddToCameFrom(neighbour, current);
                if(neighbour == objectivePos && (version == Version.EndEarly || version == Version.EndEarlyAndFill))
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

        void AddToFrontier(Vector3Int pos)
        {
            _frontier.Enqueue(pos);
            tileMap.SetTile(pos, CurrentTile);
        }

        void AddToCameFrom(Vector3Int current, Vector3Int? from)
        {
            _cameFrom.Add(current, from);
            //tileMap.SetTile(pos, GetInfectedTile());
        }
    }

    private void EndAlgorithm()
    {
        ResetPositions();
        _isFilled = !_isFilled;
        algorithmCor = null;
        Debug.Log("End Algorithm");
    }
    
    private int Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private void EndAlgorithmWithPath()
    {
        ResetPositions(pathToFollowTile, pathToFollowTile);
        _isFilled = !_isFilled;
        algorithmCor = null;
        Debug.Log("End Algorithm");
    }

    private void PaintFollowPathAndEndAlgorithm()
    {
        if (version == Version.EndEarlyAndFill)
        {
            tileMap.SwapTile(AlternativeCurrentTile, CurrentTile);
            tileMap.SwapTile(pathToFollowTile, CurrentTile);
        }

        Vector3Int? from = objectivePos.Value;
        List<Vector3Int> pathToFollow = new();
        while (from != null)
        {
            pathToFollow.Add(from.Value);
            from = _cameFrom[from.Value];
        }

        foreach(var cell in pathToFollow)
        {
            tileMap.SetTile(cell, pathToFollowTile);
        }

        EndAlgorithmWithPath();
        Debug.Log("End Algorithm");
    }


    private void ResetPositions(TileBase origintile = null, TileBase objectiveTile = null)
    {
        origintile ??= CurrentTile;
        objectiveTile ??= CurrentTile;

        tileMap.SetTile(originPos.Value, origintile);
        tileMap.SetTile(objectivePos.Value, objectiveTile);

        originPos = null;
        objectivePos = null;
    }

    private void OnEnable()
    {
        selector.OnSelectTile += OnSelectTile;
    }

    private void OnDisable()
    {
        selector.OnSelectTile -= OnSelectTile;
    }
}
