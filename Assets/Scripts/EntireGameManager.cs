using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class EntireGameManager{
    public static string nextScene;
    public SaveData playerData;
    public static EntireGameManager instance;
    public Image panel;
    // Use this for initialization

    string defaultAnimal = "cat";
    string defaultCup = "normalcup";
    string defaultLiquid = "milktea";

    public static EntireGameManager getInstance() {
        if (instance == null)
        {
            instance = new EntireGameManager();
            instance.loadPlayerData();
            return instance;
        }
        else
            return instance;
    }

    public static IEnumerator LoadSceneWithLoadingScene(Image image, string nextSceneName) {
        nextScene = nextSceneName;
        while (image.color.a < 1)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + Time.deltaTime*5);
            yield return null;
        }
        SceneManager.LoadScene("LoadingScene");
    }

    public static IEnumerator LoadSceneWithLoadingScene(string nextSceneName)
    {
        nextScene = nextSceneName;
        GameObject canvas = GameObject.Find("Canvas");
        Image image = MainUIManager.CreateCoverPanel();
        while (image.color.a < 1)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + Time.deltaTime * 5);
            yield return null;
        }
        SceneManager.LoadScene("LoadingScene");
    }

    public static IEnumerator LoadSceneWithoutLoading(Image image, string nextSceneName)
    {
        while (image.color.a < 1)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + Time.deltaTime * 5);
            yield return null;
        }
        SceneManager.LoadScene(nextSceneName);
    }

    public void Save(SaveData newData) {
        string jsonData = JsonUtility.ToJson(newData);
        PlayerPrefs.SetString("SaveData", jsonData);
        PlayerPrefs.Save();
    }

    public void Save()
    {
        string jsonData = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString("SaveData", jsonData);
        PlayerPrefs.Save();
    }

    public SaveData loadPlayerData() {
        string jsonData;
        //PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        if (PlayerPrefs.GetString("SaveData") != "" && PlayerPrefs.GetString("SaveData") != null)
        {
            jsonData = PlayerPrefs.GetString("SaveData");
            SaveData data = JsonUtility.FromJson<SaveData>(jsonData);
            playerData = data;
        }
        else
            playerData = initialNewPlayer();

        return playerData;
    }



    public void AddPearls(int num)
    {
        playerData.pearl += num;
        Save();
    }

    SaveData initialNewPlayer() {
        SaveData newPlayer = new SaveData();
        newPlayer.pearl = 100;
        newPlayer.sugar = 2000;
        newPlayer.animalList = new List<string>();
        newPlayer.cupList = new List<string>();
        newPlayer.liquidList = new List<string>();
        newPlayer.isNewPlayer = true;
        newPlayer.HavePlayingMultiPlayerGame = false;
        newPlayer.name = "吉米";
        newPlayer.CatPlayTime = 0;
        newPlayer.DogPlayTime = 0;
        newPlayer.FishPlayTime = 0;
        newPlayer.nowEquipment.animal = defaultAnimal;
        newPlayer.nowEquipment.liquid = defaultLiquid;
        newPlayer.nowEquipment.cup = defaultCup;
        newPlayer.animalList.Add(defaultAnimal);
        newPlayer.liquidList.Add(defaultLiquid);
        newPlayer.cupList.Add(defaultCup);
        return newPlayer;
    }

    public SaveData getPlayerData() {
        return playerData;
    }
}


[System.Serializable]
public struct SaveData {
    public int sugar;
    public int pearl;
    public List<string> liquidList;
    public List<string> animalList;
    public List<string> cupList;
    public Equipment nowEquipment;
    public bool isNewPlayer;
    public bool HavePlayingMultiPlayerGame;
    public string name;
    public int CatPlayTime;
    public int DogPlayTime;
    public int FishPlayTime;

    public bool HaveItem(MerchandiseType type, string Name)
    {
        List<string> targetList;
        if (type == MerchandiseType.Animal)
            targetList = animalList;
        else if (type == MerchandiseType.Cup)
            targetList = cupList;
        else
            targetList = liquidList;
        return targetList.Exists(x => x == Name);
    }
}

[System.Serializable]
public struct Equipment {
    public string cup;
    public string liquid;
    public string animal;
}
