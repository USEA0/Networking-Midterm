/*
Names:
John Wang - 100657681
Boris Au - 100660279
*/
using UnityEngine;
using UnityEngine.UI;

public class Connect : MonoBehaviour
{
    public InputField ip;

    //connect to server
    public void connectServer()
    {
        NetworkManager.ConnectToServer(ip.text);
    }
}
