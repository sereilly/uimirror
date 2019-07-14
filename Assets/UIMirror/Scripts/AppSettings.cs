using UnityEngine;
using UnityEngine.UI;

public class AppSettings : MonoBehaviour
{
    public static AppSettings Instance;

    [SerializeField]
    protected Image backgroundImage;

    protected void Awake()
    {
        UpdateBackground();
        Instance = this;
    }

    public float Background
    {
        get
        {
            return PlayerPrefs.GetFloat("background", 0.5f);
        }

        set
        {
            PlayerPrefs.SetFloat("background", value);
            UpdateBackground();
        }
    }

    public bool Vibrate
    {
        get
        {
            return PlayerPrefs.GetInt("Vibrate", 1) != 0;
        }

        set
        {
            PlayerPrefs.SetInt("Vibrate", value ? 1 : 0);
        }
    }

    private void UpdateBackground()
    {
        float background = Background;
        Color color = new Color(background, background, background);
        Camera.main.backgroundColor = color;
        backgroundImage.color = color;
    }
}
