using Assets.Tools.ProtoGen.Editor.protos.Test;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPanel : MonoBehaviour
{
    public InputField inputField;
    public Text text;
    MyClient myClient = new MyClient();
    MyServer myServer = new MyServer();
    private string lastMsg = "";

    // Start is called before the first frame update
    void Start()
    {
        myServer.StartServer();
        myClient.ConnetServer();
        myClient.receiveServerMsg += ReceiveMsg;
    }

    private void Update()
    {
        text.text = lastMsg;
    }

    public void ReceiveMsg(string msg) {
        lastMsg = msg;
    }

    public void ClickBtn() {
        Person p = new Person();
        p.name = "HelloWorld";
        p.number = 108;
        string str = ProtoHelper.Serialize<Person>(p);
        myClient.SendMessage(str);
        //myClient.SendMessage(inputField.text);
    }
}
