using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour {
    public Text LobbyText;
    public Button StartGameButton;
    public static LobbyUIManager instance;
	// Use this for initialization

    private void Awake()
    {
        instance = this;
    }

    void Start () {
        PhotonManager.getInstance().ConnectToServer();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void OnJoinedLobby()
    {
        print("joinLobby");
    }

    public void changeNumberView(int people) {
        changeLobbyText("目前房間人數有" + people + "人");
    }

    public void changeLobbyText(string text) {
        LobbyText.text = text;
    }

    public void setStartButtonState(bool state) {
        StartGameButton.gameObject.SetActive(state);
    }
}
