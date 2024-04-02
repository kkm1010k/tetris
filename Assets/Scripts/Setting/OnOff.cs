using UnityEngine;

public class OnOff : MonoBehaviour
{
    private Setting settingController;
    private GameObject setting;
    private GameObject panel;
    
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
        panel.SetActive(false);
    }
}