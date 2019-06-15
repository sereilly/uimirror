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

    private void UpdateBackground()
    {
        float background = Background;
        Color color = new Color(background, background, background);
        Camera.main.backgroundColor = color;
        backgroundImage.color = color;
    }
}
