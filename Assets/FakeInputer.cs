using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FakeInputer : Photon.PunBehaviour
{
    Vector3 target;
    public Vector2 newInput;
    public Vector2 forceFoward;
    public int playerID;
    public PlayerController attachPlayerController;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (gameObject.GetPhotonView().isMine)
        {
            checkInput();
        }
        if (PhotonNetwork.isMasterClient)
        {
            if(attachPlayerController != null)
                attachPlayerController.forceFoward = forceFoward;
        }
    }

    void checkInput()
    {
        float x;
        float y;
        //x = 0;
        //y = 0;
        //if (Mathf.Abs(Input.acceleration.x) > 0.2f)
        //{
        //    if (Input.acceleration.x > 0)
        //        x = 1;
        //    else
        //        x = -1;
        //}
        //if (Mathf.Abs(Input.acceleration.y) > 0.2f)
        //{
        //    if (Input.acceleration.y > 0)
        //        y = 1;
        //    else
        //        y = -1;
        //}

        x = 0;
        y = 0;
        if (Input.touchCount > 0)
        {
            Vector2 inp = Input.GetTouch(0).deltaPosition;
            x = inp.x / 5;
            y = inp.y / 5;
        }

        target.x += x;
        target.y += y;

        forceFoward = new Vector2(x, y);
    }

    [PunRPC]
    public void SetID(int id)
    {
        List<PlayerController> player = PhotonManager.getInstance().playerList;
        playerID = id;
        for (int i = 0; i < player.Count; i++)
        {
            if (player[i].ID == id)
            {
                attachPlayerController = player[i];
                return;
            }
        }
    }

    

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(getBytes(forceFoward));
        }
        else
        {
            forceFoward = ByteArrayToNewStuff((byte[])stream.ReceiveNext());
        }
    }


    Vector2 ByteArrayToNewStuff(byte[] bytes)
    {
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        Vector2 stuff = (Vector2)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Vector2));
        handle.Free();
        return stuff;
    }

    byte[] getBytes(Vector2 str)
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