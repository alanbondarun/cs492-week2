using UnityEngine;
using UnityEngine.UI;
using SocketIOClient;
using System;

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

    private Text txtOpponentBet = null;
    private Text txtPlayerBet = null;
    private InputField inpBetAmount = null;

    private SocketManager socketManager = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        initBoard();
    }

    public void initBoard()
    {
        firstScreenMsg = Instantiate(
            Resources.Load<GameObject>("prefabs/message_waiting"),
            Vector3.zero,
            Quaternion.identity
        ) as GameObject;

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

        txtOpponentBet = GameObject.Find("txtOpponentBet").GetComponent<Text>();
        txtPlayerBet = GameObject.Find("txtPlayerBet").GetComponent<Text>();
        inpBetAmount = GameObject.Find("inpBetAmount").GetComponent<InputField>();

        socketManager = GameObject.Find("SocketManager").GetComponent<SocketManager>();
    }

    public void activateMainGame()
    {
        if (firstScreenMsg != null)
        {
            Destroy(firstScreenMsg);
            firstScreenMsg = null;
        }

        GameObject.Find("UICanvas").SetActive(true);
        GameObject.Find("CardGroup").SetActive(true);
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

    public void raisePlayerBet()
    {
        updatePlayerBet(Int32.Parse(inpBetAmount.text));
    }

    public void callPlayerBet()
    {
        int addValue = Int32.Parse(txtOpponentBet.text) - Int32.Parse(txtPlayerBet.text);
        updatePlayerBet(addValue);
    }

    public void updateOpponentBet(int value)
    {
        txtOpponentBet.text = value.ToString();
    }

    public void Update()
    {
        /* react to inputs */
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void updatePlayerBet(int addValue)
    {
        if (addValue > 0)
        {
            int currentBet = Int32.Parse(txtPlayerBet.text);
            currentBet += addValue;
            txtPlayerBet.text = currentBet.ToString();
            socketManager.sendBet(currentBet);

            if (currentBet == Int32.Parse(txtOpponentBet.text))
            {
                socketManager.sendFlop();
            }
        }
    }
}
