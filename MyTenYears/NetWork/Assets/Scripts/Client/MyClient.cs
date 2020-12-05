using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class MyClient
{
    public delegate void ReceiveServerMsg(string message);
    public ReceiveServerMsg receiveServerMsg;
    private Socket socketSend;
    public void ConnetServer() { 
        socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        IPEndPoint point = new IPEndPoint(ip, 5000);
        socketSend.Connect(point);
        Debug.Log("连接成功");
        Thread th = new Thread(Receive);
        th.IsBackground = true;
        th.Start();
    }

    public void SendMessage(string msg) {
        byte[] buffer = Encoding.UTF8.GetBytes(msg);
        socketSend.Send(buffer);
    }

    private void Receive()
    {
        while (true)
        {
            byte[] buffer = new byte[1024 * 1024 * 2];
            int r = socketSend.Receive(buffer);
            if (r == 0)
            {
                Debug.Log(socketSend.RemoteEndPoint.ToString() + ":连接断开");
                return;
            }
            string str = Encoding.UTF8.GetString(buffer, 0, r);
            Debug.Log("客户单收到消息 " + socketSend.RemoteEndPoint.ToString() + ":" + str);
            if (receiveServerMsg != null) {
                receiveServerMsg(str);
            }
        }
    }
}
