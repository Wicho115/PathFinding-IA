using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDrawer : MonoBehaviour
{
    [SerializeField] private Tilemap tileMapDraw;

    public void Draw(Vector3Int pos, TileBase tileToDraw)
    {
        tileMapDraw.SetTile(pos, tileToDraw);
    }

    public void Draw(Vector3Int pos, TileBase tileToDraw, float alpha)
    {
        var newcolor = new Color(1, 1, 1, alpha);
        tileMapDraw.SetTile(pos, tileToDraw);
        tileMapDraw.SetColor(pos, newcolor);
    }

    public void UnDraw(Vector3Int pos)
    {
        var newcolor = new Color(1, 1, 1, 1);
        tileMapDraw.SetTile(pos, null);
        tileMapDraw.SetColor(pos, newcolor);
    }

    public void UnDrawAll()
    {
        tileMapDraw.ClearAllTiles();
    }
}
