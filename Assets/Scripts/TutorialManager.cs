using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour {
    // Use this for initialization
    public List<GameObject> TutorialUI;
    public PlayerController tutorialAI;
    public PlayerController player;
    public List<GameObject> objectToSpawn;
    public int index;
    public static TutorialManager instance;
    public GameObject skillButton;
    public bool HaveUseSkill;
    public GameObject itemFactory;

    public static TutorialManager GetInstance()
    {
        if (instance == null)
        {
            instance = new GameObject("TutorialManager").AddComponent<TutorialManager>();
        }
        return instance;
    }

    private void Awake()
    {
        instance = this;
        HaveUseSkill = false;
    }

    void Start () {
        index = 0;
        CreateTutorialUI(0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetPlayerCanMove(bool can)
    {
        player.canMove = can;
    }

    public void SpawnPlayer()
    {
        PhotonManager.getInstance().spawnPlayer(PhotonManager.getInstance().playerObj, PhotonManager.getInstance().map.spawnPoint);
        ScoreManager.getInstance().InitialCount(2);
    }

    public void StopPlayer()
    {
        player.playerRig.velocity = Vector2.zero;
        CoroutineUtility.GetInstance().Do().Wait(0.3f).Then(() => player.playerRig.velocity = Vector2.zero);
    }

    public void SetPlayerPosition(Vector3 pos)
    {
        player.transform.position = pos;
        Vector3 camPos = pos;
        camPos.z = -10;
        player.mainCamera.transform.position = camPos;        
    }

    public void SetAIActive(bool active)
    {
        tutorialAI.gameObject.SetActive(active);
    }

    public void SetAIPosition(Vector3 pos)
    {
        tutorialAI.transform.position = pos;
    }

    public void StopAI()
    {
        tutorialAI.playerRig.velocity = Vector2.zero;
    }

    public void MoveNext()
    {
        TutorialUI[index].SetActive(false);
        index++;
        if(index < TutorialUI.Count)
            CreateTutorialUI(index);
    }

    public void SetPlayerCanUseSkill(bool canUse)
    {
        MainGameManager.getInstance().canUseSkill = canUse;
    }

    public void PauseNowTutorialUI()
    {
        TutorialUI[index].SetActive(false);
    }
    public void CreateTutorialUI(int i)
    {
        TutorialUI[i].SetActive(true);
    }

    public void SpawnTutorialAI()
    {
        Vector3 pos = new Vector3(0, 0, -1) - player.transform.position;
        Vector3 spawnPoint = player.transform.position + (pos.normalized * 6);
        spawnPoint.z = -1;
        Character ch;
        ch.animal = "dog";
        ch.cup = "teacup";
        ch.liquid = "wine";
        ch.name = "阿福";
        GameObject playerObj = PhotonNetwork.InstantiateSceneObject(PhotonManager.getInstance().playerObj.name, spawnPoint, PhotonManager.getInstance().playerObj.transform.rotation, 0, null);
        PlayerController playerController = playerObj.GetComponent<PlayerController>();
        TutorialObject tutorialObj = playerObj.AddComponent<TutorialObject>();
        tutorialObj.triggerType = TriggerType.OnCollisionEnter;
        if(tutorialObj.TriggerTurtorialEvent == null)
            tutorialObj.TriggerTurtorialEvent = new UnityEvent();
        tutorialObj.TriggerTurtorialEvent.AddListener(MoveNext);
        tutorialObj.TriggerTurtorialEvent.AddListener(() => SetPlayerCanMove(false));
        tutorialObj.TriggerTurtorialEvent.AddListener(() => player.Damaged(8));
        tutorialObj.TriggerTurtorialEvent.AddListener(StopPlayer);
        tutorialObj.TriggerTurtorialEvent.AddListener(() => CoroutineUtility.GetInstance().Do().Wait(1f).Then(StopAI).Go());
        tutorialObj.TriggerTurtorialEvent.AddListener(() => CoroutineUtility.GetInstance().Do().Wait(0.2f).Then(() => Destroy(tutorialObj)).Go());
        //playerList.Add(playerController);

        PhotonManager.getInstance().playerList.Add(playerController);
        int id = -1;
        playerController.willDamaged = false;
        playerObj.GetPhotonView().RPC("Initial", PhotonTargets.AllBuffered, 2, id, true,ch.name);
        playerObj.GetPhotonView().RPC("SetCharacterSprite", PhotonTargets.AllBuffered, ch.animal, ch.cup, ch.liquid);
        playerObj.GetPhotonView().RPC("SetSkill", PhotonTargets.AllBuffered, ch.animal);
        playerController.canMove = false;
        tutorialAI = playerController;
    }

    public void CanAIMove(bool can)
    {
        tutorialAI.canMove = can;
    }

    public void SpawnObject(int index)
    {
        Instantiate(objectToSpawn[index], objectToSpawn[index].transform.position, Quaternion.identity);
        StopPlayer();
        SetPlayerCanMove(false);
    }

    public void ShowHealthBarUI()
    {
        MainUIManager.instance.healthBar.gameObject.transform.SetAsLastSibling();
    }

    public void HideHealthBarUI()
    {
        MainUIManager.instance.healthBar.gameObject.transform.SetAsFirstSibling();
    }

    public void ShowSkillButtonUI()
    {
        skillButton.transform.SetAsLastSibling();
    }

    public void HideSkillButtonUI()
    {
        skillButton.transform.SetAsFirstSibling();
    }

    public void SetPlayerWillDamaged(bool will)
    {
        player.willDamaged = will;
    }
    public void SetAIWillDamaged(bool will)
    {
        tutorialAI.willDamaged = will;
    }

    public void OnSkillFirstUsed(float waitTime)
    {

        if (!MainGameManager.getInstance().canUseSkill)
            return;
        if (index != 12)
            return;
        if (HaveUseSkill)
            return;
        HaveUseSkill = true;
        CoroutineUtility.GetInstance().Do().Wait(waitTime).Then(() => SetPlayerCanMove(false)).Then(StopPlayer).Then(() => CanAIMove(false)).Then(StopAI).Then(MoveNext).Go();
    }

    public void SetAIHealth(int health)
    {
        tutorialAI.hp = health;
    }

    public void FillSkillBar()
    {
        player.AddEnegy(100);
    }

    public void EnableItemFactory()
    {
        itemFactory.SetActive(true);
    }
}