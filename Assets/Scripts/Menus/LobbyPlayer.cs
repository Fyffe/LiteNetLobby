using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour
{
    public int id;
    public string playerName;

    public Text playerNameTxt;
    public Button kickBtn;

    public LiteNetLib.NetPeer peer;

    public void Init()
    {
        playerNameTxt = transform.Find("Name").GetComponentInChildren<Text>();
        kickBtn = transform.Find("Kick").GetComponentInChildren<Button>();
    }

    public void SetPlayer(int id, string name)
    {
        this.id = id;

        playerName = name;
        playerNameTxt.text = name;

        kickBtn.gameObject.SetActive(false);
    }

    public void SetPlayer(LiteNetLib.NetPeer p, string name, bool b, int id)
    {
        peer = p;
        playerName = name;
        playerNameTxt.text = name;

        this.id = id;

        kickBtn.gameObject.SetActive(!b);
    }
}
