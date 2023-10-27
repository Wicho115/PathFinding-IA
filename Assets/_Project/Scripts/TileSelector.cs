using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;

    public event Action<CustomTile, Vector3Int> OnSelectTile;

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        var cellPos = tileMap.WorldToCell(mouseWorldPos);
        var tile = tileMap.GetTile(cellPos);
        if (tile == null) return;
        var customTile = tile as CustomTile;
        if (customTile == null) return;
        OnSelectTile?.Invoke(customTile, cellPos);
    }
}
