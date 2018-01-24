using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostMenu : MonoBehaviour
{
    public InputField serverNameIpt;
    public InputField portIpt;

    public GameObject serverPrefab;

    void Start()
    {
        if(!serverPrefab)
        {
            serverPrefab = Resources.Load<GameObject>("Prefabs/Networking/Server");
        }
    }

    public void OnHostPressed()
    {
        bool valid = true;

        string sName = serverNameIpt.text.Trim();

        if (sName.Length <= 0)
        {
            valid = false;
        }

        string portStr = portIpt.text.Trim();
        int port = -1;
        
        if (!int.TryParse(portStr, out port))
        {
            valid = false;
        }

        if(valid)
        {
            StartServer(sName, port);
        }
    }

    void StartServer(string name, int port)
    {
        MainMenu.instance.ToggleThisMenu(false);
        NetworkServer server = Instantiate(serverPrefab, Vector3.zero, Quaternion.identity).GetComponent<NetworkServer>();

        server.name = serverPrefab.name;
        server.StartServer(name, port);
    }
}
