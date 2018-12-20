using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerInRoom : MonoBehaviour {
    public Image cupImage;
    public Image animalImage;
    public Image liquidImage;
    public Text NameText;
    public Text isReadyText;
    public GameObject characterObj;
    public PhotonView photonView;
    public bool isAI;
    public bool isReady;
    // Use this for initialization
    private void Awake()
    {
        //if(!isAI)
        //    SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Start() {
        photonView = gameObject.GetPhotonView();
        Initial();
        //if(!isAI)
        //    DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initial()
    {
        if (gameObject.GetPhotonView().isMine && !isAI)
        {
            RoomManager.getInstance().localPlayer = this;
            SaveData playerData = EntireGameManager.getInstance().playerData;
            SetCharacterSprite(playerData.nowEquipment.animal, playerData.nowEquipment.cup, playerData.nowEquipment.liquid, playerData.name);
            photonView.RPC("SetCharacterSprite", PhotonTargets.AllBuffered, playerData.nowEquipment.animal, playerData.nowEquipment.cup, playerData.nowEquipment.liquid, playerData.name);
        }
        else
        {
            characterObj.SetActive(true);
            NameText.gameObject.SetActive(true);
            isReadyText.gameObject.SetActive(true);
            if (!PhotonNetwork.isMasterClient || !isAI)
            {
                RoomManager.getInstance().SetPlayerSit(this, gameObject);
            }
        }
        if (PhotonNetwork.isMasterClient)
        {
            if (isAI)
            {
                gameObject.GetPhotonView().RPC("Ready", PhotonTargets.AllBuffered, true);
            }
            else if (gameObject.GetPhotonView().owner.IsMasterClient)
            {
                gameObject.GetPhotonView().RPC("Ready", PhotonTargets.AllBuffered, true);
            }
            else
                gameObject.GetPhotonView().RPC("Ready", PhotonTargets.AllBuffered, false);
        }
    }

    //public void StartGameClicked()
    //{
    //    photonView.RPC("StartGame", PhotonTargets.MasterClient, null);
    //}

    #region RPC Functions
    [PunRPC]
    public void SetCharacterSprite(string animal, string cup, string liquid, string name)
    {
        Sprite animalSprite = Resources.Load<Sprite>("Character/SelectView/animals/" + animal);
        Sprite cupSprite = Resources.Load<Sprite>("Character/SelectView/cup/" + cup);
        Sprite liquidSprite = Resources.Load<Sprite>("Character/SelectView/liquid/" + liquid);
        string playerName = name;
        animalImage.sprite = animalSprite;
        cupImage.sprite = cupSprite;
        liquidImage.sprite = liquidSprite;
        NameText.text = name;
        if (gameObject.GetPhotonView().isMine && !isAI)
            RoomManager.getInstance().setPlayerImage(animalSprite, cupSprite, liquidSprite);
    }

    [PunRPC]
    public void StartGame(int aiCount)
    {
        Debug.Log("StartGameeeeeeeeeeeee");
        RoomManager.getInstance().isStartGame = true;
        PhotonManager.AICount = aiCount;
        EntireGameManager.nextScene = "MainGame";
        PhotonNetwork.LoadLevel("LoadingScene");
        //PhotonManager.getInstance().OnClickedStart();
    }

    [PunRPC]
    public void Ready(bool isReady)
    {
        this.isReady = isReady;
        if (gameObject.GetPhotonView().owner != null)
        {
            if (gameObject.GetPhotonView().owner.IsMasterClient && !isAI)
            {
                isReadyText.color = RoomManager.getInstance().readyColor;
                isReadyText.text = "房主";
                return;
            }
        }
        if (isReady)
        {
            isReadyText.color = RoomManager.getInstance().readyColor;
            isReadyText.text = "準備完成";
        }
        else
        {
            isReadyText.color = RoomManager.getInstance().unReadyColor;
            isReadyText.text = "準備中";
        }
    }

    #endregion

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Room")
        {
            CoroutineUtility.GetInstance().Do().Wait(0.5f).Then(Initial).Go();
        }
    }

    private void OnDestroy()
    {
        if (RoomManager.instance != null && !isAI)
        {
            RoomManager.getInstance().CheckList(this);
            Debug.Log("died");
        }
    }

    [PunRPC]
    public void NextMap()
    {
        RoomManager.getInstance().NextMap();
    }

    [PunRPC]
    public void PreviousMap()
    {
        RoomManager.getInstance().PreviousMap();
    }
}
