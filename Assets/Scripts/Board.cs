using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public Next next { get; private set; }
    public Hold hold  { get; private set; }
    public Score score { get; private set; }
    
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition;
    public Vector3Int spawnPositionadd;
    public Vector2Int boardSize = new(10, 20);
    public List<int> sevenBag = new();
    public List<int> Bag = new();
    
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize + new Vector2Int(0,3));
        }
    }
    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        hold = GameObject.Find("Board_Hold").GetComponent<Hold>();
        next = GameObject.Find("Board_Next").GetComponent<Next>();
        score = GameObject.Find("Board_Score").GetComponent<Score>();
        
        for (var i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }
    public void SpawnPiece(bool holdcnt = false, bool Isholded = false)
    {
        Reload();
        TetrominoData data = new TetrominoData();
        if (!Isholded && !holdcnt)
        {
            data = tetrominos[Bag[0]];
        }
        else
        {
            data = hold.tetrominobefore;
        }
        Bag.RemoveAt(0);

        activePiece.Initialize(this, spawnPosition, data);
        
        next.Set(Bag);
        
        for (var i = 0; i < 3; i++)
        {
            
            if (IsValidPosition(activePiece, spawnPosition + new Vector3Int(0,i,0)))
            {
                spawnPositionadd = spawnPosition + new Vector3Int(0, i, 0);
                activePiece.position = spawnPositionadd;
                Set(activePiece);
                return;
            }
        }
        GameOver();
        activePiece.GameOver();
        hold.GameOver();
    }

    public void Reload()
    {
        if (sevenBag.Count == 0)
        {
            sevenBag.Clear();
            for (var i = 0; i < tetrominos.Length; i++)
            {
                sevenBag.Add(i);
            }
            
        }
        
        while (Bag.Count < 30)
        {
            for (var i = sevenBag.Count-1; i > 0; i--)
            {
                var rnd = Random.Range(0, i);

                (sevenBag[i], sevenBag[rnd]) = (sevenBag[rnd], sevenBag[i]);
            }

            for (var i = 0; i < sevenBag.Count; i++)
            {
                Bag.Add(sevenBag[i]);
            }
        }
    }
    
    public void GameOver()
    {
        tilemap.ClearAllTiles();
    }
    
    public void Set(Piece piece)
    {
        for (var i = 0; i < piece.cells.Length; i++)
        {
            var tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    
    public void Clear(Piece piece)
    {
        for (var i = 0; i < piece.cells.Length; i++)
        {
            var tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }
    
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;
        
        for (var i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
            
            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }
        return true;
    }
    
    public void ClearLines()
    {
        RectInt bounds = Bounds;
        var row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (var col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col,row,0);

            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (var col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col,row,0);
            tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (var col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col,row + 1,0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }

        score.s_line++;
    }
}