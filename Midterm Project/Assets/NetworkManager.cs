/*
Names:
John Wang - 100657681
Boris Au - 100660279
*/
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
        //player's data udp
        PLAYER,
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
    static extern void SendData(int type, string msg, bool useTCP, IntPtr client);          //Sends Message to all other clients    
    [DllImport("CNET.dll")]
    static extern void StartUpdating(IntPtr client);                //Starts updating
    [DllImport("CNET.dll")]
    static extern int GetPlayerNumber(IntPtr client);
    [DllImport("CNET.dll")]
    static extern void SetupPacketReception(Action<int, int, string> action); //recieve packets from server

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

    //flag to trigger connected event
    public static bool connectFlag = false;

    //queue for storing messages
    public static Queue<string> MessageQueue = new Queue<string>();

    public Player[] allPlayers;
    public Player controllingPlayer;

    public GameObject canvas;


    //tick timestep
    int fixedTimeStep = 0;
    //tick delay
    public int tickDelay = 1;

    // Start is called before the first frame update
    void Awake()
    {
        Client = CreateClient();
        SetupPacketReception(PacketRecieved);
        SetupOnConnect(OnConnect);
    }


    //this will be called if the connection is successful
    static void OnConnect()
    {
        connected = true;
        playerNumber = GetPlayerNumber(Client);
        connectFlag = true;

        Debug.Log("Connected");
    }

    //TODO call this to connect to server 
    static public void ConnectToServer(string ip)
    {
        Connect(ip, Client);
        StartUpdating(Client);
    }

    private void Update()
    {
        //set update lag delays
        if (Input.GetKey(KeyCode.Alpha1)) {
            tickDelay--;

            if (tickDelay < 1) {
                tickDelay = 1;
            }
            Debug.Log("Lag is " + tickDelay.ToString() + " Frames.");
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            tickDelay++;
            Debug.Log("Lag is " + tickDelay.ToString() + " Frames.");
        }
    }

    //update loop
    private void FixedUpdate()
    {
        #region Fixed Tick
        if (connected)
        {
            ++fixedTimeStep;
        }
        if (fixedTimeStep >= tickDelay)
        {
            TickUpdate();
            fixedTimeStep = 0;
        }
        #endregion
        //process incomming packet updates here
        lock (MessageQueue)
        {
            if (MessageQueue.Count > 0)
            {
                Debug.Log(MessageQueue.Dequeue());
            }
        }

        ////update position
        //if (positionUpdated && notPlayerPaddle != null) {
        //    notPlayerPaddle.transform.position = positionIncomming;
        //}
        ////update position
        //if (impulseUpdated)
        //{
        //    puck.I.Enqueue(puckImpulse);
        //    impulseUpdated = false;
        //}


        if (connectFlag)
        {
            ////start game here
            //if (playerNumber == 0)
            //{
            //    paddles[0].isPlayer = true;
            //    paddles[0].playerNum = 1;
            //    playerPaddle = paddles[0];
            //    notPlayerPaddle = paddles[1];
            //}
            //else {
            //    paddles[1].isPlayer = true;
            //    paddles[0].playerNum = 2;
            //    playerPaddle = paddles[1];
            //    notPlayerPaddle = paddles[0];
            //}
            canvas.SetActive(false);

            connectFlag = false;
        }

    }

    //process all things here
    void TickUpdate()
    {
        //if (playerPaddle != null)
        //{
        //    StringBuilder position = new StringBuilder();
        //    position.Append(playerPaddle.transform.position.x);
        //    position.Append(",");
        //    position.Append(playerPaddle.transform.position.y);
        

        //SendData((int)PacketType.PLAYER_POSITION, position.ToString(), false, Client);
        //}
    }

    //recieve all input data
    static void PacketRecieved(int type, int sender, string data)
    {
        //Debug.Log(data);
        data.TrimEnd();


        //parse the data
        string[] parsedData = data.Split(',');

        switch ((PacketType)type)
        {
            case PacketType.MESSAGE:
                if (parsedData.Length == 2)
                {
                    Debug.Log("Message:" + data);

                    StringBuilder newString = new StringBuilder();

                    newString.Append(parsedData[1]);
                    newString.Append(": ");
                    newString.Append(parsedData[0]);

                    //send to queue
                    MessageQueue.Enqueue(newString.ToString());
                }
                else
                {
                    Debug.LogWarning("Packet: MESSAGE Length is invalid");
                }
                break;
            case PacketType.PLAYER:
                if (parsedData.Length == 4)
                {
                    Debug.Log("Player Position:" + data);

                    //update paddle position
                    //positionIncomming = new Vector2(float.Parse(parsedData[0]), float.Parse(parsedData[1]));
                    //positionUpdated = true;
                }
                else {
                    Debug.LogWarning("Packet: PLAYER_POSITION Length is invalid");
                }
                break;

        }
    }

    //call c++ cleanup
    private void OnDestroy()
    {
        //clean up client
        DeleteClient(Client);

    }

}