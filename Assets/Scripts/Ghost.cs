using UnityEngine;
using UnityEngine.Tilemaps;
using Tile = UnityEngine.Tilemaps.Tile;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackingPiece;

    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (var i = 0; i < cells.Length; i++)
        {
            var tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (var i = 0; i < cells.Length; i++)
        {
            cells[i] = trackingPiece.cells[i];
        }
    }

    private void Drop()
    {
        Vector3Int Position = trackingPiece.position;

        var current = Position.y;
        var bottom = -board.boardSize.y / 2 - 1;

        board.Clear(trackingPiece);
        
        for (var row = current; row >= bottom; row--)
        {
            Position.y = row;

            if (board.IsValidPosition(trackingPiece, Position))
            {
                position = Position;
            }
            else
            {
                break;
            }
        }
        
        board.Set(trackingPiece);
    }

    private void Set()
    {
        for (var i = 0; i < cells.Length; i++)
        {
            var tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, tile);
        }
    }
    
}
