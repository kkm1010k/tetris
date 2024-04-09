using UnityEngine;
using UnityEngine.Events;

public class OnOff : MonoBehaviour
{
    private Setting settingController;
    private GameObject setting;
    private GameObject panel;
    
    public UnityEvent OnPanelOff;
    
    private void Start()
    {
        settingController = FindObjectOfType<Setting>();
        setting = GameObject.FindWithTag("setting");
        panel = setting.transform.GetChild(0).gameObject;
    }

    public void On()
    {
        settingController.isOutOnFocus = true;
        panel.SetActive(true);
    }
    
    public void Off()
    {
        settingController.isOutOnFocus = false;
        OnPanelOff.Invoke();
        panel.SetActive(false);
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