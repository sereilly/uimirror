using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
#if NETFX_CORE
using System.Threading.Tasks;
#endif
using UnityEngine;

public class MachineReceiver : MonoBehaviour
{
    [SerializeField]
    protected int port = 15000;

    public delegate void MachineFoundEvent(string machineName, IPEndPoint ip);
    public event MachineFoundEvent MachineFound;

    private const string opcode = "UIM";
    private UdpClient udp;
    private IPEndPoint ip;

    protected void OnEnable()
    {
        udp = new UdpClient();
        udp.ExclusiveAddressUse = false;
        udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        ip = new IPEndPoint(IPAddress.Any, port);
        udp.Client.Bind(ip);
#if NETFX_CORE
        
        udp.ReceiveAsync().ContinueWith(Receive);
#else
        udp.BeginReceive(Receive, new object());
#endif
    }

    protected void OnDisable()
    {
#if NETFX_CORE
        udp.Dispose();
#else
        udp.Close();
#endif
    }

#if NETFX_CORE
        private void Receive(Task<UdpReceiveResult> task)
        {
            OnMessage(task.Result.Buffer);
            udp.ReceiveAsync().ContinueWith(Receive);
        }
#else
    private void Receive(IAsyncResult ar)
    {
        try
        {
            byte[] bytes = udp.EndReceive(ar, ref ip);
            OnMessage(bytes);
            udp.BeginReceive(Receive, new object());
        }
        catch (ObjectDisposedException)
        {
            // ignore ObjectDisposedException from EndReceive
        }

    }
#endif

    private void OnMessage(byte[] bytes)
    {
        try
        { 
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    string op = br.ReadString();
                    if (op == opcode)
                    {
                        string machineName = br.ReadString();
                        MachineFound?.Invoke(machineName, ip);
                    }
                }
            }
        }
        catch (EndOfStreamException)
        {
            // Something went wrong parsing the message. Fail silently and continue listening.
        }
    }
}
