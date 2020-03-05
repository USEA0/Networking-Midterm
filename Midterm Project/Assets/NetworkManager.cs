using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    public enum PacketType
    {
        //initialization connection
        INIT_CONNECTION,
        //single string
        MESSAGE,
        //player's position udp
        PLAYER_POSITION,
        //puck position for non-physics player udp
        PUCK_POSITIONS,
        //puck impulse for physics player tcp
        PUCK_IMPULSE,
        //info about game win and loss tcp
        GAMEWIN,
    }

    #region Netcode

    //net code
    [DllImport("CNET.dll")]
    static extern IntPtr CreateClient();                            //Creates a client
    [DllImport("CNET.dll")]
    static extern void DeleteClient(IntPtr client);                 //Destroys a client
    [DllImport("CNET.dll")]
    static extern void Connect(string ip, IntPtr client);          //Connects to c++ Server
    [DllImport("CNET.dll")]
    static extern void SendData(int type, string str, bool useTCP, IntPtr client);          //Sends Message to all other clients    
    [DllImport("CNET.dll")]
    static extern void StartUpdating(IntPtr client);                //Starts updating
    [DllImport("CNET.dll")]
    static extern int GetPlayerNumber(IntPtr client);
    [DllImport("CNET.dll")]
    static extern void SetupMessage(Action<int, int, string> action); //recieve packets from server

    [DllImport("CNET.dll")]
    static extern void SetupOnConnect(Action action); //recieve packets from server

    //pointer to the client dll
    private static IntPtr Client;

    //ip of the server
    public string ip;

    //local player number
    public static int playerNumber = -1;

    //connected status
    public static bool connected = false;


    #endregion



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
