using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectCellLocation : MonoBehaviour
{

    [Header("World")]
    public GridLayout gridLayout;
    private Vector3 worldPosition;

    [Header("Tiles")]
    public Tilemap tilemap;
    
    [Header("Algorithms")]
    public Socavon socpow;
    public Levitacion levitpow;

    [Header("Active Algorithm")]
    public bool Socav;
    public bool Levit;

    private TileBase originalTileBase;
    private Vector3Int? origenTile;
    private Vector3Int? originalTile;
    private Vector3Int? destinoTile;

    private void Start()
    {
        origenTile = null;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            Socav = true;
            Levit = false;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2)){
            Levit = true;
            Socav = false;
        }


        if (Socav == true)
        {
            FloodFill();
            socpow.enabled = true;
            levitpow.enabled = false;

        }
        else if (Levit == true)
        {
            Heuristic();
            levitpow.enabled = true;
            socpow.enabled = false;
        }
    }
    private Vector3Int GetPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int cellPosition = gridLayout.WorldToCell(worldPosition);
        mousePos = gridLayout.CellToWorld(cellPosition);
        cellPosition.z = 0;
        return cellPosition;
    }
    public void FloodFill()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var actualTile = tilemap.GetTile(GetPosition());
            if (actualTile == null) { return; }
            Debug.Log("Origen " + GetPosition());
            socpow.startingPoint = GetPosition();
            origenTile = GetPosition();
        }

        if (Input.GetMouseButtonDown(1))
        {
            var actualTile = tilemap.GetTile(GetPosition());
            if (actualTile == null) { return; }
            Debug.Log("Destino " + GetPosition());
            socpow.objective = GetPosition();
            destinoTile = GetPosition();
        }

        if (originalTile != GetPosition())
        {
            originalTile = GetPosition();
            originalTileBase = tilemap.GetTile(GetPosition());
        }
    }
    public void Heuristic()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var actualTile = tilemap.GetTile(GetPosition());
            if (actualTile == null) { return; }
            Debug.Log("Origen " + GetPosition());
            levitpow.startingPoint = GetPosition();
            origenTile = GetPosition();
        }

        if (Input.GetMouseButtonDown(1))
        {
            var actualTile = tilemap.GetTile(GetPosition());
            if (actualTile == null) { return; }
            Debug.Log("Destino " + GetPosition());
            levitpow.objective = GetPosition();
            destinoTile = GetPosition();
        }

        if (originalTile != GetPosition())
        {
            originalTile = GetPosition();
            originalTileBase = tilemap.GetTile(GetPosition());
        }
    }
}

