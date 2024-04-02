using System.Collections.Generic;
using UnityEngine;

public class KeyConflict : MonoBehaviour
{
    private List<Keybind> keybind = new();
    private Setting setting;
    
    private void Start()
    {
        keybind.AddRange(FindObjectsOfType<Keybind>());
        setting = FindObjectOfType<Setting>();
    }
    
    public void ResetConflict()
    {
        foreach (var key in keybind)
        {
            if (key.text.color == Color.red)
            {
                setting.SetKey(key.gameObject.name, setting.GetTempKeyCode(key.gameObject.name));
            }
            key.text.color = Color.black;
        }
    }
    
    
    //만약 키가 중복된다면 중복되는 키를 setting으로 보내지 않음
    public void CheckConflict()
    {
        var keyDictionary = setting.ReturnTempDictionary();
        foreach (var key in keyDictionary)
        {
            foreach (var key2 in keyDictionary)
            {
                if (key.Key == key2.Key) continue;
                if (key.Value == key2.Value)
                {
                    foreach (var keybinds in keybind)
                    {
                        if (keybinds.text.text == key.Value.ToString())
                        {
                            keybinds.text.color = Color.red;
                            setting.SetKey(keybinds.gameObject.name, KeyCode.None);
                        }
                    }
                }
                else if (key.Value == KeyCode.None)
                {
                    Debug.Log("e");
                    foreach (var keybinds in keybind)
                    {
                        Debug.Log(keybinds.text.text);
                        Debug.Log(key.Value.ToString());
                        if (keybinds.text.text == key.Value.ToString())
                        {
                            keybinds.text.color = Color.yellow;
                            setting.SetKey(keybinds.gameObject.name, KeyCode.None);
                        }
                    }
                }
            }
        }
    }
}