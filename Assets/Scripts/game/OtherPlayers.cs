using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OtherPlayers : MonoBehaviour
{
    private Transform[] objs;
    public TetrominoData[] tetrominos;
    
    private void Awake()
    {
        objs = transform.Cast<Transform>().ToArray();
    }

    public void StartGameInit(IReadOnlyList<ulong> ids)
    {
        var cnt = 0;
        foreach (var id in ids)
        {
            if (id == NetworkManager.Singleton.LocalClientId)
            {
                continue;
            }
            objs[cnt].gameObject.SetActive(true);
            objs[cnt].name = id.ToString();
            cnt++;
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void SetTile(short[] tilemap, ulong id)
    {
        var currTilemap = objs.First(x => x.name == id.ToString()).GetComponentInChildren<Tilemap>();
        
        var width = 10;
        var height = 23;
        var tilemap2d = new short[height, width];
        
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                tilemap2d[i, j] = tilemap[i + j * height];
            }
        }
        
        for (var i = 0; i < tilemap2d.GetLength(0); i++)
        {
            for (var j = 0; j < tilemap2d.GetLength(1); j++)
            {
                var pos = new Vector3Int(j, i, 0) - new Vector3Int(width, height-2, 0) / 2;
                
                switch (tilemap2d[i, j])
                {
                    case 0:
                        currTilemap.SetTile(pos, tetrominos[0].tile);
                        break;
                    case 1:
                        currTilemap.SetTile(pos, tetrominos[1].tile);
                        break;
                    case 2:
                        currTilemap.SetTile(pos, tetrominos[2].tile);
                        break;
                    case 3:
                        currTilemap.SetTile(pos, tetrominos[3].tile);
                        break;
                    case 4:
                        currTilemap.SetTile(pos, tetrominos[4].tile);
                        break;
                    case 5:
                        currTilemap.SetTile(pos, tetrominos[5].tile);
                        break;
                    case 6:
                        currTilemap.SetTile(pos, tetrominos[6].tile);
                        break;
                    case -1:
                        currTilemap.SetTile(pos, null);
                        break;
                }
            }
        }
    }
}
