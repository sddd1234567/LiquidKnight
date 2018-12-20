using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;

public class SocialManager : MonoBehaviour {
    // Use this for initialization
    public static List<string> achievementQueue;
    public static bool readPause;
    private void Awake()
    {
        readPause = false;
        PlayGamesPlatform.Activate();
    }

    void Start () {
        Social.localUser.Authenticate((bool success) =>
        {
            //Debug.Log(success);
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void ShowAchievement()
    {
        if (Social.localUser.authenticated)
            Social.ShowAchievementsUI();
        else
        {
            InitialGooglePlayService();
        }
    }

    public static void InitialGooglePlayService()
    {
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate((bool success) =>
        {
            Debug.Log(success);
        });
    }

    public static void UnlockAchievement(string achievementID)
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportProgress(achievementID, 100, (bool success) =>
            {
                Debug.Log("Achievement Unlock！ " + success);
            });
        }
        //else
        //{
        //    InitialGooglePlayService();
        //}
    }

    public static void AddAchievementQueue(string achievementName,int reward)
    {
        if (achievementQueue == null)
            achievementQueue = new List<string>();
        achievementQueue.Add("獲得成就「"+achievementName+"」\n獎勵：珍珠*" + reward);
        EntireGameManager.getInstance().playerData.pearl += reward;
        EntireGameManager.getInstance().Save();
    }

    public static void ReadQueue(MonoBehaviour caller)
    {
        if (achievementQueue == null)
            return;
        else if (achievementQueue.Count == 0)
            return;
        Debug.Log("ReadQueue");
        caller.StartCoroutine(DoReadQueue());
    }

    public static IEnumerator DoReadQueue()
    {
        for (int i = 0; i < achievementQueue.Count;)
        {
            MainUIManager.CreateMiniCheckBox(() => {
                readPause = false;
                achievementQueue.RemoveAt(0);
            }, achievementQueue[i]);
            readPause = true;
            yield return new WaitUntil(() => { return readPause == false; });
        }
    }
    


}
