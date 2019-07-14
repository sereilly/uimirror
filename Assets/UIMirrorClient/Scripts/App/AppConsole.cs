using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AppConsole : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI textbox;
    public static AppConsole Instance;

    public LinkedList<string> lines = new LinkedList<string>();
    private const int maxMessages = 30;

    protected void Awake()
    {
        Instance = this;
        textbox.text = string.Empty;
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
        lines.AddFirst(coloredMessage);

        if (lines.Count > maxMessages)
        {
            lines.RemoveLast();
        }

        textbox.text = string.Join(Environment.NewLine, lines);
    }
}
