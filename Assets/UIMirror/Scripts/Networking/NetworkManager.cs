using System.IO;
using UnityEngine;
using System.Runtime.Serialization;

public class NetworkManager : MonoBehaviour
{
    public delegate void MessageEvent(byte[] message);
    public delegate void ClientConnectionEvent(int connectionId);
    public delegate void ServerConnectionEvent();

    public event MessageEvent ServerMessage;
    public event MessageEvent ClientMessage;
    public event ClientConnectionEvent ClientConnected;
    public event ClientConnectionEvent ClientDisconnected;
    public event ServerConnectionEvent ServerConnected;
    public event ServerConnectionEvent ServerDisconnected;


    Telepathy.Client client = new Telepathy.Client();
    Telepathy.Server server = new Telepathy.Server();

    public static NetworkManager Instance;
    public bool ShowGui;
    public bool AutoStart = true;

    [SerializeField]
    protected int Port = 7952;

    private bool clientConnected;

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // update even if window isn't focused, otherwise we don't receive.
            Application.runInBackground = true;

            // use Debug.Log functions for Telepathy so we can see it in the console
            Telepathy.Logger.Log = Debug.Log;
            Telepathy.Logger.LogWarning = Debug.LogWarning;
            Telepathy.Logger.LogError = Debug.LogError;

            if (AutoStart)
            {
                StartServer();
            }
        }
        else
        {
            Destroy(this);
        }
    }

    protected void Update()
    {
        // client
        if (client.Connected)
        {
            clientConnected = true;
            Telepathy.Message msg;
            while (client.GetNextMessage(out msg))
            {
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        ServerConnected?.Invoke();
                        break;
                    case Telepathy.EventType.Data:
                        ServerMessage?.Invoke(msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        OnServerDisconnect();
                        break;
                }
            }
        }
        else if (clientConnected)
        {
            // Server must have disconnected ungracefully
            OnServerDisconnect();
        }

        // server
        if (server.Active)
        {
            Telepathy.Message msg;
            while (server.GetNextMessage(out msg))
            {
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        ClientConnected?.Invoke(msg.connectionId);
                        break;
                    case Telepathy.EventType.Data:
                        ClientMessage?.Invoke(msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        ClientDisconnected?.Invoke(msg.connectionId);
                        break;
                }
            }
        }
    }

    private void OnServerDisconnect()
    {
        ServerDisconnected?.Invoke();
        clientConnected = false;
    }

    protected void OnGUI()
    {
        if (ShowGui)
        {
            // client
            GUI.enabled = !client.Connected;
            if (GUI.Button(new Rect(0, 0, 120, 20), "Connect Client"))
                ConnectToServer("localhost");

            GUI.enabled = client.Connected;
            if (GUI.Button(new Rect(130, 0, 120, 20), "Disconnect Client"))
                client.Disconnect();

            // server
            GUI.enabled = !server.Active;
            if (GUI.Button(new Rect(0, 25, 120, 20), "Start Server"))
                StartServer();

            GUI.enabled = server.Active;
            if (GUI.Button(new Rect(130, 25, 120, 20), "Stop Server"))
                server.Stop();

            GUI.enabled = true;
        }
    }

    protected void OnApplicationQuit()
    {
        client.Disconnect();
        server.Stop();
    }

    public void StartServer()
    {
        server.Start(Port);
    }

    public void SendMessage(int connectionId, byte[] message)
    {
        server.Send(connectionId, message);
    }

    public void SendMessageToServer(byte[] message)
    {
        client.Send(message);
    }

    public void ConnectToServer(string ip)
    {
        client.Connect(ip, Port);
    }
}