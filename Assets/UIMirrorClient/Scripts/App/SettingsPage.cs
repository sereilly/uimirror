using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPage : MonoBehaviour
{
    [SerializeField]
    protected Slider backgroundSlider;

    protected void Start()
    {
        backgroundSlider.value = AppSettings.Instance.Background;
    }

    public void SliderChanged()
    {
        float background = backgroundSlider.value;
        AppSettings.Instance.Background = background;

    }
}
