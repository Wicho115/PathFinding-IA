using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Custom Tile", fileName = "CustomTile")]
public class CustomTile : Tile
{
    [field: SerializeField] 
    public int Cost { get; private set; }
    public Vector3Int Position { get; private set; }

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        var success = base.StartUp(position, tilemap, go);
        Debug.Log(position);
        return success;
    }
}
