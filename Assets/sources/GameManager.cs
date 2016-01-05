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

    // list of the player's cards
    private Card[] arrayPlayerCards;

    // list of the opponent's cards
    private Card[] arrayOpponentCards;

    // list of the common cards
    private Card[] arrayCommonCards;

    public GameObject firstScreenMsg = null;

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
    }

    public void initBoard()
    {
        if (firstScreenMsg != null)
        {
            Destroy(firstScreenMsg);
            firstScreenMsg = null;
        }

        GameObject.Find("UICanvas").SetActive(true);
        GameObject.Find("CardGroup").SetActive(true);

        arrayPlayerCards = new Card[2];
        arrayPlayerCards[0] = GameObject.Find("PlayerCard1").GetComponent<Card>();
        arrayPlayerCards[1] = GameObject.Find("PlayerCard2").GetComponent<Card>();

        arrayOpponentCards = new Card[2];
        arrayOpponentCards[0] = GameObject.Find("OpponentCard1").GetComponent<Card>();
        arrayOpponentCards[1] = GameObject.Find("OpponentCard2").GetComponent<Card>();

        arrayCommonCards = new Card[5];
        for (int i=0; i<5; i++)
        {
            string objName = "CommonCard" + (i + 1);
            arrayCommonCards[i] = GameObject.Find(objName).GetComponent<Card>();
        }
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
