using System;
using System.IO;
using System.Runtime.Serialization;

public enum InputMessageType
{
    Button,
    Slider
}

[Serializable]
public struct InputMessage
{
    public InputMessage(InputMessageType type, int id, float value = 0)
    {
        this.type = type;
        this.id = id;
        this.value = value;
    }

    public InputMessageType type;
    public int id;
    public float value;

    public byte[] Serialize()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(InputMessage));
            ser.WriteObject(ms, this);
            return ms.ToArray();
        }
    }

    public static InputMessage Deserialize(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream(data))
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(InputMessage));
            InputMessage inputMessage = (InputMessage)ser.ReadObject(ms);
            return inputMessage;
        }
    }
}


