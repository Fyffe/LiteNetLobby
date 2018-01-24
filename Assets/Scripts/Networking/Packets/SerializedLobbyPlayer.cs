[System.Serializable]
public class SerializedLobbyPlayer
{
    public string name;
    public int id;

    public static void Serialize(LiteNetLib.Utils.NetDataWriter writer, SerializedLobbyPlayer mytype)
    {
        writer.Put(mytype.name);
        writer.Put(mytype.id);
    }

    public static SerializedLobbyPlayer Deserialize(LiteNetLib.Utils.NetDataReader reader)
    {
        SerializedLobbyPlayer res = new SerializedLobbyPlayer();

        res.name = reader.GetString();
        res.id = reader.GetInt();

        return res;
    }
}
