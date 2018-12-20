using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstSceneManager : MonoBehaviour {
    public Image panel;
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void StartGame()
    {
        PhotonManager.getInstance().ConnectToServer();
        if (EntireGameManager.getInstance().playerData.isNewPlayer)
        {
            StartCoroutine(EntireGameManager.LoadSceneWithLoadingScene("Tutorial"));
        }
        else
            StartCoroutine(EntireGameManager.LoadSceneWithLoadingScene("RoomList"));
    }
}
