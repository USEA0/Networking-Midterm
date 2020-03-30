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
using System.Collections;

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

    //quue for storing player data
    public static Queue<string[]> DataQueue = new Queue<string[]>();


    //max 99 players
    public Player[] allPlayers = new Player[99];
    public Player controllingPlayer;
    public GameObject playerObjInstance;
    public GameObject playerObj;

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

        if (connectFlag) {
            //init at random position
            playerObj = Instantiate(playerObjInstance, new Vector2(UnityEngine.Random.Range(-Player.boundaries.x, Player.boundaries.x), UnityEngine.Random.Range(-Player.boundaries.y, Player.boundaries.y)), Quaternion.identity);
            controllingPlayer = playerObj.GetComponent<Player>();
            controllingPlayer.isControllable = true;

            //add to list
            allPlayers[playerNumber] = controllingPlayer;

            Debug.Log("Created User");

            //send to server
            SendUpdate();

            canvas.SetActive(false);
            connectFlag = false;
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

        while (DataQueue.Count > 0) {
            string[] data = DataQueue.Dequeue();

            if (allPlayers[int.Parse(data[0])] != null)
            {
                allPlayers[int.Parse(data[0])].px = float.Parse(data[1]);
                allPlayers[int.Parse(data[0])].py = float.Parse(data[2]);

                allPlayers[int.Parse(data[0])].vx = float.Parse(data[3]);
                allPlayers[int.Parse(data[0])].vy = float.Parse(data[4]);
            }
            else {
                playerObj = Instantiate(playerObjInstance, new Vector2(float.Parse(data[1]), float.Parse(data[2])), Quaternion.identity);

                //add to list
                allPlayers[int.Parse(data[0])] = playerObj.GetComponent<Player>();
                allPlayers[int.Parse(data[0])].vx = float.Parse(data[3]);
                allPlayers[int.Parse(data[0])].vy = float.Parse(data[4]);

            }

            allPlayers[int.Parse(data[0])].updated = true;
        }
    }

    void SendUpdate() {

        StringBuilder data = new StringBuilder();
        data.Append(controllingPlayer);
        data.Append(",");
        data.Append(controllingPlayer.transform.position.x);
        data.Append(",");
        data.Append(controllingPlayer.transform.position.y);
        data.Append(",");
        data.Append(controllingPlayer.vx);
        data.Append(",");
        data.Append(controllingPlayer.vy);

        SendData((int)PacketType.PLAYER, data.ToString(), false, Client);
    }

    //process all things here
    void TickUpdate()
    {

        //flag for checking if moved
        if (controllingPlayer.moved)
        {
            //sends update
            SendUpdate();
            controllingPlayer.moved = false;
        }
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
                if (parsedData.Length == 5)
                {
                    Debug.Log("Player Position:" + data);

                    DataQueue.Enqueue(parsedData);
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