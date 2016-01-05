using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SocketIO;

public class SocketManager : MonoBehaviour {

    private SocketIOComponent socket;
    private GameManager gameMananger;

    // card data struct for JSON parsing
    class JsonCard
    {
        public int number { get; set; }   
        public string shape { get; set; }
    }

	// Use this for initialization
	void Start () {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On("open", Open);
        //socket.On("boop", Boop);
        socket.On("match", Match);
        socket.On("deal", Deal);
        socket.On("bet", Bet);
        socket.On("flop", Flop);
        socket.On("turn", Turn);
        socket.On("river", River);
        socket.On("msg", Msg);
        socket.On("error", Error);
        socket.On("close", Close);

        gameMananger = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine("TryMatch");
    }

    private IEnumerator TryMatch()
    {
        yield return new WaitForSeconds(1);
        socket.Emit("match");
    }

    public void Open(SocketIOEvent e)
    {
        Debug.Log("Open recieved: " + e.name + " " + e.data);
        
    }

    public void Match(SocketIOEvent e)
    {
        Debug.Log("Match recieved: " + e.name + " " + e.data);

        bool isFound = false;
        e.data.GetField(ref isFound, "found");
        if (isFound)
        {
            StopCoroutine("TryMatch");
            gameMananger.activateMainGame();
        }
    }

    public void Deal(SocketIOEvent e)
    {
        Debug.Log("Deal recieved: " + e.name + " " + e.data);

        string cardString = e.data.GetField("cards").ToString();

        List<JsonCard> listCards = JsonConvert.DeserializeObject<List<JsonCard>>(cardString);
        if (listCards.Count == 2)
        {
            for (int i=0; i<2; i++)
            {
                gameMananger.flipCard(
                    "Player" + i.ToString(),
                    listCards[i].number,
                    listCards[i].shape
                );
            }
        }
    }

    public void Bet(SocketIOEvent e)
    {
        Debug.Log("Bet recieved: " + e.name + " " + e.data);

        int amount = 0;
        e.data.GetField(ref amount, "amount");
        gameMananger.updateOpponentBet(amount);
    }

    public void Flop(SocketIOEvent e)
    {
        Debug.Log("Flop recieved: " + e.name + " " + e.data);

        string cardString = e.data.GetField("cards").ToString();

        List<JsonCard> listCards = JsonConvert.DeserializeObject<List<JsonCard>>(cardString);
        if (listCards.Count == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                gameMananger.flipCard(
                    "Common" + i.ToString(),
                    listCards[i].number,
                    listCards[i].shape
                );
            }
        }
    }

    public void Turn(SocketIOEvent e)
    {
        Debug.Log("Turn recieved: " + e.name + " " + e.data);

        string cardString = e.data.GetField("cards").ToString();

        List<JsonCard> listCards = JsonConvert.DeserializeObject<List<JsonCard>>(cardString);
        if (listCards.Count == 1)
        {
            gameMananger.flipCard(
                "Player3",
                listCards[0].number,
                listCards[0].shape
            );
        }
    }

    public void River(SocketIOEvent e)
    {
        Debug.Log("River recieved: " + e.name + " " + e.data);

        string cardString = e.data.GetField("cards").ToString();

        List<JsonCard> listCards = JsonConvert.DeserializeObject<List<JsonCard>>(cardString);
        if (listCards.Count == 1)
        {
            gameMananger.flipCard(
                "Player4",
                listCards[0].number,
                listCards[0].shape
            );
        }
    }
    /*
        public void Boop(SocketIOEvent e)
        {
            Debug.Log("Boop received: " + e.name + " " + e.data);
            if (e.data == null) { return; }
            Debug.Log("THIS: " + e.data.GetField("this").str);
        }
    */
    public void Msg(SocketIOEvent e)
    {
        Debug.Log("Msg received: " + e.name + " " + e.data);
        Debug.Log("Message: " + e.data.GetField("message").str);
    }

    public void Error(SocketIOEvent e)
    {
        Debug.Log("Error received: " + e.name + " " + e.data);
    }

    public void Close(SocketIOEvent e)
    {
        Debug.Log("Close received: " + e.name + " " + e.data);
    }

    public void sendBet(int amount)
    {
        JSONObject betInfo = new JSONObject();
        betInfo.AddField("amount", amount);
        socket.Emit("bet", betInfo);
    }

    public void sendFold()
    {
        socket.Emit("fold");
    }
}
