using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Keybind : MonoBehaviour
{
    private Setting setting;
    private KeyInput keyInput;
    private Button button;
    public TextMeshProUGUI text;
    private KeyConflict keyConflict;
    private Transform popUp;
    private OnOff onoff;
    public string tempString;
    private void Awake()
    {
        setting = FindObjectOfType<Setting>();
        button = GetComponent<Button>();
        popUp = FindObjectOfType<GridChanger>().transform.GetChild(2);
        keyInput = popUp.GetComponentInChildren<KeyInput>();
        keyConflict = popUp.GetComponentInChildren<KeyConflict>();
        onoff = FindObjectOfType<OnOff>();
        
        onoff.OnPanelOff.AddListener(Exited);
    }

    public void Exited()
    {
        setting.SetKey(gameObject.name,setting.GetTempKeyCode(gameObject.name));
    }
    
    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        tempString = setting.GetKeyCode(gameObject.name).ToString();
        SwitchString(setting.GetKeyCode(gameObject.name));
        keyConflict.ResetConflict();
        keyConflict.CheckConflict();
    }

    public void Clicked()
    {
        StartCoroutine(ClickedCoroutine());
    }

    public void Changed()
    {
        Debug.Log("e");
    }
    
    private IEnumerator ClickedCoroutine()
    {
        button.interactable = false;
        keyInput.Reset();
        keyInput.Show(true, gameObject.name);
        while (true)
        {
            yield return null;
            switch (keyInput.code)
            {
                case KeyCode.Escape:
                    keyInput.Show(false);
                    keyInput.code = KeyCode.None;
                    button.interactable = true;
                    yield break;
                case KeyCode.Backspace:
                    keyInput.Show(false);
                    tempString = "None";
                    text.text = "None";
                    keyInput.code = KeyCode.None;
                    setting.T_SetKey(gameObject.name, KeyCode.None);
                    keyConflict.ResetConflict();
                    keyConflict.CheckConflict();
                    button.interactable = true;
                    yield break;
                case KeyCode.None:
                    continue;
            }
            
            break;
        }
        
        keyInput.Show(false);
        tempString = keyInput.code.ToString();
        SwitchString(keyInput.code);
        setting.T_SetKey(gameObject.name, keyInput.code);
        keyConflict.ResetConflict();
        keyConflict.CheckConflict();
        keyInput.code = KeyCode.None;
        button.interactable = true;
    }

    public void ChangeColor(Color color)
    {
        text.color = color;
    }

    private void SwitchString(KeyCode code)
    {
        switch (code)
        {
            case KeyCode.LeftArrow:
                text.text = "\u2190";
                text.fontSize = 40;
                break;
            case KeyCode.RightArrow:
                text.text = "\u2192";
                text.fontSize = 40;
                break;
            case KeyCode.UpArrow:
                text.text = "\u2191";
                text.fontSize = 40;
                break;
            case KeyCode.DownArrow:
                text.text = "\u2193";
                text.fontSize = 40;
                break;
            case KeyCode.LeftControl:
                text.text = "LCtrl";
                text.fontSize = 25;
                break;
            case KeyCode.RightControl:
                text.text = "RCtrl";
                text.fontSize = 25;
                break;
            case KeyCode.LeftShift:
                text.text = "LShift";
                text.fontSize = 25;
                break;
            case KeyCode.RightShift:
                text.text = "RShift";
                text.fontSize = 25;
                break;
            default:
                text.text = code.ToString();
                text.fontSize = 25;
                break;
        }
    }
}
