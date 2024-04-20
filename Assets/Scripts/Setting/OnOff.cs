using UnityEngine;
using UnityEngine.Events;

public class OnOff : MonoBehaviour
{
    private Setting settingController;
    private GameObject setting;
    private GameObject panel;
    
    [HideInInspector]
    public UnityEvent OnPanelOff;
    
    private void Start()
    {
        settingController = FindObjectOfType<Setting>();
        setting = GameObject.FindWithTag("setting");
    }

    public void On()
    {
        
    }
    
    public void Off()
    {
        OnPanelOff.Invoke();
    }

    public void Toggle()
    {
        if (panel.activeSelf)
        {
            Off();
        }
        else
        {
            On();
        }
    }
}