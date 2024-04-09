using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Keybind : MonoBehaviour
{
    private Setting setting;
    private KeyInput keyInput;
    private Button button;
    [HideInInspector]
    public TextMeshProUGUI text;
    private KeyConflict keyConflict;
    private void Start()
    {
        setting = FindObjectOfType<Setting>();
        keyInput = FindObjectOfType<KeyInput>();
        keyConflict = FindObjectOfType<KeyConflict>();
        button = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = setting.GetKeyCode(gameObject.name).ToString();
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
        text.text = keyInput.code.ToString();
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
}
