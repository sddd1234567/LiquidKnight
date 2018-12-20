using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ScoreBoard : MonoBehaviour {
    public Text countDownText;
    public GameObject countDownBackText;
	// Use this for initialization
	void Start ()
    {
        if(MainGameManager.getInstance().isGameOver && SceneManager.GetActiveScene().name != "Tutorial")
            StartBoardAnimation();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Debug.Log(PhotonNetwork.isMessageQueueRunning);
        //Debug.Log(PhotonNetwork.automaticallySyncScene);
	}

    public void StartBoardAnimation()
    {
        if (!MainGameManager.getInstance().isGameOver || SceneManager.GetActiveScene().name == "Tutorial")
        {
            countDownText.gameObject.SetActive(false);
            countDownBackText.gameObject.SetActive(false);
            return;
        }
        CoroutineUtility.GetInstance()
            .Do()
            .Play(GetComponent<Animator>(), "ScoreBoardIn")
            .Wait(0.8f)
            .Then(() => countDownText.text = "9")
            .Wait(0.8f)
            .Then(() => countDownText.text = "8")
            .Wait(0.8f)
            .Then(() => countDownText.text = "7")
            .Wait(0.8f)
            .Then(() => countDownText.text = "6")
            .Wait(0.8f)
            .Then(() => countDownText.text = "5")
            .Then(DestroyAllPlayers)
            .Wait(0.8f)
            .Then(() => countDownText.text = "4")
            .Wait(0.8f)
            .Then(() => countDownText.text = "3")
            .Wait(0.8f)
            .Then(() => countDownText.text = "2")
            .Wait(0.8f)
            .Then(() => countDownText.text = "1")
            .Wait(0.8f)
            .Then(() => {
                PhotonNetwork.LoadLevel("Room");
            })
            .Go();
        if (!EntireGameManager.getInstance().playerData.HavePlayingMultiPlayerGame && SceneManager.GetActiveScene().name != "Tutorial")
        {
            EntireGameManager.getInstance().playerData.HavePlayingMultiPlayerGame = true;
            SocialManager.UnlockAchievement(LiquidKnightResources.achievement_2);
            SocialManager.AddAchievementQueue("派對！",5);
        }
        if (MainGameManager.getInstance().player.animal == "cat")
        {
            EntireGameManager.getInstance().playerData.CatPlayTime++;
            if (EntireGameManager.getInstance().playerData.CatPlayTime == 5)
            {
                SocialManager.UnlockAchievement(LiquidKnightResources.achievement_3);
                SocialManager.AddAchievementQueue("貓型玩家", 5);
            }
        }
        else if (MainGameManager.getInstance().player.animal == "dog")
        {
            EntireGameManager.getInstance().playerData.DogPlayTime++;
            if (EntireGameManager.getInstance().playerData.DogPlayTime == 5)
            {
                SocialManager.UnlockAchievement(LiquidKnightResources.achievement_4);
                SocialManager.AddAchievementQueue("狗型玩家", 5);
            }
        }
        else if (MainGameManager.getInstance().player.animal == "fish")
        {
            EntireGameManager.getInstance().playerData.FishPlayTime++;
            if (EntireGameManager.getInstance().playerData.FishPlayTime == 5)
            {
                SocialManager.UnlockAchievement(LiquidKnightResources.achievement_5);
                SocialManager.AddAchievementQueue("魚型玩家", 5);
            }
        }

        EntireGameManager.getInstance().Save();
    }

    public void DestroyAllPlayers()
    {
        if(PhotonNetwork.isMasterClient)
            PhotonNetwork.DestroyAll();
    }

    public void BackToRoom()
    {
        if (PhotonNetwork.isMasterClient)
        {
            MainGameManager.getInstance().player.gameObject.GetPhotonView().RPC("ReturnToRoom",PhotonTargets.All);
        }
    }
}
