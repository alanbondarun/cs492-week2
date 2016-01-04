using UnityEngine;
using SocketIOClient;
using System;
using SimpleJson;
using System.Threading;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    /* singleton instance */
    public static GameManager instance = null;

    // A variable to store a reference to the transform of our Board object.
    private Transform cardHolder;

    public int rows = 4;
    public int cols = 4;

    // a card prefab
    public GameObject card;

    // list of cards
    private Card[] arrayCards;

    public GameObject firstScreenMsg;

    // client object
    private Client client = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        firstScreenMsg = Instantiate(
            Resources.Load<GameObject>("prefabs/message_waiting"),
            Vector3.zero,
            Quaternion.identity
        ) as GameObject;

        connectWithServer();
    }

    public void connectWithServer()
    {
        client = new Client("http://143.248.233.58:3000");
        client.Opened += onSocketOpened;
        client.Message += onSocketGetMessage;
        client.SocketConnectionClosed += onSocketConnectionClosed;
        client.Error += onSocketError;

        client.Connect();
    }

    // invoked when the socket opened
    private void onSocketOpened(object sender, EventArgs e)
    {
        Debug.Log("onSocketOpened");
        Destroy(firstScreenMsg);
        firstScreenMsg = Instantiate(
            Resources.Load<GameObject>("prefabs/message_completed"),
            Vector3.zero,
            Quaternion.identity
        ) as GameObject;
    }

    // invoked when the socket gets message
    private void onSocketGetMessage(object sender, MessageEventArgs e)
    {
        Debug.Log("onSocketGetMessage");
        if (e != null && e.Message.Event == "message")
        {
            dynamic data = e.Message.Json.GetArgsAs<dynamic>();
            /* TODO: json parsing */
        }
    }

    // invoked when the socket connection is closed
    private void onSocketConnectionClosed(object sender, EventArgs e)
    {
        Debug.Log("onSocketConnectionClosed");

    }

    // invoked when the socket gets an error
    private void onSocketError(object sender, ErrorEventArgs e)
    {
        Debug.Log("onSocketError: " + e.Message);
    }

    public void initBoard()
    {

    }

    public void Update()
    {
        /* react to inputs */
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
