using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Setting : MonoBehaviour
{
    private KeyCode t_moveleft = KeyCode.LeftArrow;
    private KeyCode t_moveright = KeyCode.RightArrow;
    private KeyCode t_rotate = KeyCode.UpArrow;
    private KeyCode t_rotateback = KeyCode.LeftControl;
    private KeyCode t_hold = KeyCode.LeftShift;
    private KeyCode t_harddrop = KeyCode.Space;
    private KeyCode t_softdrop = KeyCode.DownArrow;
    
    private KeyCode t_moveleft2 = KeyCode.A;
    private KeyCode t_moveright2 = KeyCode.D;
    private KeyCode t_rotate2 = KeyCode.W;
    private KeyCode t_rotateback2 = KeyCode.RightControl;
    private KeyCode t_hold2 = KeyCode.RightShift;
    private KeyCode t_harddrop2 = KeyCode.R;
    private KeyCode t_softdrop2 = KeyCode.S;
    
    [HideInInspector]
    public KeyCode moveleft = KeyCode.LeftArrow;
    [HideInInspector]
    public KeyCode moveright = KeyCode.RightArrow;
    [HideInInspector]
    public KeyCode rotate = KeyCode.UpArrow;
    [HideInInspector]
    public KeyCode rotateback = KeyCode.LeftControl;
    [HideInInspector]
    public KeyCode hold = KeyCode.LeftShift;
    [HideInInspector]
    public KeyCode harddrop = KeyCode.Space;
    [HideInInspector]
    public KeyCode softdrop = KeyCode.DownArrow;
    
    [HideInInspector]
    public KeyCode moveleft2 = KeyCode.A;
    [HideInInspector]
    public KeyCode moveright2 = KeyCode.D;
    [HideInInspector]
    public KeyCode rotate2 = KeyCode.W;
    [HideInInspector]
    public KeyCode rotateback2 = KeyCode.RightControl;
    [HideInInspector]
    public KeyCode hold2 = KeyCode.RightShift;
    [HideInInspector]
    public KeyCode harddrop2 = KeyCode.R;
    [HideInInspector]
    public KeyCode softdrop2 = KeyCode.S;

    private float V_ARR = 0.5f;
    private float V_DAS = 8f;
    private float V_SDF = 40f;
    [HideInInspector]
    public UnityEvent<string, float> OnHandlingChanged;
    
    public float ARR
    {
        get => V_ARR;
        set
        {
            V_ARR = value;
            OnHandlingChanged?.Invoke("ARR", V_ARR);
        }
    }
    
    public float DAS
    {
        get => V_DAS;
        set
        {
            V_DAS = value;
            OnHandlingChanged?.Invoke("DAS", V_DAS);
        } 
    }
    
    public float SDF
    {
        get => V_SDF;
        set
        {
            V_SDF = value;
            OnHandlingChanged?.Invoke("SDF", V_SDF);
        }
    }
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    [HideInInspector]
    public bool isOutOnFocus;

    public Dictionary<string,KeyCode> ReturnDictionary()
    {
        return new Dictionary<string, KeyCode>
        {
            {"moveleft", moveleft},
            {"moveright", moveright},
            {"rotate", rotate},
            {"rotateback", rotateback},
            {"hold", hold},
            {"harddrop", harddrop},
            {"softdrop", softdrop},
            
            {"moveleft2", moveleft2},
            {"moveright2", moveright2},
            {"rotate2", rotate2},
            {"rotateback2", rotateback2},
            {"hold2", hold2},
            {"harddrop2", harddrop2},
            {"softdrop2", softdrop2}
        };
    }
    
    public Dictionary<string,KeyCode> ReturnTempDictionary()
    {
        return new Dictionary<string, KeyCode>
        {
            {"moveleft", t_moveleft},
            {"moveright", t_moveright},
            {"rotate", t_rotate},
            {"rotateback", t_rotateback},
            {"hold", t_hold},
            {"harddrop", t_harddrop},
            {"softdrop", t_softdrop},
            
            {"moveleft2", t_moveleft2},
            {"moveright2", t_moveright2},
            {"rotate2", t_rotate2},
            {"rotateback2", t_rotateback2},
            {"hold2", t_hold2},
            {"harddrop2", t_harddrop2},
            {"softdrop2", t_softdrop2}
        };
    }
    
    public void ResetAllKey()
    {
        moveleft = KeyCode.LeftArrow;
        moveright = KeyCode.RightArrow;
        rotate = KeyCode.UpArrow;
        rotateback = KeyCode.LeftControl;
        hold = KeyCode.LeftShift;
        harddrop = KeyCode.Space;
        softdrop = KeyCode.DownArrow;
        
        moveleft2 = KeyCode.A;
        moveright2 = KeyCode.D;
        rotate2 = KeyCode.W;
        rotateback2 = KeyCode.RightControl;
        hold2 = KeyCode.RightShift;
        harddrop2 = KeyCode.R;
        softdrop2 = KeyCode.S;
    }

    public void ResetAllHandling()
    {
        ARR = 0.5f;
        DAS = 8f;
        SDF = 40f;
    }
    
    
    public void T_SetKey(string key, KeyCode Code)
    {
        switch (key)
        {
            case "moveleft":
                t_moveleft = Code;
                break;
            case "moveright":
                t_moveright = Code; 
                break;
            case "rotate":
                t_rotate = Code;
                break;
            case "rotateback":
                t_rotateback = Code;
                break;
            case "hold":
                t_hold = Code;
                break;
            case "harddrop":
                t_harddrop = Code;
                break;
            case "softdrop":
                t_softdrop = Code;
                break;
            
            case "moveleft2":
                t_moveleft2 = Code;
                break;
            case "moveright2":
                t_moveright2 = Code;
                break;
            case "rotate2":
                t_rotate2 = Code;
                break;
            case "rotateback2":
                t_rotateback2 = Code;
                break;
            case "hold2":
                t_hold2 = Code;
                break;
            case "harddrop2":
                t_harddrop2 = Code;
                break;
            case "softdrop2":
                t_softdrop2 = Code;
                break;
        }
    }

    public void SetKey(string key, KeyCode Code)
    {
        switch (key)
        {
            case "moveleft":
                moveleft = Code;
                break;
            case "moveright":
                moveright = Code; 
                break;
            case "rotate":
                rotate = Code;
                break;
            case "rotateback":
                rotateback = Code;
                break;
            case "hold":
                hold = Code;
                break;
            case "harddrop":
                harddrop = Code;
                break;
            case "softdrop":
                softdrop = Code;
                break;
            
            case "moveleft2":
                moveleft2 = Code;
                break;
            case "moveright2":
                moveright2 = Code;
                break;
            case "rotate2":
                rotate2 = Code;
                break;
            case "rotateback2":
                rotateback2 = Code;
                break;
            case "hold2":
                hold2 = Code;
                break;
            case "harddrop2":
                harddrop2 = Code;
                break;
            case "softdrop2":
                softdrop2 = Code;
                break;
        }
    }
    
    public void SetHandling(string key, float value)
    {
        switch (key)
        {
            case "ARR":
                ARR = value;
                break;
            case "DAS":
                DAS = value;
                break;
            case "SDF":
                SDF = value;
                break;
        }
    }
    
    public KeyCode GetKeyCode(string gameObjectName)
    {
        return gameObjectName switch
        {
            "moveleft" => moveleft,
            "moveright" => moveright,
            "rotate" => rotate,
            "rotateback" => rotateback,
            "hold" => hold,
            "harddrop" => harddrop,
            "softdrop" => softdrop,
            
            "moveleft2" => moveleft2,
            "moveright2" => moveright2,
            "rotate2" => rotate2,
            "rotateback2" => rotateback2,
            "hold2" => hold2,
            "harddrop2" => harddrop2,
            "softdrop2" => softdrop2,
            _ => KeyCode.None
        };
    }
    
    public KeyCode GetTempKeyCode(string gameObjectName)
    {
        return gameObjectName switch
        {
            "moveleft" => t_moveleft,
            "moveright" => t_moveright,
            "rotate" => t_rotate,
            "rotateback" => t_rotateback,
            "hold" => t_hold,
            "harddrop" => t_harddrop,
            "softdrop" => t_softdrop,
            
            "moveleft2" => t_moveleft2,
            "moveright2" => t_moveright2,
            "rotate2" => t_rotate2,
            "rotateback2" => t_rotateback2,
            "hold2" => t_hold2,
            "harddrop2" => t_harddrop2,
            "softdrop2" => t_softdrop2,
            _ => KeyCode.None
        };
    }

    public float GetHandling(string gameObjectName)
    {
        return gameObjectName switch
        {
            "ARR" => ARR,
            "DAS" => DAS,
            "SDF" => SDF,
            _ => 0f
        };
    }
}
