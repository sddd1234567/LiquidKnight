using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;
using UnityEngine.UI;

public class VrSocketClient : MonoBehaviour {
    public static bool isStartTracked;
    public Text tt;
    Socket sock;
    IPAddress serverAddr;
    IPEndPoint endPoint;
    public GameObject targetObj;

    Socket sock2;
    IPAddress serverAddr2;
    IPEndPoint endPoint2;
    public GameObject targetObj2;

    public static VrSocketClient instance;
    //public bool isTracked;
    public bool isLeftTracked;
    public bool isRightTracked;
    public GameObject controllerRotation;
    // Use this for initialization
    void Start () {
        
        //Application.runInBackground = true;
        instance = this;
        initialClient("104.199.192.223");


        
    }
	
	// Update is called once per frame
	void Update () {
        PlayerInputPacket newPacket = new PlayerInputPacket();
        newPacket.playerID = 1;
        newPacket.roomName = "asd";
        newPacket.x = 5;
        newPacket.y = 6;
        newPacket.isMasterClient = true;
        sendToSocket(getBytes(newPacket));
    }

    public void sendToSocket(byte[] send_buffer)
    {
        try
        {
            sock.SendTo(send_buffer, endPoint);
        }
        catch(Exception e) {
        }        
    }

    public void initialClient(String ip) {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);

        serverAddr = IPAddress.Parse(ip);

        endPoint = new IPEndPoint(serverAddr, 6666);
    }

    public PlayerInputPacket ByteArrayToNewStuff(byte[] bytes)
    {
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        PlayerInputPacket stuff = (PlayerInputPacket)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(PlayerInputPacket));
        handle.Free();
        return stuff;
    }

    public byte[] getBytes(PlayerInputPacket str)
    {
        int size = Marshal.SizeOf(str);
        byte[] arr = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(str, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }

    public byte[] GetBytes<T>(T str)
    {
        int size = Marshal.SizeOf(str);
        byte[] arr = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(str, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }
}