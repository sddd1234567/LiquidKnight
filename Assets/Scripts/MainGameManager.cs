using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour {
    public GameObject playerCamera;
    public PlayerController player;
    public static MainGameManager instance;
    public Map map;
    public bool isTutorial;
    public bool canUseSkill;
    public GameObject changeCameraFocusPanel;
    public Animator StartCountAnimation;
    int i = 0;
    public int PlayerLoadingFinishCount;
    public bool isGameOver;
    public bool isGameStart;
    public Image ScoreBG;
    public GameObject gameOverUI;
    public reward[] rewardSugar;
    // Use this for initialization

    public static MainGameManager getInstance() {
        if (instance == null)
        {
            instance = new GameObject().AddComponent<MainGameManager>();
            return instance;
        }
        else
            return instance;
    }

    private void Awake()
    {
        canUseSkill = true;
        instance = this;
        if (SceneManager.GetActiveScene().name == "Tutorial")
            isTutorial = true;
    }

    void Start() {
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(PhotonNetwork.isMasterClient);
	}

    public void ShowScoreBoard()
    {
        map.DisableBGM();
        ScoreManager.getInstance().scoreObj.SetActive(true);
    }

    public Map InitialMap(string map)
    {
        GameObject mapObj = Resources.Load("Maps/" + map) as GameObject;
        GameObject newObj = Instantiate(mapObj, mapObj.transform.position, Quaternion.identity);
        this.map = newObj.GetComponent<Map>();
        ScoreBG.sprite = Resources.Load<Sprite>("Maps/Image/" + map);
        Instantiate(Resources.Load<GameObject>("Maps/MiniMap/" + map), playerCamera.transform);
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            Destroy(newObj.transform.Find("ItemFactory").gameObject);
        }
        if(PhotonNetwork.isMasterClient)
            this.map.ApplyMapBuff();
        return this.map;
    }

    public void UseSkill()
    {
        if (!canUseSkill)
            return;
        player.gameObject.GetPhotonView().RPC("UseSkill",PhotonTargets.All);
    }

    public void BackToRoom()
    {
        PhotonNetwork.LoadLevel("Room");
        //if (PhotonNetwork.isMasterClient)
        //    PhotonNetwork.LoadLevel("Room");
        //else
        //    StartCoroutine(EntireGameManager.LoadSceneWithLoadingScene("Room"));
    }

    public void BackToRoomList()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            if (EntireGameManager.getInstance().playerData.isNewPlayer)
            {
                EntireGameManager.getInstance().playerData.isNewPlayer = false;
                EntireGameManager.getInstance().Save();
                EntireGameManager.nextScene = "";
                SocialManager.UnlockAchievement(LiquidKnightResources.achievement);
                SocialManager.AddAchievementQueue("新手貓貓",5);
            }
        }
        PhotonManager.getInstance().BackToRoomList();
        //StartCoroutine(EntireGameManager.LoadSceneWithLoadingScene("RoomList"));
    }

    public void ChangeCameraFocus()
    {
        bool meetPre = false;
        PlayerController firstLive = null;
        bool isMeetLive = false;
        foreach (PlayerController pl in PhotonManager.getInstance().playerList)
        {
            if (!pl.isDied && !isMeetLive)
            {
                isMeetLive = true;
                firstLive = pl;
            }
            if (pl.ID == player.targetFocus.ID)
            {
                meetPre = true;
                continue;
            }
            if (meetPre && !pl.isDied)
            {
                player.targetFocus = pl;
                Debug.Log("NextID = " + pl.ID);
                return;
            }
        }
        if(firstLive != null)
            player.targetFocus = firstLive;

    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name != "Tutorial")
        {
            //AudioManager.Play("StartCount");
            StartCountAnimation.gameObject.SetActive(true);
            CoroutineUtility.GetInstance()
                .Do()
                .Wait(3)
                .Then(() => SetAllPlayerCanMove(true))
                .Then(() => isGameStart = true)
                .Go();
            CoroutineUtility.GetInstance()
                .Do()
                .Play(StartCountAnimation, "StartCount")
                .Then(() => StartCountAnimation.gameObject.SetActive(false))
                .Then(() => map.EnalbeBGM())
                .Go();
        }
        else
        {
            isGameStart = true;
        }
            
    }
    public void SetAllPlayerCanMove(bool can)
    {
        foreach (PlayerController pl in PhotonManager.getInstance().playerList)
        {
            pl.canMove = can;
        }
    }

    public void CheckLivePlayerCount()
    {
        if (!PhotonNetwork.isMasterClient)
            return;
        int count = 0;
        foreach (PlayerController pl in PhotonManager.getInstance().playerList)
        {
            if (!pl.isDied)
                count++;
        }
        if (count <= 3)
            ItemFactory.getInstance().frequency = 5;
    }
}

[System.Serializable]
public struct reward {
    public int[] num;
}
