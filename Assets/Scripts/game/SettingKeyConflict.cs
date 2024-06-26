using System.Collections.Generic;
using UnityEngine;

public class SettingKeyConflict : MonoBehaviour
{
    private List<SettingKeybind> keybinds = new();
    private Setting setting;
    
    private void Start()
    {
        setting = FindObjectOfType<Setting>();
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void ResetConflict()
    {
        keybinds.Clear();
        keybinds.AddRange(FindObjectsOfType<SettingKeybind>());
        foreach (var key in keybinds)
        {
            if (key.text.color == Color.red)
            {
                setting.T_SetKey(key.gameObject.name, key.tempString == "None" ? KeyCode.None : (KeyCode) System.Enum.Parse(typeof(KeyCode), key.tempString));
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
                    foreach (var keybinds in keybinds)
                    {
                        if (keybinds.tempString == key.Value.ToString())
                        {
                            keybinds.text.color = Color.red;
                            setting.T_SetKey(keybinds.gameObject.name, KeyCode.None);
                        }
                    }
                }
                else if (key.Value == KeyCode.None)
                {
                    foreach (var keybinds in keybinds)
                    {
                        if (keybinds.tempString == key.Value.ToString())
                        {
                            keybinds.text.color = Color.yellow;
                            setting.T_SetKey(keybinds.gameObject.name, KeyCode.None);
                        }
                    }
                }
            }
        }
    }
}
