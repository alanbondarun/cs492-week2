using UnityEngine;
using System.Collections.Generic;

public class Card : MonoBehaviour
{
    public int m_val;
    public enum Shape
    {
        Heart = 0,
        Club,
        Diamond,
        Spade
    };
    public Shape m_shape;
    public bool facingFront = false;

    public Sprite backSprite;
    public Dictionary<string, Sprite> dictCardSprites = new Dictionary<string, Sprite>();

    void Awake()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("cards");
        foreach (Sprite sprite in sprites)
        {
            dictCardSprites.Add(sprite.name, sprite);
        }

        facingFront = false;
    }

    void Update()
    {
        if (facingFront)
        {
            string spriteName = "cards_" + cardToString(m_val, m_shape);
            Sprite frontSprite = dictCardSprites[spriteName];
            GetComponent<SpriteRenderer>().sprite = frontSprite;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = backSprite;
        }
    }

    public void Flip()
    {
        facingFront = !facingFront;
    }

    public static string cardToString(int val, Shape shape)
    {
        string str = System.String.Empty;

        if (val <= 0 || val >= 14)
            return str;

        str = val.ToString();
        switch (shape)
        {
            case Shape.Club:
                str += "c";
                break;
            case Shape.Diamond:
                str += "d";
                break;
            case Shape.Heart:
                str += "h";
                break;
            case Shape.Spade:
                str += "s";
                break;
        }
        return str;
    }

    public static bool isSameColor(Shape shape1, Shape shape2)
    {
        bool shape1IsRed = (shape1 == Shape.Diamond) || (shape1 == Shape.Heart);
        bool shape2IsRed = (shape2 == Shape.Diamond) || (shape2 == Shape.Heart);
        return shape1IsRed == shape2IsRed;
    }

    public static Shape getShapeFromString(string str)
    {
        Shape shape = Shape.Club;
        switch (str)
        {
            case "Club": case "club":
                return Shape.Club;
            case "Diamond": case "diamond":
                return Shape.Diamond;
            case "Heart": case "heart":
                return Shape.Heart;
            case "Spade": case "spade":
                return Shape.Spade;
        }
        return shape;
    }
}
