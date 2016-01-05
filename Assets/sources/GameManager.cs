using UnityEngine;
using SocketIOClient;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class GameManager : MonoBehaviour
{
    /* singleton instance */
    public static GameManager instance = null;

    public int rows = 4;
    public int cols = 4;

    // a card prefab
    public GameObject card;

    public GameObject firstScreenMsg = null;

    // A variable to store a reference to the transform of our Board object.
    private Transform cardHolder;

    // list of the player's cards
    private Card[] arrayPlayerCards;

    // list of the opponent's cards
    private Card[] arrayOpponentCards;

    // list of the common cards
    private Card[] arrayCommonCards;

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

    public void flipCard(string whichCard, int number, string shape)
    {
        if (whichCard.StartsWith("Player"))
        {
            int idx = Int32.Parse(whichCard.Substring("Player".Length));
            if (idx >= 0 && idx < 2)
            {
                arrayPlayerCards[idx].m_val = number;
                arrayPlayerCards[idx].m_shape = Card.getShapeFromString(shape);
                arrayPlayerCards[idx].Flip();
            }
        }
        else if (whichCard.StartsWith("Opponent"))
        {
            int idx = Int32.Parse(whichCard.Substring("Opponent".Length));
            if (idx >= 0 && idx < 2)
            {
                arrayOpponentCards[idx].m_val = number;
                arrayOpponentCards[idx].m_shape = Card.getShapeFromString(shape);
                arrayOpponentCards[idx].Flip();
            }
        }
        else if (whichCard.StartsWith("Common"))
        {
            int idx = Int32.Parse(whichCard.Substring("Common".Length));
            if (idx >= 0 && idx < 5)
            {
                arrayCommonCards[idx].m_val = number;
                arrayCommonCards[idx].m_shape = Card.getShapeFromString(shape);
                arrayCommonCards[idx].Flip();
            }
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
