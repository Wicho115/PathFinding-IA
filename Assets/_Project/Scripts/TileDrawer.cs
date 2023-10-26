using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDrawer : MonoBehaviour
{
    [SerializeField] private Tilemap tileMapDraw;

    public void Draw(Vector3Int pos, TileBase tileToDraw)
    {
        tileMapDraw.SetTile(pos, tileToDraw);
    }

    public void UnDraw(Vector3Int pos)
    {
        tileMapDraw.SetTile(pos, null);
    }

    public void UnDrawAll()
    {
        tileMapDraw.ClearAllTiles();
    }
}
