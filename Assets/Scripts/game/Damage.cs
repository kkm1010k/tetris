using UnityEngine;
using UnityEngine.Tilemaps;

public class Damage : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile damageTile;
    private Score score;

    private int V_damage;

    public int damageCount
    {
        get => V_damage;
        set
        {
            V_damage = value > 20 ? 20 : value;
            DamageCountChanged();
        }
    }
    
    private void Awake()
    {
        score = FindObjectOfType<Score>();
    }

    private void DamageCountChanged()
    {
        tilemap.ClearAllTiles();
        
        for (var i = 0; i < damageCount; i++)
        {
            var position = new Vector3Int(-11, i - 10, 0);
            tilemap.SetTile(position, damageTile);
        }
    }
}
