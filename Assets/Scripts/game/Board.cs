using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    private Next next;
    private Hold hold;
    private Score score;
    private Damage damage;
    
    public TetrominoData[] tetrominos;
    [SerializeField] private Tile nullTile;
    public Vector3Int spawnPosition;
    public Vector3Int spawnPositionadd;
    public Vector2Int boardSize = new(10, 20);
    public List<int> sevenBag { get; } = new();
    public List<int> Bag { get; } = new();
    
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize + new Vector2Int(0,3));
        }
    }

    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    
    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponent<Piece>();
        hold = FindObjectOfType<Hold>();
        next = FindObjectOfType<Next>();
        score = FindObjectOfType<Score>();
        damage = GetComponent<Damage>();
        
        for (var i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    public void StartGameInit(float time, int seed)
    {
        Random.InitState(seed);
        StartCoroutine(ShowCoroutine());
        StartCoroutine(StartGame(time));
    }

    private IEnumerator StartGame(float time)
    {
        while (time > NetworkManager.Singleton.ServerTime.TimeAsFloat)
        {
            text.text = $"Game Start in {time - NetworkManager.Singleton.ServerTime.TimeAsFloat}";
            yield return null;
        }
        text.text = "Game Start!";
        SpawnPiece();
        yield return new WaitForSeconds(1);
        text.text = "";
    }

    private IEnumerator ShowCoroutine()
    {
        var startTime = Time.time;
        image.color = new Color(56/255f, 56/255f, 56/255f, 1);
        while (startTime + 2 > Time.time)
        {
            var seq = (Time.time - startTime) / 2f * 1f;
            image.color = new Color(56/255f, 56/255f, 56/255f, 1 - seq);
            yield return null;
        }
        image.color = new Color(1, 1, 1, 0);
        yield return null;
    }
    
    public void SpawnPiece(bool holdcnt = false, bool Isholded = false)
    {
        Reload();
        TetrominoData data = new TetrominoData();
        if (!Isholded && !holdcnt)
        {
            data = tetrominos[Bag[0]];
            Bag.RemoveAt(0);
        }
        else
        {
            data = hold.tetrominobefore;
        }

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
        score.GameOver();
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

        if (score.s_line > 0)
        {
            return;
        }
        
        if (damage.damageCount > 0)
        {
            LineFill(damage.damageCount);
            damage.damageCount = 0;
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
    
    public void LineFill(int n)
    {
        RectInt bounds = Bounds;
        
        for (var row = bounds.yMax - 1; row >= bounds.yMin; row--)
        {
            for (var col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col,row,0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row + n, 0);
                tilemap.SetTile(position, above);
            }
            
            for (var col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col,row,0);
                tilemap.SetTile(position, null);
            }
        }

        for (var i = 0; i < n; i++)
        {
            var rnd = Random.Range(bounds.xMin, bounds.xMax + 1);
            
            for (var col = bounds.xMin; col < bounds.xMax; col++)
            {
                if (col == rnd)
                {
                    continue;
                }
                Vector3Int position = new Vector3Int(col,bounds.yMin + i,0);
                tilemap.SetTile(position, nullTile);
            }
        }
    }

    public short[] GetTilemapArray()
    {
        var bounds = Bounds;
        var tilemapArray = new short[bounds.size.x,bounds.size.y];

        for (var i = 0; i < tilemapArray.GetLength(0); i++)
        {
            for (var j = 0; j < tilemapArray.GetLength(1); j++)
            {
                tilemapArray[i, j] = -1;
            }
        }

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                Tile tile = tilemap.GetTile<Tile>(pos);
                if (tile == tetrominos[0].tile)
                {
                    tilemapArray[pos.x + bounds.size.x / 2, pos.y + bounds.size.y / 2 - 1] = 0;
                }
                else if (tile == tetrominos[1].tile)
                {
                    tilemapArray[pos.x + bounds.size.x / 2, pos.y + bounds.size.y / 2 - 1] = 1;
                }
                else if (tile == tetrominos[2].tile)
                {
                    tilemapArray[pos.x + bounds.size.x / 2, pos.y + bounds.size.y / 2 - 1] = 2;
                }
                else if (tile == tetrominos[3].tile)
                {
                    tilemapArray[pos.x + bounds.size.x / 2, pos.y + bounds.size.y / 2 - 1] = 3;
                }
                else if (tile == tetrominos[4].tile)
                {
                    tilemapArray[pos.x + bounds.size.x / 2, pos.y + bounds.size.y / 2 - 1] = 4;
                }
                else if (tile == tetrominos[5].tile)
                {
                    tilemapArray[pos.x + bounds.size.x / 2, pos.y + bounds.size.y / 2 - 1] = 5;
                }
                else if (tile == tetrominos[6].tile)
                {
                    tilemapArray[pos.x + bounds.size.x / 2, pos.y + bounds.size.y / 2 - 1] = 6;
                }
                else
                {
                    tilemapArray[pos.x + bounds.size.x / 2, pos.y + bounds.size.y / 2 - 1] = -1;
                }
            }
        }
        
        var tilemap1d = new short[bounds.size.x * bounds.size.y];
        
        for (var i = 0; i < bounds.size.x; i++)
        {
            for (var j = 0; j < bounds.size.y; j++)
            {
                tilemap1d[i * bounds.size.y + j] = tilemapArray[i, j];
            }
        }

        return tilemap1d;
    }
}