using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;

    public event Action<TileBase, Vector3Int> OnSelectTile;

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        var cellPos = tileMap.WorldToCell(mouseWorldPos);
        var tile = tileMap.GetTile(cellPos);
        if (tile == null) return;
        OnSelectTile?.Invoke(tile, cellPos);
    }
}
