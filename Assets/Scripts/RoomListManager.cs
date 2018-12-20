using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListManager : MonoBehaviour {
    [SerializeField]RectTransform content;

    [SerializeField]GameObject roomItem;

    RoomInfo[] roomList;

    int itemHeight = 140;

    List<GameObject> roomItemList;

    public static RoomListManager instance;

    public Image playerAnimalImg;
    public Image playerCupImg;
    public Image playerLiquidImg;
    public GameObject EnterNameUI;
    public InputField EnterRoomIDUI;
    public string filter;
    // Use this for initialization
    private void Awake()
    {
        filter = "";
        instance = this;
        if (EntireGameManager.getInstance().playerData.name == "吉米")
        {
            GameObject enterNameObj = Instantiate(EnterNameUI, GameObject.Find("Canvas").transform);
            enterNameObj.transform.SetAsLastSibling();
        }
    }


    void Start () {
        roomItemList = new List<GameObject>();
        UpdateRoomList();
        CreateCharacterImage(EntireGameManager.getInstance().getPlayerData().nowEquipment);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void CreateCharacterImage(Equipment equip)
    {
        Sprite animal = Resources.Load<Sprite>("Character/SelectView/Animals/" + equip.animal);
        Sprite cup = Resources.Load<Sprite>("Character/SelectView/cup/" + equip.cup);
        Sprite liquid = Resources.Load<Sprite>("Character/SelectView/liquid/" + equip.liquid);

        playerAnimalImg.sprite = animal;
        playerCupImg.sprite = cup;
        playerLiquidImg.sprite = liquid;
    }

    public void UpdateRoomList()
    {
        while (roomItemList.Count > 0)
        {
            Destroy(roomItemList[0]);
            roomItemList.Remove(roomItemList[0]);
        }

        roomList = PhotonManager.getInstance().GetRoomList();
        if (roomList.Length <= 0)
            return;

        int posY = 0;
        for (int i = 0; i < roomList.Length; i++)
        {

            if (roomList[i].Name != filter && filter != string.Empty)
                continue;
            if ((bool)roomList[i].CustomProperties.ContainsKey("isTutorial"))
                continue;
            if (!roomList[i].IsOpen)
                continue;
            string name = roomList[i].Name;
            string map = (string)roomList[i].CustomProperties["Map"];
            int maxPlayers = roomList[i].MaxPlayers;
            //int nowPlayers = roomList[i].PlayerCount;
            int nowPlayers = 1;
            if (roomList[i].CustomProperties.ContainsKey("PlayerCount"))
                nowPlayers = (int)roomList[i].CustomProperties["PlayerCount"];
            addRoomItemToList(name, map, maxPlayers, nowPlayers,posY);

            posY -= itemHeight;

            content.sizeDelta = new Vector2(content.sizeDelta.x, -posY);
        }
    }

    public void addRoomItemToList(string name, string map, int maxPlayers, int nowPlayers, int posY)
    {
        GameObject newItem = Instantiate(roomItem, content.transform);
        roomItemList.Add(newItem);
        RectTransform rect = newItem.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector3(0, posY, 0);
        RoomItem item = newItem.GetComponent<RoomItem>();
        item.InitialItem(name, map, maxPlayers, nowPlayers);
        content.sizeDelta = new Vector2(content.sizeDelta.x, -posY + itemHeight);

    }

    public void CreateNewRoom()
    {

        Debug.Log(PhotonNetwork.connectionState);
        if (PhotonManager.getInstance().isInLobby())
        {
            PhotonManager.getInstance().createRoom();
        }
    }

    public void EnterShop()
    {
        StartCoroutine(EntireGameManager.LoadSceneWithLoadingScene("Shop"));
    }

    public void OpenAchievementPanel()
    {
        Social.ShowAchievementsUI();
    }

    public void OnTextFieldValueChange()
    {
        filter = EnterRoomIDUI.text;
        UpdateRoomList();
    }

    public void RandomJoin()
    {
        PhotonManager.getInstance().RandomJoinRoom();
    }
}
