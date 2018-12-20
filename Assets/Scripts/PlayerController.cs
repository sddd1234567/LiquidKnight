using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerController : Photon.PunBehaviour {

    [SerializeField] Joystick joystick;
    public GameObject mainCamera;
    [SerializeField] GameObject crashEffect;
    public Rigidbody2D playerRig;
    Vector3 target;
    public float maxVelcocity = 22;
    public float maxMaxSpeed = 28;
    float nowSpriteZ = 0;
    transformInfo newInfo;
    //PhotonView photonView;
    public Vector2 lastVelocity;
    public int hp = 100;
    public float mp = 0;
    public float rotateSpeed = 360;
    public Vector2 forceFoward;
    public GameObject playerSprite;
    public Skill skill;
    public GameObject DeadAnimation;

    public int ID;

    public FakeInputer fakeInputer;

    public float masterClientTTL = 0;

    public int speedBuff = 0;
    public int index;

    public GameObject SpawnAnimation;

    public bool canUseSkill;
    public Animator playerAnimator;
    public string nowIdleAnim;
    #region Character Sprites
    [SerializeField] Sprite animalSprite;
    Sprite cupSprite;
    Sprite liquidSprite;
    Sprite middleHealthAnimal;
    Sprite lowHealthAnimal;
    float AITime = 3;
    public bool isAI;
    public bool canMove;
    public SpriteRenderer MinimapIconSprite;
    public string animal;
    public string cup;
    public string liquid;
    public bool isDied;
    public string NickName;
    public bool willDamaged;
    bool isAIFocusEnemy = false;
    PlayerController AIFocusEnemy;
    public PlayerController targetFocus;
    [SerializeField] SpriteRenderer animalObj;
    [SerializeField] SpriteRenderer cupObj;
    [SerializeField] SpriteRenderer liquidObj;
    #endregion
    public TextMesh PlayerNameText;


    void Start() {
        willDamaged = true;
        isDied = false;
        canUseSkill = false;
        targetFocus = this;
        mainCamera = MainGameManager.instance.playerCamera;
        playerRig = GetComponent<Rigidbody2D>();
        target = Vector3.zero;
        //photonView = GetComponent<PhotonView>();
        hp = 100;
        if (MainGameManager.getInstance().isTutorial == true)
            canMove = false;
    }

    void Update()
    {
        rotateMySelf();
        if (isOwner())
        {
            Vector3 camPos = mainCamera.transform.position;
            if (targetFocus != null)
                camPos = Vector3.Lerp(mainCamera.transform.position, targetFocus.transform.position, 5f * Time.deltaTime);
            camPos.z = -10;
            mainCamera.transform.position = camPos;
        }
    }

    public bool isOwner()
    {
        return ID == PhotonNetwork.player.ID; 
    }

    private void FixedUpdate()
    {
        if(!PhotonNetwork.isMasterClient)
            mp = newInfo.mp;
        AITime += Time.deltaTime;
        lastVelocity = playerRig.velocity;
        if (PhotonNetwork.isMasterClient)
        {
            if (isDied)
                playerRig.velocity = Vector2.zero;
            checkInput();
            mp += playerRig.velocity.magnitude * 0.1f * Time.deltaTime;
            if (mp > 100)
                mp = 100;
            //addFriction();
        }
    }

    public void OnSceneLoaded()
    {
        gameObject.GetPhotonView().RPC("ImReady", PhotonTargets.MasterClient);
    }

    [PunRPC]
    public void ImReady()
    {
        MainGameManager.getInstance().PlayerLoadingFinishCount++;
        if (MainGameManager.getInstance().PlayerLoadingFinishCount >= PhotonManager.getInstance().playerList.Count)
        {
            CoroutineUtility.GetInstance()
                .Do()
                .Wait(1.5f)
                .Then(() => gameObject.GetPhotonView().RPC("StartGame", PhotonTargets.All))
                .Go();
            Debug.Log("EveryBodyFinishLoading");
        }
    }

    [PunRPC]
    public void StartGame()
    {
        MainGameManager.getInstance().StartGame();
    }

    void checkInput() {
        //float x = joystick.Horizontal;
        //float y = joystick.Vertical;

        //target.x += x;
        //target.y += y;

        //forceFoward = new Vector2(x, y);
        if (MainGameManager.getInstance().isGameOver)
            return;
        if (isAI)
        {
            if (AITime > 1)
            {
                float x = UnityEngine.Random.Range(-6, 7);
                float y = UnityEngine.Random.Range(-6, 7);
                forceFoward = new Vector2(x, y);
                if (isAIFocusEnemy && UnityEngine.Random.Range(0,2) >= 1)
                    forceFoward = AIFocusEnemy.transform.position - transform.position;
                CoroutineUtility.GetInstance().Do().Wait(0.6f).Then(() => forceFoward = Vector2.zero).Go();
                if (mp >= 100)
                    gameObject.GetPhotonView().RPC("UseSkill", PhotonTargets.All);
                AITime = 0;
            }
        }
        if (canMove)
        {
            playerRig.AddForce(forceFoward * (0.1f * (maxVelcocity + speedBuff)), ForceMode2D.Impulse);
            if (Vector2.Distance(Vector2.zero, playerRig.velocity) > maxVelcocity + speedBuff)
                playerRig.velocity = playerRig.velocity.normalized * (maxVelcocity + speedBuff);
        }


        //photonView.RPC("PlayerMove", PhotonTargets.MasterClient, forceFoward);
        //playerRig.AddForce(forceFoward * 1.5f, ForceMode2D.Impulse);
        //if (Vector2.Distance(Vector2.zero, playerRig.velocity) > maxVelcocity)
        //    playerRig.velocity = playerRig.velocity.normalized * maxVelcocity;

    }

    void addFriction()
    {
        playerRig.AddForce(-playerRig.velocity * 0.02f, ForceMode2D.Impulse);
    }

    void rotateMySelf() {
        rotateSpeed = lastVelocity.magnitude * 25;
        nowSpriteZ += rotateSpeed * Time.deltaTime;
        playerSprite.transform.localRotation = Quaternion.Euler(0, 0, -2.5f * nowSpriteZ);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAI)
            forceFoward = Vector2.zero;
        if (collision.gameObject.tag == "Player")
        {
            if (!canMove)
                return;
            if (PhotonNetwork.isMasterClient)
            {
                if (collision.contacts.Length <= 0)
                    return;
                ContactPoint2D cp = collision.contacts[0];
                Vector2 newVelocity = Vector2.Reflect(-cp.relativeVelocity, cp.normal) + (cp.normal * 10f);
                int damage = CalculateDamage(lastVelocity, collision.relativeVelocity, collision.gameObject.GetComponent<PlayerController>().lastVelocity);
                gameObject.GetPhotonView().RPC("CollisionEvent", PhotonTargets.All, newVelocity, cp.point, damage);
            }
        }
        else if (collision.gameObject.tag == "DamageItem")
        {            
            if (PhotonNetwork.isMasterClient)
            {
                if (collision.gameObject.GetComponent<PickedItem>().ownerID == ID)     //如果是自己的道具就不用做事
                    return;
                if (collision.contacts.Length <= 0)
                    return;
                ContactPoint2D cp = collision.contacts[0];
                Vector2 newVelocity = Vector2.Reflect(-cp.relativeVelocity, cp.normal) + (cp.normal * 10f);
                DamageItem item = collision.gameObject.GetComponent<DamageItem>();
                int damage = CalculateDamage(lastVelocity, collision.relativeVelocity, item.lastVelocity);
                gameObject.GetPhotonView().RPC("CollisionEvent", PhotonTargets.All, newVelocity, cp.point, damage);
                item.DestroyMe();
            }
        }
        else if (collision.gameObject.tag == "FishTail")
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (collision.gameObject.GetComponent<PickedItem>().ownerID == ID)     //如果是自己的道具就不用做事
                    return;
                if (collision.contacts.Length <= 0)
                    return;
                ContactPoint2D cp = collision.contacts[0];
                Vector2 newVelocity = Vector2.Reflect(-cp.relativeVelocity, cp.normal) + (cp.normal * 15f);
                FishTail item = collision.gameObject.GetComponent<FishTail>();
                int damage = CalculateDamage(lastVelocity, collision.relativeVelocity, item.lastVelocity);
                gameObject.GetPhotonView().RPC("CollisionEvent", PhotonTargets.All, newVelocity, cp.point, damage + 3);
            }
        }
        else
        {
            if (collision.contacts.Length <= 0)
                return;
            ContactPoint2D cp = collision.contacts[0];
            playerRig.velocity = Vector3.Reflect(lastVelocity, cp.normal);
            playerRig.velocity += cp.normal * 2.0f;
        }
    }


    public int CalculateDamage(Vector2 myVel, Vector2 collisionVelocity, Vector2 enemyVelocity)
    {
        float basicDamage = 0.39f;
        float damageValue = basicDamage * collisionVelocity.magnitude / 5f;
        float dot = Vector2.Dot(enemyVelocity.normalized, collisionVelocity.normalized);
        if (dot > 0)
        {
            return (int)(dot * damageValue);
        }
        else
        {
            return (int)basicDamage;
        }
    }

    public void Damaged(int damage) {
        if (!willDamaged)
            return;
        if (isDied)
            return;
        if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("UseSkill"))
            PlayIdleAnime();
        hp -= damage;
        if (hp <= 0)
        {
            if (PhotonNetwork.isMasterClient)
                gameObject.GetPhotonView().RPC("PlayerDied", PhotonTargets.All);
            
        }
        SpeedUp(1);
        if (isOwner())
        {
            //MainUIManager.instance.UpdateHealthBar(hp);
        }
    }

    [PunRPC]
    public void PlayerDied()
    {
        animalObj.gameObject.SetActive(false);
        cupObj.gameObject.SetActive(false);
        liquidObj.gameObject.SetActive(false);
        MinimapIconSprite.gameObject.SetActive(false);
        PlayerNameText.gameObject.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        canMove = false;
        hp = 0;
        isDied = true;
        canMove = false;
        DeadAnimation.SetActive(true);
        if (isOwner() || MainGameManager.getInstance().player.targetFocus.GetComponent<PlayerController>() == this)
        {
            CoroutineUtility.GetInstance()
                .Do()
                .Wait(1.5f)
                .Then(() => MainGameManager.getInstance().ChangeCameraFocus())
                .Then(() => MainGameManager.getInstance().changeCameraFocusPanel.SetActive(true))
                .Go();
        }

        if (isOwner())
        {
            MainGameManager.getInstance().canUseSkill = false;
        }

        if (PhotonNetwork.isMasterClient)
        {
            int rank = OnPlayerDied();
            gameObject.GetPhotonView().RPC("SetPlayerRank", PhotonTargets.All, rank);
            if (rank == 1)
            {
                Debug.Log("GameOver");
                gameObject.GetPhotonView().RPC("GameOver", PhotonTargets.All);
            }
            else if (rank == 3 || rank == 2)
            {
                PlayerController targetEnemy = null;
                PlayerController targetEnemy2 = null;
                int count = 0;
                foreach (PlayerController pl in PhotonManager.getInstance().playerList)
                {
                    if (!pl.isDied)
                    {
                        count++;
                        if (count <= 1)
                        {
                            targetEnemy = pl;
                        }
                        else
                        {
                            targetEnemy2 = pl;
                            break;
                        }
                    }
                }
                foreach (PlayerController pl in PhotonManager.getInstance().playerList)
                {
                    if (pl.isAI)
                    {
                        pl.isAIFocusEnemy = true;
                        if (pl == targetEnemy)
                            pl.AIFocusEnemy = targetEnemy2;
                        else
                            pl.AIFocusEnemy = targetEnemy;
                    }
                }
            }
        }
    }

    public int OnPlayerDied()
    {

        return ScoreManager.getInstance().OnPlayerDied(this);
    }

    [PunRPC]
    public void SetPlayerRank(int rank)
    {
        ScoreManager.getInstance().SetRank(rank, this);
    }

    [PunRPC]
    public void GameOver()
    {
        Debug.Log("GameOverrrrrrrrrrrr");
        MainGameManager.getInstance().isGameOver = true;
        List<PlayerController> list = PhotonManager.getInstance().playerList;
        CoroutineUtility.GetInstance()
                .Do()
                .Wait(1.2f)
                .Then(() => MainGameManager.getInstance().gameOverUI.SetActive(true))
                .Wait(3.5f)
                .Then(() => MainGameManager.getInstance().map.DisableBGM())
                .Then(() => ScoreManager.getInstance().scoreObj.SetActive(true))
                .Go();
        foreach (PlayerController p in list)
        {
            if (!p.isDied)
            {
                MainGameManager.getInstance().changeCameraFocusPanel.SetActive(false);
                CoroutineUtility.GetInstance()
                .Do()
                .Wait(1.5f)
                .Then(() => p.canMove = false)
                .Go();
                
                p.playerRig.velocity = Vector2.zero;
                ScoreManager.getInstance().SetRank(0, p);
            }
        }
        //MainGameManager.getInstance().GameOver();
    }

    public void AddSpeedBuff(int value)
    {
        speedBuff += value;
    }

    public void SpeedUp(int value)
    {
        if(maxVelcocity < maxMaxSpeed)
            maxVelcocity += value;
    }

    public void PlayCrashEffect(Vector2 contactPoint)
    {
        GameObject effectObj = Instantiate(crashEffect, contactPoint, Quaternion.identity);
        CoroutineUtility.GetInstance()
            .Do()
            .Wait(0.5f)
            .Then(() => Destroy(effectObj))
            .Go();
    }

    public void AddEnegy(int value)
    {
        mp += value;
        if (mp > 100)
            mp = 100;
        //UpdateSkillBarValue((int)mp);
    }

    public void PlayIdleAnime()
    {
        if (hp > 30)
        {
            playerAnimator.Play("Idle");
            if(isOwner())
                MainUIManager.instance.ChangeAnimalIcon(0);
        }
        else if (hp > 10)
        {
            playerAnimator.Play("HalfHealth");
            if (isOwner())
                MainUIManager.instance.ChangeAnimalIcon(1);
        }
        else
        {
            playerAnimator.Play("LowHealth");
            if (isOwner())
                MainUIManager.instance.ChangeAnimalIcon(2);
        }
    }

    #region RPC Functions
    [PunRPC]
    public void ReturnToRoom()
    {
        EntireGameManager.nextScene = "Room";
        PhotonNetwork.LoadLevel("Room");
    }

    [PunRPC]
    public void SetCharacterSprite(string animal, string cup, string liquid)
    {
        animalSprite = Resources.Load<Sprite>("Character/InGameView/animals/" + animal);
        cupSprite = Resources.Load<Sprite>("Character/InGameView/cup/" + cup);
        liquidSprite = Resources.Load<Sprite>("Character/InGameView/liquid/" + liquid);
        MinimapIconSprite.sprite = Resources.Load<Sprite>("Character/IconImage/" + animal);
        
        animalObj.sprite = animalSprite;
        cupObj.sprite = cupSprite;
        liquidObj.sprite = liquidSprite;
        this.animal = animal;
        this.cup = cup;
        this.liquid = liquid;
        playerAnimator.runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Animator/" + animal);
    }
    [PunRPC]
    public void UseSkill()
    {
        if (mp < 100)
            return;
        mp = 0;
        UpdateSkillBarValue(0);
        canUseSkill = false;
        if(skill.isPlayAnim)
            playerAnimator.Play("UseSkill");
        if(isOwner())
            AudioManager.Play("skill");
        CoroutineUtility.GetInstance()
            .Do()
            .Wait(skill.time)
            .Then(() => PlayIdleAnime())
            .Go();
        MainUIManager.instance.OnSkillUsed(animal);
        if (PhotonNetwork.isMasterClient)
        {
            gameObject.GetPhotonView().RPC("UpdateSkillBarValue", PhotonTargets.All, 0);
            for (int i = 0; i < skill.ObjToCreate.Count; i++)
            {
                GameObject itemObj = PhotonNetwork.InstantiateSceneObject(skill.ObjToCreate[i].name, transform.position, skill.ObjToCreate[i].transform.rotation, 0, null);
                if (skill.ObjToCreate[i].GetComponent<PickedItem>().isAttachPlayer)
                    itemObj.GetPhotonView().RPC("SetTransform", PhotonTargets.All, ID, skill.ObjToCreate[i].transform.position, skill.ObjToCreate[i].transform.localScale,skill.time);
                else
                {
                    itemObj.GetPhotonView().RPC("SetOwnerID", PhotonTargets.All, ID);
                    Vector3 pos = transform.position;
                    pos.z = -0.5f;

                    itemObj.transform.position = pos + new Vector3(0, 0, UnityEngine.Random.Range(-0.1f, 0.1f));
                    itemObj.transform.localScale = skill.ObjToCreate[i].transform.localScale;
                }
            }
        }
    }

    [PunRPC]
    public void CollisionEvent(Vector2 newVelocity, Vector2 collisionPoint, int damage)
    {
        playerRig.velocity = newVelocity;
        PlayCrashEffect(collisionPoint);
        AddEnegy(5);
        Damaged(damage);
        if (isOwner() || MainGameManager.getInstance().player.targetFocus.GetComponent<PlayerController>() == this)
            AudioManager.PlayCollisionSound();
        //if(isOwner())
            //AudioManager.Play("crash");
    }

    [PunRPC]
    public void AddHP(int value)
    {
        if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("UseSkill"))
            PlayIdleAnime();
        hp += value;
        if (hp > 100)
            hp = 100;
        if (isOwner())
        {
            MainUIManager.instance.DoHPBarShiningEffect();
        }
    }
    [PunRPC]
    public void AddMP(int value)
    {
        mp += value;
        if (mp > 100)
            mp = 100;
        if (isOwner())
            MainUIManager.instance.UpdateSkillBar((int)mp);
    }

    public void SetMP(int value)
    {
        mp = value;
    }

    [PunRPC]
    public void Initial(int index, int id, bool isAI, string nickName)
    {
        int wait = 1;
        if (SceneManager.GetActiveScene().name == "Tutorial")
            wait = 0;
        CoroutineUtility.GetInstance()
            .Do()
            .Wait(wait)
            .Then(() => SpawnAnimation.SetActive(true))
            .Wait(0.3f)
            .Then(() => playerSprite.SetActive(true))
            .Wait(1f)
            .Then(() => SpawnAnimation.SetActive(false))
            .Go();
        this.isAI = isAI;
        if (PhotonManager.getInstance().playerList == null)
            PhotonManager.getInstance().playerList = new List<PlayerController>();
        PhotonManager.getInstance().playerList.Add(this);
        ID = id;
        this.index = index;
        this.NickName = nickName;
        PlayerNameText.text = nickName;
        if (isOwner())
        {
            OnSceneLoaded();
            MainGameManager.getInstance().player = this;
            if (SceneManager.GetActiveScene().name == "Tutorial")
                TutorialManager.GetInstance().player = this;
            //MainUIManager.instance.UpdateHealthBar(hp);
            SaveData playerData = EntireGameManager.getInstance().playerData;
            SetCharacterSprite(playerData.nowEquipment.animal, playerData.nowEquipment.cup, playerData.nowEquipment.liquid);
            gameObject.GetPhotonView().RPC("SetCharacterSprite", PhotonTargets.All, playerData.nowEquipment.animal, playerData.nowEquipment.cup, playerData.nowEquipment.liquid);
            GameObject inputer = PhotonNetwork.Instantiate("FakeInputer", Vector3.zero, Quaternion.identity, 0);
            fakeInputer = inputer.GetComponent<FakeInputer>();
            fakeInputer.playerID = id;
            inputer.GetPhotonView().RPC("SetID", PhotonTargets.MasterClient, id);
            gameObject.GetPhotonView().RPC("SetSkill", PhotonTargets.All, playerData.nowEquipment.animal);
        }
        else if(isAI && PhotonNetwork.isMasterClient)
            OnSceneLoaded();
    }

    [PunRPC]
    public void SetSkill(string skillName)
    {
        skill = Resources.Load<Skill>("Skills/" + skillName);
    }

    [PunRPC]
    public void UpdateSkillBarValue(int value)
    {
        mp = value;
        if (isOwner())
        {
            MainUIManager.instance.UpdateSkillBar(value);
            if (mp == 100)
                canUseSkill = true;
        }
    }

    [PunRPC]
    public void CreateUIObject(string Obj)
    {
        if (isOwner())
        {
            GameObject UIObj = Resources.Load<GameObject>(Obj);
            Vector2 newUIPos = UIPosition.WorldToUI(MainUIManager.instance.canvas.GetComponent<RectTransform>(), transform.position);
            GameObject newObj = Instantiate(UIObj, UIObj.transform.position, UIObj.transform.rotation, MainUIManager.instance.canvas.transform);
            if (newObj.GetComponent<RadiusMoveItem>() != null)
                newObj.GetComponent<RadiusMoveItem>().SetStartPoint(newUIPos);
            RectTransform rect = newObj.GetComponent<RectTransform>();
            rect.anchoredPosition = newUIPos;
            rect.localScale = UIObj.transform.localScale;
        }
    }

    [PunRPC]
    public void CreateObject(string Obj)
    {
        if (isOwner())
        {
            GameObject UIObj = Resources.Load<GameObject>(Obj);
            Instantiate(UIObj, transform.position, Quaternion.identity);
        }
    }
    [PunRPC]
    public void PlayItemAudio()
    {
        if(isOwner())
            AudioManager.Play("EatItem");
    }

    #endregion



    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            transformInfo myTransform;
            myTransform.myPos = transform.position;
            myTransform.spriteRotation = playerSprite.transform.localRotation;
            myTransform.mp = mp;
            stream.SendNext(getBytes(myTransform));
        }
        else
        {
            newInfo = ByteArrayToNewStuff((byte[])stream.ReceiveNext());
        }
    }

    transformInfo ByteArrayToNewStuff(byte[] bytes)
    {
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        transformInfo stuff = (transformInfo)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(transformInfo));
        handle.Free();
        return stuff;
    }

    byte[] getBytes(transformInfo str)
    {
        int size = Marshal.SizeOf(str);
        byte[] arr = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(str, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }
}
struct transformInfo
{
    public float mp;
    public Vector3 myPos;
    public Quaternion spriteRotation;
}
