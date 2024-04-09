using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderBind : MonoBehaviour
{
    private Setting setting;
    private KeyInput keyInput;
    private OnOff onoff;
    private Slider slider;
    private TMP_InputField value;
    
    private float tempValue;
    private void Awake()
    {
        setting = FindObjectOfType<Setting>();
        slider = GetComponent<Slider>();
        value = GetComponentInChildren<TMP_InputField>();
        onoff = FindObjectOfType<OnOff>();
        
        value.text = setting.GetHandling(gameObject.name).ToString();
        tempValue = setting.GetHandling(gameObject.name);
        slider.value = tempValue;
        value.text = tempValue.ToString();
        
        onoff.OnPanelOff.AddListener(Exited);
    }

    public void Exited()
    {
        setting.SetHandling(gameObject.name, tempValue);
    }

    public void OnChangedSlider()
    {
        tempValue = Mathf.RoundToInt(slider.value * 100f) / 100f;
        value.text = tempValue.ToString();
        
        if (tempValue > 40)
        {
            value.text = "inf";
        }
    }

    public void OnChangedInput()
    {
        if (value.text == "" || !float.TryParse(value.text, out var f)) return;

        if (f < slider.minValue)
        {
            tempValue = slider.minValue;
            value.text = tempValue.ToString();
            
        }
        else if (f > slider.maxValue)
        {
            tempValue = slider.maxValue;
            value.text = tempValue.ToString();
        }
        else
        {
            tempValue = Mathf.RoundToInt(f * 100f) / 100f;
        }

        if (tempValue > 40)
        {
            value.text = "inf";
        }
        
        slider.value = tempValue;
    }
}
