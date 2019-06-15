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
        canvasData = Fossil.Delta.Apply(canvasData, message);
        destinationCanvas.Deserialize(canvasData);
    }
}
