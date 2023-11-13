using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileSelector), typeof(PlayerAreaSelector))]
public class PlayerTileMovement : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Tilemap tilemapPlayer;
    [SerializeField] private TileSelector selector;
    [SerializeField] private PlayerAreaSelector playerAreaSelector;

    [SerializeField] private float velocity;
    private bool _isMoving = false;

    private IEnumerator MoveToPos(List<Vector3Int> cells)
    {
        _isMoving = true;
        //AL PARECER INICIA AL REVES 
        cells.Reverse();
        
        foreach (var cell in cells)
        {
            var objectiveCell = tilemapPlayer.CellToWorld(cell);
            
            
            yield return new WaitUntil(() =>
            {
                transform.position = Vector3.MoveTowards(transform.position, objectiveCell, Time.deltaTime * velocity);
                return (transform.position - objectiveCell).sqrMagnitude < 0.0005f || transform.position == objectiveCell;
            });

            //if (cell == playerAreaSelector.ObjectivePos) break;
            transform.position = objectiveCell;
        }
        
        _isMoving = false;
    }
    
    private void OnSelectTile(CustomTile tile, Vector3Int pos)
    {
        if (_isMoving) return;
        if (!playerAreaSelector.IsAreaActive) return;

        StartCoroutine(MoveToPos(playerAreaSelector.MovementPath));
        playerAreaSelector.DeactivateArea();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isMoving) return;
        var cell = tilemapPlayer.WorldToCell(transform.position);
        playerAreaSelector.ActivateArea(cell, 7);
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
