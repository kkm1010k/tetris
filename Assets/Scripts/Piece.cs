using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public Hold hold;
    public Score score;
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position;
    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float moveDelay = 0.1f;
    public float lockDelay = 0.5f;
    
    private float stepTime;
    private float moveTime;
    private float lockTime;

    public bool holdcnt;
    public bool Isholded;

    public void Initialize(Board board ,Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        
        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;
        
        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (var i = 0; i < data.cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        board.Clear(this);

        lockTime += Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Rotate(-1);
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if (!holdcnt)
            { 
                hold.Holding(this); 
                board.SpawnPiece(holdcnt, Isholded);
                holdcnt = true;
                Isholded = true;
                return;
            }
        }
        
        if (Time.time > moveTime) {
            HandleMoveInputs();
        }
        
        if (Time.time >= stepTime)
        {
            Step();
        }
                
        
        board.Set(this);
    }
    
    private void HandleMoveInputs()
    {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (Move(Vector2Int.down)) {
                stepTime = Time.time + stepDelay;
            }
        }
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
        }
    }

    public void GameOver()
    {
        holdcnt = false;
        Isholded = false;
        board.Bag.Clear();
        board.sevenBag.Clear();
        board.Reload();
        board.SpawnPiece();
        
    }
    
    private void Step()
    {
        stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        if (lockTime >= lockDelay)
        {
            Lock();
        }
    }

    private void Lock()
    {
        if (data.tetromino == Tetromino.T && score.s_rotate)
        {
            score.TestT();
        }
        board.Set(this);
        board.ClearLines();

        if (board.tilemap.GetTilesRangeCount(new Vector3Int(-5,-10,0), new Vector3Int(5,12,0)) == 0)
        {
            score.s_pclear = true;
        }
        
        holdcnt = false;
        
        score.Set();
        score.s_rotate = false;
        score.s_line = 0;
        
        board.SpawnPiece();
    }
    
    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            // :)
        }
        Lock();
    }
    
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        var valid = board.IsValidPosition(this, newPosition);

        if (valid)
        {
            position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f;
            score.s_rotate = false;
            
        }
        
        return valid;
    }

    private void Rotate(int direction)
    {
        var originalRotation = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);
        
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }

        if (originalRotation != rotationIndex)
        {
            score.s_rotate = true;
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        for (var i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x *Data.RotationMatrix[0] * direction) +
                                        (cell.y * Data.RotationMatrix[1] * direction));
                    
                    y = Mathf.CeilToInt((cell.x *Data.RotationMatrix[2] * direction) +
                                        (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                case Tetromino.T:
                case Tetromino.J:
                case Tetromino.L:
                case Tetromino.S:
                case Tetromino.Z:
                default:
                    x = Mathf.RoundToInt((cell.x *Data.RotationMatrix[0] * direction) +
                                         (cell.y * Data.RotationMatrix[1] * direction));
                    
                    y = Mathf.RoundToInt((cell.x *Data.RotationMatrix[2] * direction) +
                                         (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }
    
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        var wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (var i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];
            if (Move(translation))
            {
                return true;
            }
            
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        var wallkickIndex = 0;
        if (rotationIndex == 1 && rotationDirection == 1) wallkickIndex = 0;
        else if (rotationIndex == 0 && rotationDirection == -1) wallkickIndex = 1;
        else if (rotationIndex == 2 && rotationDirection == 1) wallkickIndex = 2;
        else if (rotationIndex == 1 && rotationDirection == -1) wallkickIndex = 3;
        else if (rotationIndex == 3 && rotationDirection == 1) wallkickIndex = 4;
        else if (rotationIndex == 2 && rotationDirection == -1) wallkickIndex = 5;
        else if (rotationIndex == 0 && rotationDirection == 1) wallkickIndex = 6;
        else if (rotationIndex == 3 && rotationDirection == -1) wallkickIndex = 7;
        
        // var wallkickIndex = rotationIndex * 2;
        // if (rotationDirection < 0)
        // {
        //     wallkickIndex--;
        // }
        
        return Wrap(wallkickIndex, 0, data.wallKicks.GetLength(0));
    }
    
    private int Wrap(int input, int min, int max)
    {
        if (input < min) {
            return max - (min - input) % (max - min);
        } else {
            return min + (input - min) % (max - min);
        }
    }
}
