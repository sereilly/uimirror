using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPage : MonoBehaviour
{
    [SerializeField]
    protected Slider backgroundSlider;
    [SerializeField]
    protected Toggle vibrateToggle;

    protected void Start()
    {
        backgroundSlider.value = AppSettings.Instance.Background;
        vibrateToggle.isOn = AppSettings.Instance.Vibrate;
    }

    public void SliderChanged()
    {
        float background = backgroundSlider.value;
        AppSettings.Instance.Background = background;

    }

    public void VibrateToggled()
    {
        AppSettings.Instance.Vibrate = vibrateToggle.isOn;
    }
}
