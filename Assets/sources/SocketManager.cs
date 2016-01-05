using UnityEngine;
using System.Collections;
using SimpleJson;
using SocketIO;

public class SocketManager : MonoBehaviour {

    private SocketIOComponent socket;
    private GameManager gameMananger;

	// Use this for initialization
	void Start () {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On("open", Open);
        //socket.On("boop", Boop);
        socket.On("match", Match);
        socket.On("deal", Deal);
        socket.On("msg", Msg);
        socket.On("error", Error);
        socket.On("close", Close);

        gameMananger = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private IEnumerator TryMatch()
    {
        yield return new WaitForSeconds(1);
        socket.Emit("match");
    }

    public void Open(SocketIOEvent e)
    {
        Debug.Log("Open recieved: " + e.name + " " + e.data);
        StartCoroutine("TryMatch");
    }

    public void Match(SocketIOEvent e)
    {
        Debug.Log("Match recieved: " + e.name + " " + e.data);

        bool isFound = false;
        e.data.GetField(ref isFound, "found");
        if (isFound)
        {
            gameMananger.initBoard();
        }
    }

    public void Deal(SocketIOEvent e)
    {

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
}
