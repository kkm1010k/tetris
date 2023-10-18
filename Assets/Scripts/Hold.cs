using UnityEngine;
using UnityEngine.Tilemaps;

public class Hold : MonoBehaviour
{
    private Tilemap tilemap;
    public Tile tile { get; private set; }
    private Vector3Int[] cells;
    public TetrominoData tetromino;
    public TetrominoData tetrominobefore;
    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        tilemap.ClearAllTiles();
    }
    
    public void Holding(Piece piece)
    {
        tilemap.ClearAllTiles();
        
        if (cells is null)
        {
            cells = new Vector3Int[4];
        }

        tetrominobefore = tetromino;
        tetromino = piece.data;
        
        Vector3Int position = new Vector3Int(-1,-1,0);
        
        
        for (var i = 0; i < tetromino.cells.Length; i++)
        {
            cells[i] = (Vector3Int)tetromino.cells[i];
        }
        tile = tetromino.tile;
        
        for (var i = 0; i < cells.Length; i++)
        {
            var tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, tile);
        }
    }

    public void GameOver()
    {
        tilemap.ClearAllTiles();
    }
}
