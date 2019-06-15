using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
#if NETFX_CORE
using Windows.Networking;
using Windows.Networking.Connectivity;
using System.Linq;
#endif

public class MachineBroadcaster : MonoBehaviour
{
    [SerializeField]
    protected float broadcastInterval = 3.0f;
    [SerializeField]
    protected int port = 15000;

    private const string opcode = "UIM";

    private UdpClient client;
    private byte[] udpMessage;
    private IPEndPoint ip;
    private WaitForSeconds broadcastWait;

    protected void OnEnable()
    {
        client = new UdpClient();
        ip = new IPEndPoint(IPAddress.Broadcast, port);
        broadcastWait = new WaitForSeconds(broadcastInterval);

        if (udpMessage == null)
        {
            InitMessage();
        }
        StartCoroutine(BroadcastIpCo());
    }

    protected void OnDisable()
    {
#if NETFX_CORE
        client.Dispose();
#else
        client.Close();
#endif
    }

    private void InitMessage()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter br = new BinaryWriter(ms))
            {
                br.Write(opcode);
                br.Write(GetMachineName());
                udpMessage = ms.ToArray();
            }
        }
    }

    private string GetMachineName()
    {
#if NETFX_CORE
        var hostNames = NetworkInformation.GetHostNames();
        var hostName = hostNames.FirstOrDefault(name => name.Type == HostNameType.DomainName)?.DisplayName ?? "???";
        return hostName;
#else
        return Environment.MachineName;
#endif
    }

    private IEnumerator BroadcastIpCo()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
#if NETFX_CORE
            client.SendAsync(udpMessage, udpMessage.Length, ip);
#else
            client.Send(udpMessage, udpMessage.Length, ip);
#endif
            yield return broadcastWait;
        }
    }
}
