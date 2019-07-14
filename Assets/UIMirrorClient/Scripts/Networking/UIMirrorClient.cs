using UnityEngine;

public class UIMirrorClient : MonoBehaviour
{
    [SerializeField]
    protected UIMirrorDestination destinationCanvas;

    private byte[] canvasData;

    protected void Start()
    {
        NetworkManager.Instance.ServerMessage += Instance_Message;
        NetworkManager.Instance.ServerConnected += Instance_ServerConnected;
    }

    private void Instance_ServerConnected()
    {
        destinationCanvas.Clear();
        canvasData = new byte[0];
    }

    private void Instance_Message(byte[] message)
    {
        if (message.Length >= 4 && message[0] == 'C' && message[1] == 'O' && message[2] == 'N')
        {
            LogType messageType = (LogType)message[3];
            string consoleMessage = System.Text.Encoding.UTF8.GetString(message, 4, message.Length - 4);
            AppConsole.Instance.Log(consoleMessage, messageType);
        }
        else
        {
            canvasData = Fossil.Delta.Apply(canvasData, message);
            GameObject root = destinationCanvas.Deserialize(canvasData);
            CenterCanvas(root);
        }
    }

    private void CenterCanvas(GameObject root)
    {
        RectTransform rectTransform = root.gameObject.GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localScale = Vector3.one;
    }
}
