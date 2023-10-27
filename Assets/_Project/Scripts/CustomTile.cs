using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Custom Tile", fileName = "CustomTile")]
public class CustomTile : Tile
{
    [field: SerializeField] 
    public int Cost { get; private set; }
    
}
