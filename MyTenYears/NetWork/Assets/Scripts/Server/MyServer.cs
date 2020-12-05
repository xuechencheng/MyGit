using Assets.Tools.ProtoGen.Editor.protos.Test;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class MyServer
{
    // Start is called before the first frame update
    public void StartServer()
    {
        Socket socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ip = IPAddress.Any;
        IPEndPoint point = new IPEndPoint(ip,5000);
        socketWatch.Bind(point);
        Debug.Log("监听成功");
        socketWatch.Listen(10);
        Thread th = new Thread(Listen);
        th.IsBackground = true;
        th.Start(socketWatch);
    }

    private void Listen(object o) {
        Socket socketWatch = o as Socket;
        while (true) {
            Socket socketSend = socketWatch.Accept();
            Debug.Log(socketSend.RemoteEndPoint.ToString() + ":连接成功");
            Thread th = new Thread(Receive);
            th.IsBackground = true;
            th.Start(socketSend);
        }
    }

    private void Receive(object o) {
        Socket socketSend = o as Socket;
        while (true)
        {
            byte[] buffer = new byte[1024 * 1024 * 2];
            int r = socketSend.Receive(buffer);
            if (r == 0) {
                Debug.Log(socketSend.RemoteEndPoint.ToString() + ":连接断开");
                return;
            }
            string str = Encoding.UTF8.GetString(buffer, 0, r);

            Person p = ProtoHelper.Deserialize<Person>(str);
            Debug.Log(p.name + " " + p.number);

            Debug.Log(socketSend.RemoteEndPoint.ToString() + ":" + str);
            //SendMessage("Hello World: " + str, socketSend);
        }
    }

    private void SendMessage(string msg, Socket socketSend) {
        byte[] buffer = Encoding.UTF8.GetBytes(msg);
        socketSend.Send(buffer);
    }
}
