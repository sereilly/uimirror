using TMPro;
using UnityEngine;

public class AppConsole : MonoBehaviour
{
    [SerializeField]
    protected Transform textbox;
    [SerializeField]
    protected TextMeshProUGUI consoleTextPrefab;

    public static AppConsole Instance;

    private const int maxMessages = 50;

    protected void Awake()
    {
        Instance = this;
        foreach (Transform child in textbox)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < maxMessages; i++)
        {
            TextMeshProUGUI consoleText = Instantiate(consoleTextPrefab);
            consoleText.transform.SetParent(textbox, false);
            consoleText.text = string.Empty;
        }
    }

    public void Log(string message, LogType type)
    {
        string colorString;
        switch (type)
        {
            case LogType.Log:
                colorString = "white";
                break;
            case LogType.Warning:
                colorString = "yellow";
                break;
            default:
                colorString = "red";
                break;
        }

        string coloredMessage = string.Format("<color=\"{0}\">{1}</color>", colorString, message);

        var consoleText = textbox.GetChild(0);
        consoleText.GetComponent<TextMeshProUGUI>().text = coloredMessage;
        consoleText.transform.SetAsLastSibling();

    }
}
