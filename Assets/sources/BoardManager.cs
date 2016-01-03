using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    // A variable to store a reference to the transform of our Board object.
    private Transform cardHolder;

    public int rows = 4;
    public int cols = 4;

    // array of Card prefabs
    public GameObject[] cardObjects;

    private Card[] cards;
    private Vector3[] cardPositions;

    public void initBoard()
    {
        Debug.Log("initBoard start");
        cardHolder = new GameObject("Board").transform;

        List<Vector3> listPositions = new List<Vector3>();
        for (int i=0; i<rows; i++)
        {
            for (int j=0; j<cols; j++)
            {
                listPositions.Add(new Vector3(j - (float)(cols-1)/2.0f, i - (float)(rows - 1)/2.0f, 0f) * 2.5f);
            }
        }

        cards = new Card[rows*cols];
        cardPositions = new Vector3[rows * cols];
        for (int i=0; i<rows*cols; i++)
        {
            cards[i] = new Card();
            cards[i].m_val = i / 4 + 1;
            cards[i].m_shape = (Card.Shape)(i % 4);

            int randomPos = Random.Range(0, listPositions.Count);
            cardPositions[i] = listPositions[randomPos];
            listPositions.RemoveAt(randomPos);

            GameObject cardInstance = Instantiate(
                cardObjects[i],
                cardPositions[i],
                Quaternion.identity
            ) as GameObject;
            cardInstance.transform.SetParent(cardHolder);
        }
    }
}
