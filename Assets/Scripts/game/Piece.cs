using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    private Hold hold;
    private Score score;
    private Setting setting;
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position;
    public int rotationIndex { get; private set; }

    private float stepDelay = 1f;
    private float softDelay = 0.1f;
    private float moveDelay = 0.1f;
    private float lockDelay = 0.5f;
    private float autoDelay = 0.15f;

    private float leftAutoTime;
    private float rightAutoTime;
    private float downAutoTime;
    
    private float stepTime;
    private float softTime;
    private float moveTime;
    private float lockTime;

    private bool holdcnt;
    private bool Isholded;

    private void Awake()
    {
        setting = FindObjectOfType<Setting>();
        hold = FindObjectOfType<Hold>();
        score = FindObjectOfType<Score>();

        moveDelay = setting.ARR;
        autoDelay = setting.DAS;
        softDelay = setting.SDF;
        
        setting.OnHandlingChanged.AddListener(OnHandling);
    }

    private void OnHandling(string str, float f)
    {
        switch (str)
        {
            case "ARR":
                moveDelay = f;
                break;
            case "DAS":
                autoDelay = f;
                break;
            case "SDF":
                softDelay = f;
                break;
        }
    }
    
    public void Initialize(Board board ,Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        
        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        softTime = Time.time + 1/softDelay;
        moveTime = Time.time + moveDelay * Time.deltaTime;
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
        if (!board)
        {
            return;
        }
        
        board.Clear(this);

        lockTime += Time.deltaTime;
        
        if (!setting.isOutOnFocus)
        {
            KeyMoveInputs();
            
            HandleMoveInputs();
        }
        
        if (Time.time >= stepTime)
        {
            Step();
        }
        
        board.Set(this);

    }

    private void KeyMoveInputs()
    {
        if (Input.GetKeyDown(setting.rotate) || Input.GetKeyDown(setting.rotate2))
        {
            Rotate(1);
        }

        if (Input.GetKeyDown(setting.rotateback) || Input.GetKeyDown(setting.rotateback2))
        {
            Rotate(-1);
        }

        if (Input.GetKeyDown(setting.harddrop))
        {
            HardDrop();
            return;
        }

        if (Input.GetKeyDown(setting.hold) || Input.GetKeyDown(setting.hold2))
        {
            if (!holdcnt)
            {
                hold.Holding(this);
                board.SpawnPiece(holdcnt, Isholded);
                holdcnt = true;
                Isholded = true;
            }
        }
    }
    
    private void HandleMoveInputs()
    {
        if (leftAutoTime >= autoDelay * Time.deltaTime)
        {
            if (moveDelay == 0)
            {
                while (Move(Vector2Int.left))
                {
                    // Do nothing
                }
            }
            else if (Time.time > moveTime)
            {
                Move(Vector2Int.left);
            }
        }
        else if (rightAutoTime >= autoDelay * Time.deltaTime)
        {
            if (moveDelay == 0)
            {
                while (Move(Vector2Int.right))
                {
                    // Do nothing
                }
            }
            else if (Time.time > moveTime)
            {
                Move(Vector2Int.right);
            }    
            
        }
        
        
        if (Input.GetKey(setting.softdrop) || Input.GetKey(setting.softdrop2))
        {
            if (softDelay > 40)
            {
                while (Move(Vector2Int.down))
                {
                    // Do nothing
                }
                
                stepTime = Time.time + stepDelay;
            }
            else if (Time.time > softTime)
            {
                if (Move(Vector2Int.down))
                {
                    stepTime = Time.time + stepDelay;
                    softTime = Time.time + 1/softDelay;
                }
            }
        }
        
        if (Input.GetKey(setting.moveleft) || Input.GetKey(setting.moveleft2))
        {
            if (leftAutoTime == 0)
            {
                Move(Vector2Int.left);
            }
            rightAutoTime = 0f;
            leftAutoTime += Time.deltaTime;
        }
        else if (Input.GetKey(setting.moveright) || Input.GetKey(setting.moveright2))
        {
            if (rightAutoTime == 0)
            {
                Move(Vector2Int.right);
            }
            leftAutoTime = 0f;
            rightAutoTime += Time.deltaTime;

        }
        else
        {
            leftAutoTime = 0f;
            rightAutoTime = 0f;
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
            // Do nothing
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
            moveTime = Time.time + moveDelay * Time.deltaTime;
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
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) +
                                        (cell.y * Data.RotationMatrix[1] * direction));
                    
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) +
                                        (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                case Tetromino.T:
                case Tetromino.J:
                case Tetromino.L:
                case Tetromino.S:
                case Tetromino.Z:
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) +
                                         (cell.y * Data.RotationMatrix[1] * direction));
                    
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) +
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
