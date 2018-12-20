using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour {
    string roomID;
    string roomMap;
    int maxPlayers;
    int nowPlayers;
    [SerializeField] Text roomIDText;
    [SerializeField] Text roomMapText;
    [SerializeField] Text roomPlayersText;
    Dictionary<string, string> MapName = new Dictionary<string, string>()
        {
            { "Picnic","陽光與野餐" },
            { "Table", "餐桌的戰爭" },
            { "IceFood", "冰天凍地"}
        };
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitialItem(string roomID, string roomMap, int maxPlayers, int nowPlayers)
    {
        this.roomID = roomID;
        this.roomMap = roomMap;
        this.maxPlayers = maxPlayers;
        this.nowPlayers = nowPlayers;

        roomIDText.text = roomID;
        roomMapText.text = MapName[roomMap];
        roomPlayersText.text = nowPlayers + "/" + maxPlayers;
    }

    public void joinThisRoom()
    {
        if (nowPlayers != maxPlayers)
        {
            PhotonManager.getInstance().joinRoom(roomID);
        }
        else
        {
            MainUIManager.CreateCheckBox(() => { }, "房間人數已滿");
        }
    }
}
