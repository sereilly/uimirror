using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UIMirrorServer : MonoBehaviour
{
    [SerializeField]
    protected UIMirrorSource sourceCanvas;

    private CancellationTokenSource tokenSource;

    public class Connection
    {
        public byte[] canvasData = new byte[0];
    }

    public Dictionary<int, Connection> connections = new Dictionary<int, Connection>();
    private ConcurrentQueue<byte[]> canvasDataQueue = new ConcurrentQueue<byte[]>();
    private bool needsUpdate = true;

    protected void Start()
    {
        NetworkManager.Instance.ClientConnected += Instance_ClientConnected;
        NetworkManager.Instance.ClientDisconnected += Instance_ClientDisconnected;
        NetworkManager.Instance.ClientMessage += Instance_Message;
        Application.logMessageReceived += Application_logMessageReceived;

        tokenSource = new CancellationTokenSource();
        CancellationToken ct = tokenSource.Token;
        Task.Run(() => sourceCanvas.SerializeTask(canvasDataQueue, ct), ct);
    }

    private void Application_logMessageReceived(string message, string stackTrace, LogType type)
    {
        if (NetworkManager.Instance)
        {
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            byte[] messageBuffer = new byte[messageBytes.Length + 4];
            messageBuffer[0] = (byte)'C';
            messageBuffer[1] = (byte)'O';
            messageBuffer[2] = (byte)'N';
            messageBuffer[3] = (byte)type;
            Array.Copy(messageBytes, 0, messageBuffer, 4, messageBytes.Length);

            foreach (int connectionId in connections.Keys)
            {
                NetworkManager.Instance.SendMessage(connectionId, messageBuffer);
            }
        }
    }

    protected void OnDisable()
    {
        tokenSource.Cancel();
    }

    private void Instance_Message(byte[] message)
    {
        InputMessage inputMessage = InputMessage.Deserialize(message);
        switch (inputMessage.type)
        {
            case InputMessageType.Button:
                sourceCanvas.InvokeButton(inputMessage.id);
                break;
            case InputMessageType.Slider:
                sourceCanvas.MoveSlider(inputMessage.id, inputMessage.value);
                break;
        }        
    }

    private void Instance_ClientConnected(int connectionId)
    {
        Connection connection = new Connection();
        connections[connectionId] = connection;
    }

    private void Instance_ClientDisconnected(int connectionId)
    {
        connections.Remove(connectionId);
    }

    protected void LateUpdate()
    {
        if (needsUpdate)
        {
            needsUpdate = false;
            sourceCanvas.Serialize(canvasDataQueue);
        }

        if (canvasDataQueue.TryDequeue(out byte[] canvasData))
        {
            foreach (KeyValuePair<int, Connection> connectionKvp in connections)
            {
                int connectionId = connectionKvp.Key;
                Connection connection = connectionKvp.Value;
                byte[] connectionData = connection.canvasData;
                connection.canvasData = canvasData;
                Task.Run(() =>
                {
                    byte[] delta = Fossil.Delta.Create(connectionData, canvasData);
                    if (delta.Length > 17) // ignore checksum data
                    {
                        NetworkManager.Instance.SendMessage(connectionId, delta);
                    }
                });
            }
            needsUpdate = true;
        }       
    }
}
