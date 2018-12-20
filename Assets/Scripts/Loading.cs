using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour {
    [SerializeField]bool isFinishedLoading = false;

    [SerializeField]Image fadePanel;
    AsyncOperation async;
    bool isCreatedRoom = false;
    // Use this for initialization
    void Start () {
        if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
            PhotonManager.getInstance().ConnectToServer();
        if (EntireGameManager.nextScene != "MainGame")
        {
            async = SceneManager.LoadSceneAsync(EntireGameManager.nextScene);
            async.allowSceneActivation = false;
        }
        else
            PhotonNetwork.LoadLevel("MainGame");
    }
	
	// Update is called once per frame
	void Update () {
        if (EntireGameManager.nextScene != "MainGame")
        {
            if (!isFinishedLoading && PhotonManager.getInstance().isInLobby())
            {
                if (EntireGameManager.nextScene == "Tutorial" && !isCreatedRoom)
                {
                    PhotonManager.getInstance().CreateTutorial();
                    isCreatedRoom = true;
                }
                if (EntireGameManager.nextScene != "Tutorial")
                {
                    async.allowSceneActivation = true;
                    isFinishedLoading = true;
                }
            }
            else if (PhotonNetwork.inRoom && EntireGameManager.nextScene == "Tutorial" && !isFinishedLoading)
            {
                async.allowSceneActivation = true;
                isFinishedLoading = true;
                Debug.Log("Finish");
            }
            else if (PhotonNetwork.inRoom && EntireGameManager.nextScene == "Room" && !isFinishedLoading)
            {
                async.allowSceneActivation = true;
                isFinishedLoading = true;
            }
        }
        
    }
}
