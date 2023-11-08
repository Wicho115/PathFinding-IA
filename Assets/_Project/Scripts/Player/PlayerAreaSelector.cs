using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PlayerAreaSelector : MonoBehaviour
{
    [SerializeField] private Tilemap tilemapArea;

    [Header("Draw")]
    [SerializeField] private TileDrawer drawer;
    [SerializeField] private CustomTile originTileDraw;
    [SerializeField] private CustomTile areaTileDraw;
    [SerializeField] private CustomTile pathTileDraw;
    //[SerializeField] private CustomTile objectiveTileDraw;
    
    public List<Vector3Int> MovementArea { get; private set; } = new List<Vector3Int>();
    public List<Vector3Int> MovementPath { get; private set; } = new List<Vector3Int>();

    public bool IsAreaActive => _isAreaActive;
    private bool _isAreaActive = false;

    private CustomTile originTile;
    public Vector3Int ObjectivePos => objectivePos;
    private Vector3Int originPos, objectivePos;
    
    public void ActivateArea(Vector3Int pos, int tileMovement)
    {
        drawer.UnDrawAll();
        originPos = pos;

        MovementArea = PathFindingAlgorithm.FillLimited(originPos, tileMovement, tilemapArea);

        foreach (var fillTilePos in MovementArea)
        {
            drawer.Draw(fillTilePos, areaTileDraw);
        }
        
        drawer.Draw(originPos, originTileDraw);

        _isAreaActive = true;
    }
    
    public void DeactivateArea()
    {
        drawer.UnDrawAll();
        
        
        _isAreaActive = false;
    }

    private void Update()
    {
        if (!_isAreaActive) return;
        
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        
        var newObjectivePos = tilemapArea.WorldToCell(mouseWorldPos);
        if (!MovementArea.Contains(newObjectivePos)) return;
        if (newObjectivePos == objectivePos) return;
        objectivePos = newObjectivePos;

        if (MovementPath.Any())
            UnDrawMovementPath();
        
        MovementPath = PathFindingAlgorithm.PathFinding(originPos, objectivePos, tilemapArea);
        DrawPathTiles();
    }

    private void UnDrawMovementPath()
    {
        foreach (var pathTile in MovementPath)
        {
            var drawTile = MovementArea.Contains(pathTile) ? areaTileDraw : null;
            drawer.Draw(pathTile, drawTile);
        }
    }

    private void DrawPathTiles()
    {
        foreach (var pathTile in MovementPath)  
        {
            drawer.Draw(pathTile, pathTileDraw);
        }
    }
}