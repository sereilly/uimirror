using Microsoft.MixedReality.SpectatorView;
using UnityEngine;

public class UIMirrorClient : MonoBehaviour
{
    [SerializeField]
    protected UIMirrorDestination destinationCanvas;
    [SerializeField]
    protected Socketer socketerClient;

    private byte[] canvasData;

    protected void Start()
    {
        socketerClient.Connected += Instance_ServerConnected;
        socketerClient.Message += Instance_Message;
    }

    private void Instance_ServerConnected(Socketer socketer, MessageEvent e)
    {
        destinationCanvas.Clear();
        canvasData = new byte[0];
    }

    private void Instance_Message(Socketer socketer, MessageEvent messageEvent)
    {
        byte[] message = messageEvent.Message;
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

    public void SendMessageToServer(byte[] message)
    {
        socketerClient.SendNetworkMessage(message);
    }
}
