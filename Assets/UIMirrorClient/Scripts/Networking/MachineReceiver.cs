using Microsoft.MixedReality.SpectatorView;
using System.IO;
using UnityEngine;

public class MachineReceiver : MonoBehaviour
{
    [SerializeField]
    protected Socketer socketer;

    public delegate void MachineFoundEvent(string machineName, string ip);
    public event MachineFoundEvent MachineFound;

    private const string opcode = "UIM";

    protected void Awake()
    {
        socketer.StartServer();
        socketer.Message += Socketer_Message;
    }

    private void Socketer_Message(Socketer arg1, MessageEvent messageEvent)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream(messageEvent.Message))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    string op = br.ReadString();
                    if (op == opcode)
                    {
                        string machineName = br.ReadString();
                        MachineFound?.Invoke(machineName, messageEvent.SourceHost);
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
