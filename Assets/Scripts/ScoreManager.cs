using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    public List<Text> PlayerNameText;
    public List<Image> WinnerAniaml;
    public List<Image> WinnerCup;
    public List<Image> WinnerLiquid;
    public List<Image> reward;
    public List<Text> rewardText;
    public int playerCount;
    public int nowRank;
    public GameObject scoreObj;
    public Button BackButton;
    public static ScoreManager instance;

    public static ScoreManager getInstance()
    {
        if (instance == null)
            instance = new GameObject("ScoreManager").AddComponent<ScoreManager>();
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start() {
        BackButton.onClick.AddListener(() => MainGameManager.getInstance().BackToRoomList());
    }

    // Update is called once per frame
    void Update() {

    }

    public void InitialCount(int count)
    {
        playerCount = count;
        nowRank = count;

        for (int i = count; i < PlayerNameText.Count; i++)
        {
            PlayerNameText[i].gameObject.SetActive(false);
            reward[i].gameObject.SetActive(false);
            rewardText[i].gameObject.SetActive(false);
        }
    }

    public void SetWinnerImage(int index, string animal, string cup, string liquid)
    {
        WinnerAniaml[index].gameObject.SetActive(true);
        WinnerCup[index].gameObject.SetActive(true);
        WinnerLiquid[index].gameObject.SetActive(true);
        WinnerAniaml[index].sprite = Resources.Load<Sprite>("Character/SelectView/animals/" + animal);
        WinnerCup[index].sprite = Resources.Load<Sprite>("Character/SelectView/cup/" + cup);
        WinnerLiquid[index].sprite = Resources.Load<Sprite>("Character/SelectView/liquid/" + liquid);
    }

    public void SetRank(int index, PlayerController player)
    {
        int playerCount = PhotonManager.getInstance().playerList.Count;
        int reward = MainGameManager.getInstance().rewardSugar[playerCount].num[index];
        if (player.isOwner())
        {
            EntireGameManager.getInstance().playerData.sugar += reward;
            EntireGameManager.getInstance().Save();
        }
        Debug.Log("Rank = " + index);
        if (index <= 2)
        {
            SetWinnerImage(index, player.animal, player.cup, player.liquid);
            if (index == 1 && SceneManager.GetActiveScene().name == "MainGame")
            {
            }
        }
        if (index == 0)
            Debug.Log("第一名");
        PlayerNameText[index].text = player.NickName;
        rewardText[index].text = reward.ToString();
    }

    public int OnPlayerDied(PlayerController player)
    {
        nowRank--;
        if (nowRank == 1)
        {
            PhotonNetwork.automaticallySyncScene = false;
            Debug.Log("GameOver");
        }
        return nowRank;
    }
}
