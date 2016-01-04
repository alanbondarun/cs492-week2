using UnityEngine;
using System.Collections;
using SocketIO;

public class SocketManager : MonoBehaviour {

    private SocketIOComponent socket;

	// Use this for initialization
	void Start () {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On("open", Open);
        socket.On("boop", Boop);
        socket.On("match", Match);
        socket.On("error", Error);
        socket.On("close", Close);

        StartCoroutine("BeepBop");
	}
	
	// Update is called once per frame
	//void Update ()
    //{
	//}


    private IEnumerator BeepBop()
    {
        yield return new WaitForSeconds(3);
        socket.Emit("beep");
    }

    public void Open(SocketIOEvent e)
    {
        Debug.Log("Open recieved: " + e.name + " " + e.data);
    }

    public void Match(SocketIOEvent e)
    {
        Debug.Log("Open recieved: " + e.name + " " + e.data);
    }

    public void Boop(SocketIOEvent e)
    {
        Debug.Log("Boop received: " + e.name + " " + e.data);
        if (e.data == null) { return; }
        Debug.Log("THIS: " + e.data.GetField("this").str);
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
