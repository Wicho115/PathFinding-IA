using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileSelector))]
public class PathSelector : MonoBehaviour
{
    [SerializeField] private TileSelector selector;

    [Header("Draw")]
    [SerializeField] private TileDrawer drawer;
    [SerializeField] private CustomTile originTileDraw;
    [SerializeField] private CustomTile objectiveTileDraw;

    public delegate void OnMoveSelectionDel(OnMoveSelectionArgs args);
    public event OnMoveSelectionDel OnMoveSelectionDone;

    private CustomTile originTile, objectiveTile;
    private Vector3Int originPos, objectivePos;

    private void Awake()
    {
        selector ??= GetComponent<TileSelector>();
    }

    private void OnSelectedTile(CustomTile tile, Vector3Int pos)
    {
        if (objectiveTile != null)
        {
            drawer.UnDrawAll();
            originTile = null;
            objectiveTile = null;
        }
        
        if (originTile == null)
        {
            originTile = tile;
            originPos = pos;
            drawer.Draw(pos, originTileDraw);
            return;
        }

        objectiveTile = tile;
        objectivePos = pos;
        drawer.Draw(pos, objectiveTileDraw);

        OnMoveSelectionDone?.Invoke(new OnMoveSelectionArgs(originTile, objectiveTile, originPos, objectivePos));
    }

    private void OnEnable()
    {
        selector.OnSelectTile += OnSelectedTile;
    }

    private void OnDisable()
    {
        selector.OnSelectTile -= OnSelectedTile;
    }
}
public struct OnMoveSelectionArgs
{
    public Vector3Int originPos;
    public CustomTile origin { get; set; }
    public Vector3Int objectivePos;
    public CustomTile objective { get; set; }

    public OnMoveSelectionArgs(CustomTile origin, CustomTile objective, Vector3Int originPos, Vector3Int objectivePos)
    {
        this.origin = origin;
        this.objective = objective;
        this.originPos = originPos;
        this.objectivePos = objectivePos;
    }
}