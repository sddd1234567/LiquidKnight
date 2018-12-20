using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{

    public static RoomManager instance;

    public List<GameObject> playerSits;
    public List<GameObject> addAIButton;
    public bool[] sitHasPlayer;
    //public int[] playerID;

    public Text RoomIDText;

    public Image playerAnimal;
    public Image playerCup;
    public Image playerLiquid;
    public PlayerInRoom localPlayer;
    public int AICount;
    public string[] animal;
    public string[] cup;
    public string[] liquid;
    public PlayerInRoom[] playerInRooms;
    public static PlayerInRoom myPlayerInRoom;
    Character[] AIList;
    public bool isStartGame;
    public float itemWidth;
    public GameObject MapContainer;
    public List<GameObject> MapImageList;
    public List<string> MapList;
    public int nowMapIndex;
    bool isSlidingMap;
    public int playerCount;

    public Image StartGameButton;

    public Sprite StartButtonSprite;
    public Sprite ReadyButtonSprite;
    public Sprite CancelButtonSprite;

    public Color readyColor;
    public Color unReadyColor;

    public bool isClientReady;

    bool isClickedMap = false;
    float clickTime = 0;

    public static RoomManager getInstance()
    {
        if (instance == null)
        {
            instance = new GameObject("RoomManager").AddComponent<RoomManager>();
            return instance;
        }
        else
            return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        //playerID = new int[7];
        if (playerInRooms == null)
            playerInRooms = new PlayerInRoom[7];
        Character ch;
        ch.isEmpty = true;
        ch.animal = "";
        ch.cup = "";
        ch.liquid = "";
        ch.name = "";
        AIList = new Character[7] { ch, ch, ch, ch, ch, ch, ch };
        sitHasPlayer = new bool[] { true, true, true, true, true, true, true };
        RoomIDText.text = "#" + PhotonNetwork.room.Name;
        if (PhotonNetwork.isMasterClient)
        {
            foreach (GameObject item in addAIButton)
            {
                item.SetActive(true);
            }
        }
        InitialMap();
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.IsOpen = true;
            PhotonNetwork.room.IsVisible = true;
        }
        if (!PhotonNetwork.isMasterClient)
        {
            StartGameButton.sprite = ReadyButtonSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //print(PhotonNetwork.inRoom);
    }

    public void LeaveRoom()
    {
        MainUIManager.CreateDialogBox(() => PhotonManager.getInstance().LeaveRoom(), "確定要離開房間嗎？");
    }

    public void SetPlayerSit(PlayerInRoom player, GameObject obj)
    {
        CheckList();
        for (int i = 0; i < sitHasPlayer.Length; i++)
        {
            if (sitHasPlayer[i])
            {
                SetAddAIButtonActive(i, false);
                //playerID[i] = obj.GetPhotonView().ownerId;
                SetSit(player, obj, i);
                return;
            }
        }
    }

    public void SetAddAIButtonActive(int index, bool active)
    {
        if (PhotonNetwork.isMasterClient && !isStartGame)
        {
            if(addAIButton != null)
                if(addAIButton[index].gameObject != null)
                    addAIButton[index].gameObject.SetActive(active);
        }
    }

    public void setPlayerImage(Sprite animal, Sprite cup, Sprite liquid)
    {
        playerAnimal.sprite = animal;
        playerCup.sprite = cup;
        playerLiquid.sprite = liquid;
    }

    public void OnStartGameClicked()
    {
        if (PhotonNetwork.isMasterClient)
        {
            CheckList();
            if (playerCount < 2)
            {
                MainUIManager.CreateCheckBox(() => { }, "至少要兩位玩家才能進行遊戲喔！\n試著加入電腦吧！");
                return;
            }
            if (IsEveryoneReady())
            {
                isStartGame = true;
                AudioManager.Play("startgame");
                PhotonManager.AICount = AICount;
                PhotonManager.AIList = AIList;
                //PhotonManager.getInstance().OnClickedStart();
                PhotonNetwork.automaticallySyncScene = false;
                PhotonNetwork.room.IsOpen = false;
                localPlayer.gameObject.GetPhotonView().RPC("StartGame", PhotonTargets.All, AICount);
            }
            else
            {
                MainUIManager.CreateMiniCheckBox(() => { }, "必須等待玩家皆準備完成才能開始遊戲");
            }
        }
        else
        {
            if (localPlayer.isReady)
            {
                if (localPlayer != null)
                {
                    localPlayer.gameObject.GetPhotonView().RPC("Ready", PhotonTargets.AllBuffered, false);
                    StartGameButton.sprite = ReadyButtonSprite;
                }
            }
            else
            {
                if (localPlayer != null)
                {
                    localPlayer.gameObject.GetPhotonView().RPC("Ready", PhotonTargets.AllBuffered, true);
                    StartGameButton.sprite = CancelButtonSprite;
                }
            }
        }
    }
    public void AddAI(int index)
    {
        AudioManager.Play("buttonclick");
        GameObject ai = PhotonNetwork.Instantiate("AIInRoom", Vector2.zero, Quaternion.identity, 0, null);
        SetAddAIButtonActive(index, false);
        SetSit(ai.GetComponent<PlayerInRoom>(),ai, index);
        Character newAI = RandomAI();
        if (newAI.animal == "cat")
            newAI.name = "吉米";
        else if (newAI.animal == "dog")
            newAI.name = "阿福";
        else
            newAI.name = "小萌";
        AIList[index] = newAI;
        ai.GetPhotonView().RPC("SetCharacterSprite", PhotonTargets.AllBuffered, newAI.animal, newAI.cup, newAI.liquid, newAI.name);
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventdata) => RemoveAI(index));
        ai.GetComponent<EventTrigger>().triggers.Add(entry);
        AICount++;
        CheckList();
    }


    public void SetSit(PlayerInRoom player,GameObject obj, int i)
    {
        SetAddAIButtonActive(i, false);
        //playerID[i] = obj.GetPhotonView().ownerId;
        playerInRooms[i] = player;
        obj.transform.SetParent(playerSits[i].transform);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        obj.transform.localScale = Vector3.one;
        sitHasPlayer[i] = false;
        PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable(2) { { "Map", MapList[nowMapIndex] }, { "PlayerCount", playerCount } });
    }

    public void RemoveAI(int index)
    {
        PhotonNetwork.Destroy(playerInRooms[index].gameObject);
        playerInRooms[index] = null;
        SetAddAIButtonActive(index, true);
        AIList[index].isEmpty = true;
        CheckList();
        AICount--;
    }

    Character RandomAI()
    {
        Character newAI;
        newAI.animal = animal[Random.Range(0, 3)];
        newAI.cup = cup[Random.Range(0, 3)];
        newAI.liquid = liquid[Random.Range(0, 3)];
        newAI.isEmpty = false;
        newAI.name = "";
        return newAI;
    }

    public void CheckList()
    {
        if (playerInRooms == null)
            playerInRooms = new PlayerInRoom[7];
        bool hasSit = false;
        int nowPlayerCount = 1;
        for (int i = 0; i < playerInRooms.Length; i++)
        {
            if (playerInRooms[i] == null)
            {
                sitHasPlayer[i] = true;
                SetAddAIButtonActive(i, true);
                hasSit = true;
            }
            else
            {
                sitHasPlayer[i] = false;
                SetAddAIButtonActive(i, false);
                nowPlayerCount++;
            }
        }
        if (PhotonNetwork.isMasterClient)
        {
            if (hasSit)
            {
                PhotonNetwork.room.IsOpen = true;
            }
            else
            {
                PhotonNetwork.room.IsOpen = false;
            }
            playerCount = nowPlayerCount;
            PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable(2) { { "Map", MapList[nowMapIndex] }, { "PlayerCount", playerCount } });
        }
    }

    public void CheckList(PlayerInRoom pl)
    {
        if (playerInRooms == null)
            playerInRooms = new PlayerInRoom[7];
        bool hasSit = false;
        int nowPlayerCount = 1;
        for (int i = 0; i < playerInRooms.Length; i++)
        {
            if (playerInRooms[i] == null || playerInRooms[i] == pl)
            {
                sitHasPlayer[i] = true;
                SetAddAIButtonActive(i, true);
                hasSit = true;
            }
            else
            {
                sitHasPlayer[i] = false;
                SetAddAIButtonActive(i, false);
            }
        }
        if (PhotonNetwork.isMasterClient)
        {
            if (hasSit)
            {
                PhotonNetwork.room.IsOpen = true;
            }
            else
            {
                PhotonNetwork.room.IsOpen = false;
            }
            playerCount = nowPlayerCount;
            PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable(2) { { "Map", MapList[nowMapIndex] }, { "PlayerCount", playerCount } });
        }
    }
    public void Next()
    {
        if(PhotonNetwork.isMasterClient)
            localPlayer.gameObject.GetPhotonView().RPC("NextMap", PhotonTargets.AllBuffered);
    }

    public void Previous()
    {
        if (PhotonNetwork.isMasterClient)
            localPlayer.gameObject.GetPhotonView().RPC("PreviousMap", PhotonTargets.AllBuffered);
    }

    public void NextMap()
    {
        if (isSlidingMap)
            return;
        int targetIndex = 0;
        if (nowMapIndex + 1 >= MapList.Count)
        {
            targetIndex = nowMapIndex + 1;
            nowMapIndex = 0;
        }
        else
        {
            nowMapIndex++;
            targetIndex = nowMapIndex;
        }
        ChangeMap(targetIndex);
        if(PhotonNetwork.isMasterClient)
            PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable(2) { { "Map", MapList[nowMapIndex] }, { "PlayerCount", playerCount } });
    }

    public void PreviousMap()
    {
        if (isSlidingMap)
            return;
        int targetIndex = 0;
        if (nowMapIndex <= 0)
        {
            targetIndex = nowMapIndex - 1;
            nowMapIndex = MapList.Count - 1;
        }
        else
        {
            nowMapIndex--;
            targetIndex = nowMapIndex;
        }
        ChangeMap(targetIndex);
        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable(2) { { "Map", MapList[nowMapIndex] }, { "PlayerCount", playerCount } });
    }

    void InitialMap()
    {
        for (int i = 0; i < MapList.Count; i++)
        {
            AddMap(MapImageList[i], i);
        }
        AddMap(MapImageList[0], MapImageList.Count);
        AddMap(MapImageList[MapImageList.Count - 1], -1);

        string nowMap = (string)PhotonNetwork.room.CustomProperties["Map"];
        int index = nowMapIndex;
        for (int i = 0; i < MapList.Count; i++)
        {
            if (MapList[i] == nowMap)
                ChangeMapInstantly(i);
        }
    }

    public void ChangeMapInstantly(int targetIndex)
    {
        nowMapIndex = targetIndex;
        float targetX = -((float)targetIndex * itemWidth);
        RectTransform rect = MapContainer.gameObject.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(targetX, rect.anchoredPosition.y);
    }

    void AddMap(GameObject obj, int index)
    {
        RectTransform rect = Instantiate(obj, MapContainer.transform).GetComponent<RectTransform>();
        float x = index * itemWidth;
        rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
    }

    public void ChangeMap(int targetIndex)
    {
        isSlidingMap = true;
        CoroutineUtility.GetInstance()
            .Do()
            .DoEnumerator(Slide(MapContainer.gameObject, targetIndex))
            .Then(() => MoveMapPos(MapContainer.gameObject, nowMapIndex))
            .Then(() => isSlidingMap = false)
            .Go();
    }

    void MoveMapPos(GameObject obj, int targetIndex)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        float targetX = -((float)targetIndex * itemWidth);
        rect.anchoredPosition = new Vector2(targetX, rect.anchoredPosition.y);
    }

    IEnumerator Slide(GameObject obj, int targetIndex)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        int t;
        float targetX = -((float)targetIndex * itemWidth);
        float relativeX = targetX - rect.anchoredPosition.x;
        if (relativeX >= 0)
            t = 1;
        else
            t = -1;

        while (relativeX * t > 2)
        {
            float x = Mathf.Lerp(rect.anchoredPosition.x, targetX, Time.deltaTime * 10);
            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
            relativeX = targetX - rect.anchoredPosition.x;
            yield return null;
        }
        rect.anchoredPosition = new Vector2(targetX, rect.anchoredPosition.y);
    }

    public bool IsEveryoneReady()
    {
        foreach (PlayerInRoom pl in playerInRooms)
        {
            if (pl != null)
            {
                if (!pl.isReady)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void OnPlayerClickMap(bool isClick)
    {
        if (isClick && !isClickedMap)
        {
            isClickedMap = isClick;
            StartCoroutine(clickMap());
        }
    }

    IEnumerator clickMap()
    {
        while (isClickedMap)
        {
            clickTime += Time.deltaTime;
            if (clickTime > 1)
            {
                if (MapList[nowMapIndex] == "Picnic")
                    MainUIManager.CreateMiniCheckBox(() => { }, "無特殊效果");
                else if (MapList[nowMapIndex] == "Table")
                    MainUIManager.CreateMiniCheckBox(() => { }, "檸檬大量出現");
                else
                    MainUIManager.CreateMiniCheckBox(() => { }, "極速狂飆");
                break;
            }
            yield return null;
        }
        clickTime = 0;
        isClickedMap = false;

    }
}

[System.Serializable]
public struct Character
{
    public bool isEmpty;
    public string animal;
    public string cup;
    public string liquid;
    public string name;
}