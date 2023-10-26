using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileSelector))]
public class MoveSelector : MonoBehaviour
{
    [SerializeField] private TileSelector selector;

    public delegate void OnMoveSelectionDel(OnMoveSelectionArgs args);
    public event OnMoveSelectionDel OnMoveSelectionDone;

    private CustomTile originTile, objectiveTile;

    private void Awake()
    {
        selector ??= GetComponent<TileSelector>();
    }

    private void OnSelectedTile(TileBase tile, Vector3Int pos)
    {
        var customTile = tile as CustomTile;
        if (customTile == null) return; 

        if(originTile == null)
        {
            originTile = customTile;
            return;
        }

        objectiveTile = customTile;

        OnMoveSelectionDone?.Invoke(new OnMoveSelectionArgs(originTile, objectiveTile));

        originTile = null;
        objectiveTile = null;
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
    public CustomTile origin { get; set; }
    public CustomTile objective { get; set; }

    public OnMoveSelectionArgs(CustomTile origin, CustomTile objective)
    {
        this.origin = origin;
        this.objective = objective;
    }
}