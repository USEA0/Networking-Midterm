using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Connect : MonoBehaviour
{
    public InputField ip;

    public void connectServer()
    {
        NetworkManager.ConnectToServer(ip.text);
    }
}
