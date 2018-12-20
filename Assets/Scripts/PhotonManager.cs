using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonManager : Photon.PunBehaviour {
    string versionName = "1.0";
    // Use this for initialization

    public GameObject playerObj;
    public static PhotonManager instance;
    public List<PlayerController> playerList;
    public static int AICount;
    public static Character[] AIList;
    public Map map;
    static bool IsTryingReconnecting;
    int nowMasterID;
    bool isCreatingTutorial;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);  
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
        playerObj = Resources.Load<GameObject>("PlayerInGame");
    }

    public static PhotonManager getInstance() {
        if (instance == null)
        {
            GameObject photonManager = new GameObject();
            Instantiate(photonManager);
            photonManager.AddComponent<PhotonManager>();
            return instance;
        }
        else
            return instance;
    }

    void Start() {
        PhotonNetwork.sendRate = 120;
        PhotonNetwork.sendRateOnSerialize = 120;
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKey(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "MainGame")
            {
                if (MainGameManager.instance != null)
                {
                    if(MainGameManager.getInstance().player == null)
                        MainUIManager.CreateDialogBoxInGame(() => MainGameManager.getInstance().BackToRoomList(), () => { }, "遊戲還在進行中\n確定要離開房間嗎？");
                    else if (MainGameManager.getInstance().player.isDied)
                        MainUIManager.CreateDialogBoxInGame(() => MainGameManager.getInstance().ShowScoreBoard(), () => { }, "遊戲還在進行中\n確定要離開房間嗎？");
                    else
                        MainUIManager.CreateDialogBoxInGame(() => MainGameManager.getInstance().BackToRoomList(), () => { }, "遊戲還在進行中\n確定要離開房間嗎？");
                }
            }
            else if (SceneManager.GetActiveScene().name == "Tutorial")
            {
                if (MainGameManager.instance != null)
                {
                    MainUIManager.CreateDialogBoxInGame(() => MainGameManager.getInstance().BackToRoomList(), () => { }, "確定要跳過新手教學嗎？");
                }
            }
            else
            {
                MainUIManager.CreateDialogBox(() => Application.Quit(), () => { }, "確定要離開 Liquid Knight嗎？？");
            }
        }
    }

    private void FixedUpdate()
    {

    }

    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings(versionName);

        //PhotonNetwork.ConnectToBestCloudServer(versionName);
    }

    override
    public void OnConnectedToMaster() {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("連接Photon成功");
        //LobbyUIManager.instance.changeLobbyText("正在加入遊戲大廳...");
    }

    override
    public void OnJoinedLobby()
    {
        IsTryingReconnecting = false;
        PhotonNetwork.player.NickName = EntireGameManager.getInstance().playerData.name;
        Debug.Log(PhotonNetwork.player.NickName);
        //PhotonNetwork.autoCleanUpPlayerObjects = false;
        //LobbyUIManager.instance.changeLobbyText("正在加入房間...");
    }

    public void joinRoom(string RoomName) {
        PhotonNetwork.JoinRoom(RoomName);
    }

    public void RandomJoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void createRoom() {
        isCreatingTutorial = false;
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        int max = 0;
        for (int i = 0; i < rooms.Length; i++)
        {
            if (int.Parse(rooms[i].Name) > max)
                max = int.Parse(rooms[i].Name);
        }
        createRoom(max+1);
    }

    public void CreateTutorial()
    {
        isCreatingTutorial = true;
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        int max = 0;
        for (int i = 0; i < rooms.Length; i++)
        {
            if (int.Parse(rooms[i].Name) > max)
                max = int.Parse(rooms[i].Name);
        }
        string roomName = (max+1).ToString();
        RoomOptions newRoomOptions = new RoomOptions();
        newRoomOptions.IsVisible = true;
        newRoomOptions.IsOpen = false;
        newRoomOptions.MaxPlayers = 1;        
        newRoomOptions.CustomRoomPropertiesForLobby = new string[] { "Map" };
        newRoomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable(2) { { "Map", "Picnic" }, { "isTutorial",true } }; // add this line
        //newRoomOptions.CustomRoomProperties.Add("Map", "dd");
        PhotonNetwork.CreateRoom(roomName, newRoomOptions, null);
    }

    override
    public void OnJoinedRoom() {
        //LobbyUIManager.instance.changeNumberView(PhotonNetwork.room.PlayerCount);
        //if (PhotonNetwork.isMasterClient)
        //    LobbyUIManager.instance.setStartButtonState(true);
        Debug.Log("OnJoinRoom");
        if (EntireGameManager.nextScene != "Tutorial")
            PhotonNetwork.LoadLevel("Room");
        //PhotonNetwork.LoadLevel("MainGame");
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        base.OnPhotonCreateRoomFailed(codeAndMsg);
        if (isCreatingTutorial)
        {
            Debug.Log("fail create tutorial");
            CreateTutorial();
        }
        else
        {
            Debug.Log("fail create room");
            createRoom();
        }
    }

    public void createRoom(int roomID)
    {
        Debug.Log("CreateNewRoom");
        string roomName = roomID.ToString();
        RoomOptions newRoomOptions = new RoomOptions();
        newRoomOptions.EmptyRoomTtl = 0;
        newRoomOptions.MaxPlayers = 8;
        newRoomOptions.IsOpen = true;
        newRoomOptions.IsVisible = true;
        newRoomOptions.CustomRoomPropertiesForLobby = new string[] { "Map", "PlayerCount" };
        newRoomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable(2) { { "Map", "Picnic" }, { "PlayerCount", 1 } };
        //newRoomOptions.CustomRoomProperties.Add("Map", "dd");
        PhotonNetwork.CreateRoom(roomName, newRoomOptions, null);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        PhotonNetwork.automaticallySyncScene = false;
    }


    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("loadscene");
        if (scene.name == "Room")
        {
            PhotonNetwork.BackgroundTimeout = 180;
            PhotonNetwork.automaticallySyncScene = false;
            PhotonNetwork.isMessageQueueRunning = true;
            if(playerList != null)
                playerList.Clear();
            spawnRoomPlayer();
            SocialManager.ReadQueue(this);
        }
        else if (scene.name == "MainGame")
        {
            if (PhotonNetwork.isMasterClient)
                PhotonNetwork.BackgroundTimeout = 10;
            map = MainGameManager.getInstance().InitialMap((string)PhotonNetwork.room.CustomProperties["Map"]);
            CoroutineUtility.GetInstance()
                .Do()
                .Wait(0.2f)
                .Then(() => spawnPlayer(playerObj, map.spawnPoint))
                .Go();
            //spawnPlayer(playerObj, map.spawnPoint);
        }
        else if (scene.name == "Tutorial")
        {
            map = MainGameManager.getInstance().InitialMap((string)PhotonNetwork.room.CustomProperties["Map"]);
        }
        else if (scene.name == "RoomList")
        {
            SocialManager.ReadQueue(this);
        }
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.inRoom)
        {
            PhotonNetwork.LeaveRoom();
            StartCoroutine(EntireGameManager.LoadSceneWithLoadingScene("RoomList"));
        }
    }

    void spawnRoomPlayer()
    {
        GameObject obj = PhotonNetwork.Instantiate("PlayerInRoom", Vector2.zero, Quaternion.identity, 0);
        RoomManager.myPlayerInRoom = obj.GetComponent<PlayerInRoom>();

        RoomManager.getInstance().localPlayer = RoomManager.myPlayerInRoom;
    }

    public void spawnPlayer(GameObject playerObject, List<Vector2> spawnPoints)
    {
        //nowMasterID = PhotonNetwork.masterClient.ID;
        Debug.Log(playerObj.name);
        Debug.Log(PhotonNetwork.inRoom);
        Debug.Log(PhotonNetwork.masterClient.ID);
        if(playerList == null)
            playerList = new List<PlayerController>();
        if (PhotonNetwork.isMasterClient)
        {
            int i = 0;
            for (; i < PhotonNetwork.playerList.Length; i++)
            {
                Vector3 spawnPoint = spawnPoints[i];
                if (SceneManager.GetActiveScene().name == "Tutorial")
                    spawnPoint = Vector3.zero;
                spawnPoint.z = -1;
                GameObject playerObj = PhotonNetwork.InstantiateSceneObject(playerObject.name, spawnPoint, playerObject.transform.rotation,0,null);
                PlayerController playerController = playerObj.GetComponent<PlayerController>();
                //playerList.Add(playerController);
                int id = PhotonNetwork.playerList[i].ID;
                string nickName = PhotonNetwork.playerList[i].NickName;
                Debug.Log(nickName);
                playerController.canMove = false;
                playerObj.GetPhotonView().RPC("Initial", PhotonTargets.All, i, id, false, nickName);
                //playerController.Initial();
            }
            i = PhotonNetwork.playerList.Length;
            if (AIList == null)
                return;
            for(int a = 0; a < AIList.Length; a++)
            {
                Vector3 spawnPoint = spawnPoints[i];
                spawnPoint.z = -1;
                Character ch = AIList[a];
                if (ch.isEmpty)
                    continue;
                Debug.Log(ch.name);
                GameObject playerObj = PhotonNetwork.InstantiateSceneObject(playerObject.name, spawnPoint, playerObject.transform.rotation, 0, null);
                PlayerController playerController = playerObj.GetComponent<PlayerController>();
                //playerList.Add(playerController);
                int id = -a;
                playerController.canMove = false;
                playerObj.GetPhotonView().RPC("Initial", PhotonTargets.All, i, id, true, ch.name);
                playerObj.GetPhotonView().RPC("SetCharacterSprite", PhotonTargets.All, ch.animal, ch.cup, ch.liquid);
                playerObj.GetPhotonView().RPC("SetSkill", PhotonTargets.All, ch.animal);
                i++;
            }
            ScoreManager.getInstance().InitialCount(i);
        }
    }

    public void updatePlayerInRoom() {
    }

    override
    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
        
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (SceneManager.GetActiveScene().name == "Room" && PhotonNetwork.isMasterClient)
        {
            RoomManager.getInstance().CheckList();
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.Log("連線中斷");
        base.OnDisconnectedFromPhoton();
        if (SceneManager.GetActiveScene().name == "RoomList" || SceneManager.GetActiveScene().name == "Shop")
        {
            PhotonNetwork.Reconnect();
            Debug.Log("嘗試重新連接");
        }
        else
        {
            if(SceneManager.GetActiveScene().name == "MainGame" || SceneManager.GetActiveScene().name == "Tutorial")
                MainUIManager.CreateMainGameCheckBox(() => ReStartWholeGame(), "與伺服器連線中斷\n點擊OK回到主畫面");
            else
                MainUIManager.CreateCheckBox(() => ReStartWholeGame(), "與伺服器連線中斷\n點擊OK回到主畫面");            
        }
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        base.OnConnectionFail(cause);
        if (IsTryingReconnecting)
        {
            MainUIManager.CreateCheckBox(() => ReStartWholeGame(), "與伺服器連線中斷，點擊OK回到主畫面");
        }

    }

    public void ReStartWholeGame()
    {
        AIList = null;
        playerList = null;
        CoroutineUtility.GetInstance().Do()
            .Then(() => SceneManager.sceneLoaded -= OnSceneFinishedLoading)
            .Then(() => Destroy(this))
            .Then(() => SceneManager.LoadScene("FirstScene"))
            .Go();
        //Destroy(gameObject);
    }

    public bool isInLobby() {
        return PhotonNetwork.insideLobby;
    }

    public RoomInfo[] GetRoomList()
    {
        return PhotonNetwork.GetRoomList();
    }

    public override void OnReceivedRoomListUpdate()
    {
        base.OnReceivedRoomListUpdate();
        if (RoomListManager.instance != null)
            RoomListManager.instance.UpdateRoomList();
    }

    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        if (SceneManager.GetActiveScene().name == "MainGame")
        {
            MainUIManager.CreateMainGameCheckBox(() => BackToRoomList(), "房主解散房間\n準備回到遊戲大廳");
        }
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        base.OnPhotonJoinRoomFailed(codeAndMsg);
        Debug.Log("加入房間失敗");
    }

    public PlayerController FindPlayerByID(int id)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].ID == id)
            {
                return playerList[i];
            }
        }
        return null;
    }

    public void BackToRoomList()
    {
        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LoadLevel("RoomList");
        StartCoroutine(EntireGameManager.LoadSceneWithLoadingScene("RoomList"));
    }
}
