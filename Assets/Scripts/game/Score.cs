using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private Piece piece;
    [SerializeField] private Board board;
    
    [Space]
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text combo;
    
    public int s_line;
    public bool s_rotate;
    public bool s_pclear;

    public bool b2b;
    private int V_com = -1;
    public int com
    {
        get => V_com;
        set
        {
            V_com = value;
            combo.text = $"{value}";
        }
    }

    private int V_score;
    public int s_score
    {
        get => V_score;
        set
        {
            V_score = value;
            score.text = $"{value}";
        }
    }
    public int a_score;

    private int cnt;

    public void GameOver()
    { 
        s_line = 0;
        s_rotate = false;
        s_pclear = false;

        b2b = false;
        com = -1;

        s_score = 0; 
        a_score = 0;

    }

    public void Set()
    {
        if (s_pclear)
        {
            a_score = 10;
            b2b = false;
            s_score += a_score * 10;
            //Debug.Log($"{a_score} {b2b} dmg");
            a_score = 0;
            return;
        }
        
        if (piece.data.tetromino == Tetromino.T && s_rotate)
        {
            if (cnt >= 3)
            {
                switch (s_line)
                {
                    case 1:
                        a_score += 2;
                        break;
                    case 2:
                        a_score += 4;
                        break;
                    case 3:
                        a_score += 6;
                        break;
                }
                if (b2b)
                {
                    a_score += 1;
                }
                
                b2b = true;
            }
            else
            {
                if (b2b)
                {
                    a_score += 1;
                }
                a_score += 0;
                b2b = true;
            }
        }
        else
        {
            switch (s_line)
            {
                case 1:
                    a_score += 0;
                    b2b = false;
                    break;
                case 2:
                    a_score += 1;
                    b2b = false;
                    break;
                case 3:
                    a_score += 2;
                    b2b = false;
                    break;
                case 4:
                    a_score += 4;
                    if (b2b)
                    {
                        a_score += 1;
                    }
                    b2b = true;
                    break;
            }
        }

        if (s_line > 0)
        {
            com++;
        }
        else
        {
            com = -1;
        }

        switch (com)
        {
            case 0:
            case 1:
                a_score += 0;
                break;
            case 2:
            case 3:
                a_score += 1;
                break;
            case 4:
            case 5:
                a_score += 2;
                break;
            case 6:
            case 7:
                a_score += 3;
                break;
            case 8:
            case 9:
            case 10:
                a_score += 4;
                break;
            case >= 11:
                a_score += 5;
                break;
        }
        
        s_score += a_score * 10;
        //Debug.Log($"{a_score} {b2b} dmg");
        a_score = 0;
    }

    public void TestT()
    { 
        cnt = 0;
        if (board.tilemap.HasTile(piece.position + new Vector3Int(1,1,0)))
        {
            cnt++;
        }
        if (board.tilemap.HasTile(piece.position + new Vector3Int(1,-1,0)))
        {
            cnt++;
        }
        if (board.tilemap.HasTile(piece.position + new Vector3Int(-1,1,0)))
        {
            cnt++;
        }
        if (board.tilemap.HasTile(piece.position + new Vector3Int(-1,-1,0)))
        {
            cnt++;
        }
    }

    public void OnAttack(int dmg)
    {
        
    }

    public void OnDamaged(int dmg)
    {
        
    }
}
