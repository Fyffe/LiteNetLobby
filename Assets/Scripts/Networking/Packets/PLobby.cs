using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PLobby
{
    public SerializedLobbyPlayer[] players { get; set; }
    public string serverName { get; set; }
}
