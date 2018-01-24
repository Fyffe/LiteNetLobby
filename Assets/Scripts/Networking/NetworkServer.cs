using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class NetworkServer : MonoBehaviour, INetEventListener
{
    public static NetworkServer instance;

    NetManager netServer;
    NetPeer mPeer;
    NetSerializer serializer;

    string serverName;
    int serverPort;

    public List<NetPeer> Peers = new List<NetPeer>();

    bool started;
    bool isInit = false;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        if(!started)
        {
            return;
        }

        netServer.PollEvents();
    }

    void Init()
    {
        serializer = new NetSerializer();
        serializer.RegisterCustomType(SerializedLobbyPlayer.Serialize, SerializedLobbyPlayer.Deserialize);

        serializer.SubscribeReusable<PClientHandshake, NetPeer>(OnReceiveHandshake);

        isInit = true;
    }

    public void StartServer(string name, int port)
    {
        if (!isInit)
        {
            Init();
        }

        serverName = name;
        serverPort = port;

        netServer = new NetManager(this, 100, "cg3");
        netServer.Start(serverPort);
        netServer.DiscoveryEnabled = true;
        netServer.UpdateTime = 15;
        
        started = true;

        LobbyMenu.instance.CreateLobby(name);
        LobbyMenu.instance.CreatePlayer(mPeer, "Host", true);
    }

    public void StopServer()
    {
        netServer.Stop();
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (Peers.Count > 0)
        {

        }
    }

    void OnDestroy()
    {
        if (netServer != null)
        {
            netServer.Stop();
        }
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("New peer connected: " + peer.EndPoint);
        if(!Peers.Contains(peer))
        {
            Peers.Add(peer);
        }
    }

    public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
    {
        Debug.Log("Server error! " + socketErrorCode);
    }

    public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType msg)
    {
        if(msg == UnconnectedMessageType.DiscoveryRequest)
        {
            Debug.Log("Received discovery request! Sending discovery response!");
            netServer.SendDiscoveryResponse(new byte[] { 1 }, remoteEndPoint);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {

    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        if(Peers.Contains(peer))
        {
            Peers.Remove(peer);
        }

        LobbyPlayer lp = LobbyMenu.instance.GetPeer(peer);

        if(lp)
        {
            LobbyMenu.instance.RemovePlayer(lp);
        }

        PLobbyPlayer packet = new PLobbyPlayer();
        packet.id = lp.id;
        packet.name = lp.name;
        packet.action = false;

        for(int i = 0; i < Peers.Count; i++)
        {
            Peers[i].Send(serializer.Serialize<PLobbyPlayer>(packet), SendOptions.ReliableOrdered);
        }

        Debug.Log("Peer disconnected, reason: " + disconnectInfo.Reason.ToString());
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        serializer.ReadAllPackets(reader, peer);
    }

    public void OnReceiveHandshake(PClientHandshake packet, NetPeer peer)
    {
        int id = LobbyMenu.instance.Players.Count;
        LobbyMenu.instance.CreatePlayer(peer, packet.clientName, false);

        PLobby lobbyPacket = new PLobby();
        List<SerializedLobbyPlayer> S = new List<SerializedLobbyPlayer>();
        
        for(int i = 0; i < LobbyMenu.instance.Players.Count; i++)
        {
            LobbyPlayer p = LobbyMenu.instance.Players[i];
            SerializedLobbyPlayer slp = new SerializedLobbyPlayer();
            slp.id = p.id;
            slp.name = p.playerName;

            S.Add(slp);
        }

        lobbyPacket.serverName = serverName;
        lobbyPacket.players = S.ToArray();
        
        peer.Send(serializer.Serialize(lobbyPacket), SendOptions.ReliableOrdered);

        PLobbyPlayer lp = new PLobbyPlayer();
        lp.id = id;
        lp.name = packet.clientName;
        lp.action = true;

        for (int i = 0; i < Peers.Count; i++)
        {
            if (Peers[i] != peer)
            {
                Peers[i].Send(serializer.Serialize(lp), SendOptions.ReliableOrdered);
            }
        }
    }
}
