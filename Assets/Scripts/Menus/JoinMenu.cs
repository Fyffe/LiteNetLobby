using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinMenu : MonoBehaviour
{
    public InputField playerNameIpt;
    public InputField ipAddressIpt;
    public InputField portIpt;

    public GameObject clientPrefab;

    void Start()
    {
        if(!clientPrefab)
        {
            clientPrefab = Resources.Load<GameObject>("Prefabs/Networking/Client");
        }
    }

    public void OnJoinPressed()
    {
        bool valid = true;

        string pName = playerNameIpt.text.Trim();

        if(pName.Length <= 0)
        {
            valid = false;
        }

        string ipAddr = ipAddressIpt.text.Trim();

        if(ipAddr.Length <= 0)
        {
            valid = false;
        }

        string portStr = portIpt.text.Trim();
        int port = -1;

        int.TryParse(portStr, out port);

        if(port == -1)
        {
            valid = false;
        }

        if(valid)
        {
            StartClient(pName, ipAddr, port);
        }
    }

    void StartClient(string name, string address, int port)
    {
        MainMenu.instance.ToggleThisMenu(false);
        NetworkClient client = Instantiate(clientPrefab, Vector3.zero, Quaternion.identity).GetComponent<NetworkClient>();

        client.name = clientPrefab.name;
        client.StartClient(name, port);
    }
}
