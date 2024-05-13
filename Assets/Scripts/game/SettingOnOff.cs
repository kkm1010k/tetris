using UnityEngine;
using UnityEngine.Events;

public class SettingOnOff : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    
    [HideInInspector]
    public UnityEvent OnPanelOff;
    
    public void On()
    {
        obj.SetActive(true);
    }
    
    public void Off()
    {
        OnPanelOff.Invoke();
        obj.SetActive(false);
    }
    
    public void Toggle()
    {
        if (obj.activeSelf)
        {
            Off();
        }
        else
        {
            On();
        }
    }
}
