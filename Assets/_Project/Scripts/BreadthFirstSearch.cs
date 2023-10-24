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
    [SerializeField] private TileBase infectedTile;

    [Space]
    [SerializeField] private Tilemap tileMap;

    [Header("Versions")]
    [SerializeField] private Version version = Version.CameFrom;

    enum Version{
        JustPoints,
        ReachedAndFill,
        CameFrom
    }

    [Header("Visual Vars")]
    [SerializeField, Min(0)] private float _secondsPerTileChange;
    [SerializeField, Min(0)] private float _secondsPerNeighbourChange;

    private Vector3Int? originPos;
    private Vector3Int? objectivePos;

    private Queue<Vector3Int> _frontier;
    private HashSet<Vector3Int> _reached;
    private Dictionary<Vector3Int, Vector3Int> _cameFrom;

    private bool _isInfected = false;

    private void OnSelectTile(TileBase tileSelected, Vector3Int cellPos)
    {
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
            _reached = new HashSet<Vector3Int>();

        if(version == Version.CameFrom)
            _cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        _breadthCoroutine = StartCoroutine(BreadthCoroutine());
    }

    private Coroutine _breadthCoroutine = null;
    private IEnumerator BreadthCoroutine()
    {
        AddToFrontier(originPos.Value);
        AddToReached(originPos.Value);

        while (_frontier.Count > 0)
        {
            var currentPos = _frontier.Dequeue();
            foreach (var neighbour in tileMap.GetNeighbours(currentPos))
            {
                if (_reached.Contains(neighbour)) continue;
                //Si no se ha alcanzado el vecino, se añade a la frontera y a los tiles alcanzados
                AddToFrontier(neighbour);
                AddToReached(neighbour);

                yield return new WaitForSeconds(_secondsPerTileChange);
            }

            yield return new WaitForSeconds(_secondsPerNeighbourChange);
        }

        EndAlgorithmCoroutine();

        void AddToFrontier(Vector3Int pos)
        {
            _frontier.Enqueue(pos);
            tileMap.SetTile(pos, GetInfectedTile());
        }

        void AddToReached(Vector3Int pos)
        {
            _reached.Add(pos);
            //tileMap.SetTile(pos, GetInfectedTile());
        }
    }

    private void EndAlgorithmCoroutine()
    {
        ResetPositions();
        _isInfected = !_isInfected;
        _breadthCoroutine = null;
    }

    private TileBase GetInfectedTile() => _isInfected ? defaultTile : infectedTile;

    private void ResetPositions()
    {
        tileMap.SetTile(originPos.Value, GetInfectedTile());
        tileMap.SetTile(objectivePos.Value, GetInfectedTile());
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
