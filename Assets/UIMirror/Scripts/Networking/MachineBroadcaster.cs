using Microsoft.MixedReality.SpectatorView;
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
    protected Socketer socketer;

    private const string opcode = "UIM";

    private byte[] udpMessage;
    private WaitForSeconds broadcastWait;

    protected void OnEnable()
    {
        broadcastWait = new WaitForSeconds(broadcastInterval);

        if (udpMessage == null)
        {
            InitMessage();
            socketer.StartClient();
        }
        StartCoroutine(BroadcastIpCo());
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
            socketer.SendNetworkMessage(udpMessage);
            yield return broadcastWait;
        }
    }
}
