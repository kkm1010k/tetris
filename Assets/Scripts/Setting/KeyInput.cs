using TMPro;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    public KeyCode code = KeyCode.None;

    private TextMeshProUGUI tmp;

    private Setting setting;
    private void Start()
    {
        setting = FindObjectOfType<Setting>();
        tmp = gameObject.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        
        Show(false);
    }
    
    public void Show(bool isActive, string obj = "")
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(isActive);
        }

        if (isActive)
        {
            tmp.text = $"now : <{setting.GetKeyCode(obj).ToString()}>";
        }
    }
    
    public void Reset()
    {
        code = KeyCode.None;
        tmp.text = code.ToString();
    }
    
    private void OnGUI()
    {
        Event e = Event.current;
        if (!e.isKey) return;
        code = e.keyCode;
    }
}