using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System;

public class NetworkClient : MonoBehaviour, INetEventListener
{
    public static NetworkClient instance;

    NetManager netClient;
    NetPeer host;
    NetSerializer serializer;

    string clientName;
    int discPort;

    bool started;
    bool isInit = false;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Init()
    {
        serializer = new NetSerializer();
        serializer.RegisterCustomType(SerializedLobbyPlayer.Serialize, SerializedLobbyPlayer.Deserialize);
        
        serializer.SubscribeReusable<PLobby, NetPeer>(OnReceiveLobby);
        serializer.SubscribeReusable<PLobbyPlayer>(OnClientLobbyAction);

        isInit = true;
    }

    public void StartClient(string name, int port)
    {
        if (!isInit)
        {
            Init();
        }

        clientName = name;
        discPort = port;
        
        netClient = new NetManager(this, "cg3");
        netClient.Start();
        netClient.UpdateTime = 15;

        started = true;
    }

    public void StopClient()
    {
        netClient.Stop();
        Destroy(gameObject);

        started = false;
    }

    void Update()
    {
        if (!started)
        {
            return;
        }

        netClient.PollEvents();

        if (host == null)
        {
            host = netClient.GetFirstPeer();
        }

        if (host != null)
        {
            if (host.ConnectionState == ConnectionState.Connected)
            {
                    
            }
        }
        else
        {
            netClient.SendDiscoveryRequest(new byte[] { 1 }, discPort);
        }
    }

    void OnDestroy()
    {
        if (netClient != null)
            netClient.Stop();
    }

    public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
    {
        Debug.Log("Received error " + socketErrorCode);
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        serializer.ReadAllPackets(reader, peer);
    }

    public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.DiscoveryResponse && netClient.PeersCount == 0)
        {
            Debug.Log("Received discovery response. Connecting to: " + remoteEndPoint);
            netClient.Connect(remoteEndPoint);
        }
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("Connected to " + peer.EndPoint);

        string cName = clientName;

        PClientHandshake packet = new PClientHandshake();
        packet.clientName = cName;
        
        host.Send(serializer.Serialize(packet), SendOptions.ReliableOrdered);
    }

    public void OnReceiveLobby(PLobby packet, NetPeer peer)
    {
        LobbyMenu.instance.OpenLobby(packet.serverName);

        for(int i = 0; i < packet.players.Length; i++)
        {
            LobbyMenu.instance.CreatePlayer(packet.players[i].id, packet.players[i].name);
        }
    }

    public void OnClientLobbyAction(PLobbyPlayer packet)
    {
        if (packet.action)
        {
            LobbyMenu.instance.CreatePlayer(packet.id, packet.name);
        }
        else
        {
            LobbyPlayer p = LobbyMenu.instance.GetPeer(packet.id);
            LobbyMenu.instance.RemovePlayer(p);
        }
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("Disconnected because " + disconnectInfo.Reason);
        LobbyMenu.instance.Disconnect();
    }
}
