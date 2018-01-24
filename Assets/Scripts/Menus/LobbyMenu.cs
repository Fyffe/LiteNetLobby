using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenu : MenuRoot
{
    public static LobbyMenu instance;
    
    public List<LobbyPlayer> Players = new List<LobbyPlayer>();

    public Transform playersList;

    public LobbyPlayer playerPrefab;

    public Text lobbyName;

    public GameObject hostControls;
    public GameObject clientControls;

    public bool isHost { get; protected set; }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void CreateLobby(string name)
    {
        lobbyName.text = name;

        ClearPlayersList();
        ToggleThisMenu(true);

        isHost = true;
        HandleHostControls();
    }

    public void OpenLobby(string name)
    {
        lobbyName.text = name;

        ClearPlayersList();
        ToggleThisMenu(true);

        isHost = false;
        HandleHostControls();
    }

    void HandleHostControls()
    {
        hostControls.SetActive(isHost);
        clientControls.SetActive(!isHost);
    }

    public void CreatePlayer(LiteNetLib.NetPeer peer, string name, bool self)
    {
        LobbyPlayer p = Instantiate(playerPrefab, playersList, false);

        p.Init();
        p.SetPlayer(peer, name, self, Players.Count);

        Players.Add(p);
    }

    public void CreatePlayer(int id, string name)
    {
        LobbyPlayer p = Instantiate(playerPrefab, playersList, false);

        p.Init();
        p.SetPlayer(id, name);

        Players.Add(p);
    }

    void ClearPlayersList()
    {
        for(int i = 0; i < playersList.childCount; i++)
        {
            Destroy(playersList.GetChild(i).gameObject);
        }

        Players.Clear();
    }

    public void Disconnect()
    {
        if(NetworkServer.instance)
        {
            NetworkServer.instance.StopServer();
        }

        if(NetworkClient.instance)
        {
            NetworkClient.instance.StopClient();
        }

        ClearPlayersList();
        ToggleThisMenu(false);

        MainMenu.instance.OpenMenu(0);
        MainMenu.instance.ToggleThisMenu(true);
    }

    public void RemovePlayer(LobbyPlayer player)
    {
        if(Players.Contains(player))
        {
            GameObject go = player.gameObject;

            Players.Remove(player);
            Destroy(go);
        }
    }

    public LobbyPlayer GetPeer(LiteNetLib.NetPeer peer)
    {
        LobbyPlayer player = null;

        for(int i = 0; i < Players.Count; i++)
        {
            LobbyPlayer lp = Players[i];

            if(lp.peer == peer)
            {
                player = lp;
            }
        }

        return player;
    }

    public LobbyPlayer GetPeer(int id)
    {
        LobbyPlayer player = null;

        for (int i = 0; i < Players.Count; i++)
        {
            LobbyPlayer lp = Players[i];

            if (lp.id == id)
            {
                player = lp;
            }
        }

        return player;
    }
}
