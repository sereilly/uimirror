using System;
using Microsoft.MixedReality.SpectatorView;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    protected Socketer socketer;

    public bool AutoStart;
    public Action ServerDisconnected;
    public Action ServerConnected;

    //public static NetworkManager Instance { get; private set; }
    protected void Awake()
    {
        //if (Instance == null)
        //{
        //    Instance = this;
            Application.runInBackground = true;

            if (AutoStart)
            {
                StartServer();
            }
        //}
        //else
        //{
        //    Destroy(this);
        //}

        if (socketer && socketer.Direction == SocketerClient.ProtocolDirection.Sender)
        {
            socketer.Connected += OnConnected;
            socketer.Disconnected += OnDisconnected;
        }
    }

    public void OnConnected(Socketer s, MessageEvent e)
    {
        ServerConnected?.Invoke();
    }

    public void OnDisconnected(Socketer s, MessageEvent e)
    {
        ServerDisconnected?.Invoke();
    }

    public void OnDestroy()
    {
        if (socketer)
        {
            socketer.Connected -= OnConnected;
            socketer.Disconnected -= OnDisconnected;
        }
    }

    public void StartServer()
    {
        socketer.StartServer();
    }

    public void SendMessageToServer(byte[] message)
    {
        socketer.SendNetworkMessage(message);
    }

    public void ConnectToServer(string address)
    {
        socketer.Host = address;
        socketer.StartClient();
    }
}