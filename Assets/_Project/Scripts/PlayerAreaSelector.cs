using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


[RequireComponent(typeof(TileSelector))]
    public class PlayerAreaSelector : MonoBehaviour
    {
        [SerializeField] private TileSelector selector;
        [SerializeField] private Tilemap tilemapArea;

        [Header("Draw")]
        [SerializeField] private TileDrawer drawer;
        [SerializeField] private CustomTile originTileDraw;
        [SerializeField] private CustomTile areaTileDraw;
        [SerializeField] private CustomTile pathTileDraw;
        //[SerializeField] private CustomTile objectiveTileDraw;

        [Header("Movement Type")] [SerializeField, Min(1)]
        private int tilesToMove;

        private CustomTile originTile;
        private Vector3Int originPos, objectivePos;
        private bool isAreaActive = false;
        
        private List<Vector3Int> movementArea = new List<Vector3Int>();
        private List<Vector3Int> movementPath = new List<Vector3Int>();

        private void OnSelectTile(CustomTile tile, Vector3Int pos)
        {
            drawer.UnDrawAll();
            originPos = pos;
            drawer.Draw(originPos, originTileDraw);
            
            movementArea = PathFindingAlgorithm.FillLimited(originPos,tilesToMove, tilemapArea);
            
            foreach (var fillTilePos in movementArea)
            {
                drawer.Draw(fillTilePos, areaTileDraw);
            }

            isAreaActive = true;
        }

        private void Update()
        {
            if (!isAreaActive) return;
            
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            
            var newObjectivePos = tilemapArea.WorldToCell(mouseWorldPos);
            if (!movementArea.Contains(newObjectivePos)) return;
            if (newObjectivePos == objectivePos) return;
            objectivePos = newObjectivePos;

            if (movementPath.Any())
                UnDrawMovementPath();
            
            movementPath = PathFindingAlgorithm.PathFinding(originPos, objectivePos, tilemapArea);
            DrawPathTiles();
        }

        private void UnDrawMovementPath()
        {
            foreach (var pathTile in movementPath)
            {
                var drawTile = movementArea.Contains(pathTile) ? areaTileDraw : null;
                drawer.Draw(pathTile, drawTile);
            }
        }

        private void DrawPathTiles()
        {
            foreach (var pathTile in movementPath)  
            {
                drawer.Draw(pathTile, pathTileDraw);
            }
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