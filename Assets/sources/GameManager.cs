using UnityEngine;
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
        Debug.Log("initBoard start");
        cardHolder = new GameObject("Board").transform;

        List<Vector3> listPositions = new List<Vector3>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                listPositions.Add(new Vector3(j - (float)(cols - 1) / 2.0f, i - (float)(rows - 1) / 2.0f, 0f) * 2.5f);
            }
        }

        arrayCards = new Card[rows * cols];
        for (int i = 0; i < rows * cols; i++)
        {
            int randomPos = Random.Range(0, listPositions.Count);
            Vector3 cardPosition = listPositions[randomPos];
            listPositions.RemoveAt(randomPos);

            GameObject cardInstance = Instantiate(
                card.gameObject,
                cardPosition,
                Quaternion.identity
            ) as GameObject;
            arrayCards[i] = cardInstance.GetComponent<Card>();
            arrayCards[i].m_val = i / 4 + 1;
            arrayCards[i].m_shape = (Card.Shape)(i % 4);

            Debug.Log(Card.cardToString(arrayCards[i].m_val, arrayCards[i].m_shape));
        }
    }

    public void Update()
    {
        /* react to inputs */
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        List<Card> openCards = new List<Card>();

        foreach (Card card in arrayCards)
        {
            if (card.facingFront)
            {
                openCards.Add(card);
            }
        }

        if (openCards.Count == 2)
        {
        }
        if (openCards.Count >= 2)
        {

        }
    }
}
