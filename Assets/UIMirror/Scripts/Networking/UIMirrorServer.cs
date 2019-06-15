using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class UIMirrorServer : MonoBehaviour
{
    [SerializeField]
    protected UIMirrorSource sourceCanvas;

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

        Task.Run(() => sourceCanvas.SerializeTask(canvasDataQueue));
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
