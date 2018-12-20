using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ExitGames.Client.Photon;
using UnityEngine;

public class InputSocket : MonoBehaviour
{
    IPEndPoint ipep;
    Socket sock;
    EndPoint endPoint;
    IPAddress serverAddr;
    Thread connectThread;
    PlayerInputPacket inputPacket;
    int ID;
    string roomName;
    FakeInputer fakeInputer;
    bool isMasterClient;
    List<PlayerInputPacket> inputList;
    UdpClient udpClient;
    // Use this for initialization
    private void Awake()
    {
        
        Application.runInBackground = true;
    }

    void Start()
    {

        if (!gameObject.GetPhotonView().isMine)
            Destroy(this);
        else
        {
            ID = PhotonNetwork.player.ID;
            roomName = PhotonNetwork.room.Name;

            inputList = new List<PlayerInputPacket>();
            fakeInputer = GetComponent<FakeInputer>();
            inputPacket = new PlayerInputPacket();
            isMasterClient = PhotonNetwork.isMasterClient;

            inputPacket.isMasterClient = isMasterClient;
            inputPacket.playerID = ID;
            inputPacket.roomName = roomName;
            inputPacket.x = 0;
            inputPacket.y = 0;

            ipep = new IPEndPoint(IPAddress.Parse("192.168.137.242"), 6666);
            udpClient = new UdpClient();
            connectThread = new Thread(new ThreadStart(TryReceive));
            connectThread.Start();
        }
    }

    void TryReceive()
    {
        while (true)
        {
            string str = JsonUtility.ToJson(inputPacket);
            Debug.Log(str);
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            //byte[] bytes = ;
            //Debug.Log(ByteArrayToNewStuff(bytes).isMasterClient);
            udpClient.Send(bytes, bytes.Length, ipep);
            IPEndPoint point = new IPEndPoint(IPAddress.Any, 0);

            byte[] data = udpClient.Receive(ref point);

            string message = Encoding.UTF8.GetString(data);

            Debug.Log("收到了消息：" + message);
            List<PlayerInputPacket> list;
        }
    }

    void Update()
    {
        //if (isMasterClient)
        //{
        //    if (inputList != null)
        //    {
        //        foreach (PlayerInputPacket input in inputList)
        //        {
        //            //PlayerController player = PhotonManager.getInstance().FindPlayerByID(input.playerID);
        //            //player.forceFoward = new Vector2(input.x, input.y);
        //        }
        //    }
        //}
        //inputPacket.playerID = ID;
        //inputPacket.roomName = roomName;
        //inputPacket.x = fakeInputer.forceFoward.x;
        //inputPacket.y = fakeInputer.forceFoward.y;
        //inputPacket.isMasterClient = isMasterClient;
        //sendToSocket(getBytes(inputPacket));
        //byte[] sendBytes = getBytes(inputPacket);        
        
    }

    void ClientReceiveData()
    {
        //IPEndPoint object will allow us to read datagrams sent from any source.
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        // Blocks until a message returns on this socket from a remote host.
        Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
        string returnData = Encoding.ASCII.GetString(receiveBytes);
        Debug.Log(returnData);
    }

    public void SocketQuit()
    {
        //关闭线程
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最后关闭socket
        if (sock != null)
            sock.Close();
        if(udpClient != null)
            udpClient.Close();
    }


    void OnApplicationQuit()
    {
        SocketQuit();
    }

    void ExchangeData()
    {
        while (true)
        {
            inputPacket.playerID = ID;
            inputPacket.roomName = roomName;
            inputPacket.x = fakeInputer.forceFoward.x;
            inputPacket.y = fakeInputer.forceFoward.y;
            inputPacket.isMasterClient = isMasterClient;
            sendToSocket(getBytes(inputPacket));
            Debug.Log("asd");
            Thread.Sleep(1000);
            //if (isMasterClient)
            //{
            //    List<PlayerInputPacket> lists = new List<PlayerInputPacket>();
            //    byte[] data = new byte[Marshal.SizeOf(lists)]; //存放接收的資料
            //    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0); //定一個空端點(唯讀)
            //    EndPoint Remote = (EndPoint)sender; //不會解釋.. 宣告可以存放IP位址的用 EndPoint
            //    int recv = sock.ReceiveFrom(data, Marshal.SizeOf(lists), SocketFlags.None, ref Remote);
            //    if (data == null || data.Length == 0)
            //    {
            //        print("nodata");
            //        continue;
            //    }
            //    else
            //    {
            //        print(Encoding.UTF8.GetString(data, 0, recv));
            //    }
            //    List<PlayerInputPacket> newInputList = new List<PlayerInputPacket>();
            //    newInputList = ByteArrayToInputList(data);
            //    inputList = newInputList;
            //}
        }
    }

    List<PlayerInputPacket> ByteArrayToInputList(byte[] bytes)
    {
        string get = Encoding.UTF8.GetString(bytes);
        return JsonUtility.FromJson<InputList>(get).playerInputList;
    }

    public void sendToSocket(byte[] send_buffer)
    {
        try
        {
            sock.SendTo(send_buffer, endPoint);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void initialClient(String ip)
    {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

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

    private void OnDestroy()
    {
        SocketQuit();
    }

}

[Serializable]
public struct InputList
{
    [SerializeField]
    public List<PlayerInputPacket> playerInputList;
}

[Serializable]
public struct PlayerInputPacket
{
    public bool isMasterClient;
    public string roomName;
    public int playerID;
    public float x;
    public float y;
}