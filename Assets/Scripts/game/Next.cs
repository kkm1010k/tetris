using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Next : MonoBehaviour
{
    private Tilemap tilemap;
    private Vector3Int[] cells;
    public TetrominoData[] tetrominos;
    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        tilemap.ClearAllTiles();
        for (var i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    public void Set(List<int> Bag)
    {
        tilemap.ClearAllTiles();
        
        if (cells == null)
        {
            cells = new Vector3Int[4];
        }
        
        for (var i = 0; i < 5; i++)
        {
            Vector3Int position = new Vector3Int(-1,10-i*4,0);
            
            for (var j = 0; j < cells.Length; j++)
            {
                cells[j] = (Vector3Int)tetrominos[Bag[i]].cells[j];
            }
            
            for (var j = 0; j < cells.Length; j++)
            {
                var tilePosition = cells[j] + position;
                tilemap.SetTile(tilePosition, tetrominos[Bag[i]].tile);
            }
        }
    }
}