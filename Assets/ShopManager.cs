using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {
    public MerchandiseList merchandiseList;
    public List<Merchandise> animals;
    public List<Merchandise> cups;
    public List<Merchandise> liquids;
    public RectTransform animalShop;
    public RectTransform cupShop;
    public RectTransform liquidShop;
    public float itemWidth;
    public int nowAnimalIndex;
    public int nowCupIndex;
    public int nowLiquidIndex;
    public GameObject AnimalProductPrefab;
    public GameObject CupProductPrefab;
    public GameObject LiquidProductPrefab;
    bool isSlidingAnimal;
    bool isSlidingCup;
    bool isSlidingLiquid;
    public Text SkillDescript;
    public Text SkillName;
    public Image playerAnimalImage;
    public Image playerCupImage;
    public Image playerLiquidImage;

    public Text SugarCount;
    public Text PearlCount;
    public static bool isOpenIAPPanel;
    public static ShopManager instance;

    public GameObject IAPPanel;

    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start () {
        animals = merchandiseList.animals;
        cups = merchandiseList.cups;
        liquids = merchandiseList.liquids;
        UpdateMoney();
        InitialProducts();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SwitchAnimalShop(int slideType)
    {
        int targetIndex;
        if (isSlidingAnimal)
            return;
        if (slideType == 1)
        {
            if (nowAnimalIndex + 1 >= animals.Count)
            {
                targetIndex = nowAnimalIndex + 1;
                nowAnimalIndex = 0;
            }
            else
            {
                nowAnimalIndex++;
                targetIndex = nowAnimalIndex;
            }
        }
        else
        {
            if (nowAnimalIndex <= 0)
            {
                targetIndex = nowAnimalIndex - 1;
                nowAnimalIndex = animals.Count - 1;
            }
            else
            {
                nowAnimalIndex--;
                targetIndex = nowAnimalIndex;
            }
        }
        isSlidingAnimal = true;
        CoroutineUtility.GetInstance()
            .Do()
            .DoEnumerator(Slide(animalShop.gameObject, targetIndex))
            .Then(() => MoveShopPos(animalShop.gameObject, nowAnimalIndex))
            .Then(() => isSlidingAnimal = false)
            .Go();
        ChangeImageSprite(playerAnimalImage, animals[nowAnimalIndex].SelectViewSprite);
        ChangeSkillDescript(animals[nowAnimalIndex].skillName, animals[nowAnimalIndex].skillDescript);
    }

    public void ChangeSkillDescript(string skillName, string descript)
    {
        SkillName.text = skillName;
        SkillDescript.text = descript;
    }

    public void LeaveShop()
    {
        if (EntireGameManager.getInstance().getPlayerData().HaveItem(MerchandiseType.Animal, animals[nowAnimalIndex].indexName))
        {
            EntireGameManager.getInstance().playerData.nowEquipment.animal = animals[nowAnimalIndex].indexName;
        }
        if (EntireGameManager.getInstance().getPlayerData().HaveItem(MerchandiseType.Cup, cups[nowCupIndex].indexName))
        {
            EntireGameManager.getInstance().playerData.nowEquipment.cup = cups[nowCupIndex].indexName;
        }
        if (EntireGameManager.getInstance().getPlayerData().HaveItem(MerchandiseType.Liquid, liquids[nowLiquidIndex].indexName))
        {
            EntireGameManager.getInstance().playerData.nowEquipment.liquid = liquids[nowLiquidIndex].indexName;
        }
        EntireGameManager.getInstance().Save();
        StartCoroutine(EntireGameManager.LoadSceneWithLoadingScene("RoomList"));
    }

    public void SwitchCupShop(int slideType)
    {
        int targetIndex;
        if (isSlidingCup)
            return;
        if (slideType == 1)
        {
            if (nowCupIndex + 1 >= cups.Count)
            {
                targetIndex = nowCupIndex + 1;
                nowCupIndex = 0;
            }
            else
            {
                nowCupIndex++;
                targetIndex = nowCupIndex;
            }
        }
        else
        {
            if (nowCupIndex <= 0)
            {
                targetIndex = nowCupIndex - 1;
                nowCupIndex = cups.Count - 1;
            }
            else
            {
                nowCupIndex--;
                targetIndex = nowCupIndex;
            }
        }
        isSlidingCup = true;
        CoroutineUtility.GetInstance()
            .Do()
            .DoEnumerator(Slide(cupShop.gameObject, targetIndex))
            .Then(() => MoveShopPos(cupShop.gameObject, nowCupIndex))
            .Then(() => isSlidingCup = false)
            .Go();
        ChangeImageSprite(playerCupImage, cups[nowCupIndex].SelectViewSprite);
    }

    public void SwitchLiquidShop(int slideType)
    {
        int targetIndex;
        if (isSlidingLiquid)
            return;
        if (slideType == 1)
        {
            if (nowLiquidIndex + 1 >= liquids.Count)
            {
                targetIndex = nowLiquidIndex + 1;
                nowLiquidIndex = 0;
            }
            else
            {
                nowLiquidIndex++;
                targetIndex = nowLiquidIndex;
            }
        }
        else
        {
            if (nowLiquidIndex <= 0)
            {
                targetIndex = nowLiquidIndex - 1;
                nowLiquidIndex = liquids.Count - 1;
            }
            else
            {
                nowLiquidIndex--;
                targetIndex = nowLiquidIndex;
            }
        }
        isSlidingLiquid = true;
        CoroutineUtility.GetInstance()
            .Do()
            .DoEnumerator(Slide(liquidShop.gameObject, targetIndex))
            .Then(() => MoveShopPos(liquidShop.gameObject, nowLiquidIndex))
            .Then(() => isSlidingLiquid = false)
            .Go();
        ChangeImageSprite(playerLiquidImage, liquids[nowLiquidIndex].SelectViewSprite);
    }
    
    public void ChangeImageSprite(Image image, Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void OnBuyClicked(Merchandise merchandise, ProductInShop productInShop)
    {
        if (merchandise.type == MerchandiseType.Animal && isSlidingAnimal)
            return;
        else if (merchandise.type == MerchandiseType.Cup && isSlidingCup)
            return;
        else if (merchandise.type == MerchandiseType.Liquid && isSlidingLiquid)
            return;
        string costType = "方糖";
        if (merchandise.costType == CostType.Pearl)
            costType = "珍珠";
        MainUIManager.CreateDialogBox(() => OnSuccessBuy(merchandise, productInShop), CancelBuy, "確定要花費"+merchandise.cost+costType+"購買" + merchandise.chineseName + "嗎？");
    }

    public void OnSuccessBuy(Merchandise merchandise, ProductInShop productInShop)
    {
        if (merchandise.costType == CostType.Pearl)
        {
            if (EntireGameManager.getInstance().getPlayerData().pearl < merchandise.cost)
            {
                Debug.Log("珍珠不足");
                MainUIManager.CreateCheckBox(() => { }, "珍珠不足");
                return;
            }
            else
                EntireGameManager.getInstance().playerData.pearl -= merchandise.cost;
        }
        else
        {
            if (EntireGameManager.getInstance().getPlayerData().sugar < merchandise.cost)
            {
                MainUIManager.CreateCheckBox(() => { }, "方糖不足");
                Debug.Log("方糖不足");
                return;
            }
            else
                EntireGameManager.getInstance().playerData.sugar -= merchandise.cost;
        }
        Debug.Log("成功購買" + merchandise.chineseName);
        if (merchandise.type == MerchandiseType.Animal)
            EntireGameManager.getInstance().getPlayerData().animalList.Add(merchandise.indexName);
        else if (merchandise.type == MerchandiseType.Cup)
            EntireGameManager.getInstance().getPlayerData().cupList.Add(merchandise.indexName);
        else if (merchandise.type == MerchandiseType.Liquid)
            EntireGameManager.getInstance().getPlayerData().liquidList.Add(merchandise.indexName);
        productInShop.HaveProduct(true);
        EntireGameManager.getInstance().Save();
        AudioManager.Play("purchase");
        UpdateMoney();
    }

    public void CancelBuy()
    {
        Debug.Log("取消");
    }

    void MoveShopPos(GameObject obj, int targetIndex)
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

        while (relativeX * t >　2)
        {
            float x = Mathf.Lerp(rect.anchoredPosition.x, targetX, Time.deltaTime * 10);
            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
            relativeX = targetX - rect.anchoredPosition.x;
            yield return null;
        }
        rect.anchoredPosition = new Vector2(targetX, rect.anchoredPosition.y);        
    }

    public void InitialProducts()
    {
        for (int i = 0; i < animals.Count; i++)
        {
            AddProduct(animals[i], animals[i].type, i);
            if (animals[i].indexName == EntireGameManager.getInstance().getPlayerData().nowEquipment.animal)
            {
                MoveShopPos(animalShop.gameObject, i);
                nowAnimalIndex = i;
                ChangeSkillDescript(animals[i].skillName, animals[i].skillDescript);
            }
        }
        AddProduct(animals[0], animals[0].type, animals.Count);
        AddProduct(animals[animals.Count - 1], animals[animals.Count - 1].type, -1);

        for (int i = 0; i < cups.Count; i++)
        {
            AddProduct(cups[i], cups[i].type, i);
            if (cups[i].indexName == EntireGameManager.getInstance().getPlayerData().nowEquipment.cup)
            {
                MoveShopPos(cupShop.gameObject, i);
                nowCupIndex = i;
            }
        }
        AddProduct(cups[0], cups[0].type, cups.Count);
        AddProduct(cups[animals.Count - 1], cups[cups.Count - 1].type, -1);
        for (int i = 0; i < liquids.Count; i++)
        {
            AddProduct(liquids[i], liquids[i].type, i);
            if (liquids[i].indexName == EntireGameManager.getInstance().getPlayerData().nowEquipment.liquid)
            {
                MoveShopPos(liquidShop.gameObject, i);
                nowLiquidIndex = i;
            }
        }
        AddProduct(liquids[0], liquids[0].type, liquids.Count);
        AddProduct(liquids[animals.Count - 1], liquids[cups.Count - 1].type, -1);

        ChangeImageSprite(playerAnimalImage, animals[nowAnimalIndex].SelectViewSprite);
        ChangeImageSprite(playerCupImage, cups[nowCupIndex].SelectViewSprite);
        ChangeImageSprite(playerLiquidImage, liquids[nowLiquidIndex].SelectViewSprite);
    }

    void AddProduct(Merchandise product, MerchandiseType type, int index)
    {
        if (type == MerchandiseType.Animal)
        {
            RectTransform rect = Instantiate(AnimalProductPrefab, animalShop).GetComponent<RectTransform>();
            float x = index * itemWidth;
            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
            ProductInShop prod = rect.GetComponent<ProductInShop>();
            prod.icon.sprite = product.SelectViewSprite;
            prod.nameText.text = product.chineseName;
            prod.costText.text = "x"+product.cost;
            prod.SetCostType(product.costType);
            if (EntireGameManager.getInstance().getPlayerData().HaveItem(type, product.indexName))
                prod.HaveProduct(true);
            else
                prod.HaveProduct(false);
            prod.Cover.gameObject.GetComponent<Button>().onClick.AddListener(() => OnBuyClicked(product, prod));
        }
        else if (type == MerchandiseType.Cup)
        {
            RectTransform rect = Instantiate(CupProductPrefab, cupShop).GetComponent<RectTransform>();
            float x = index * itemWidth;
            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
            ProductInShop prod = rect.GetComponent<ProductInShop>();
            prod.icon.sprite = product.SelectViewSprite;
            prod.nameText.text = product.chineseName;
            prod.costText.text = "x" + product.cost;
            prod.SetCostType(product.costType);
            if (EntireGameManager.getInstance().getPlayerData().HaveItem(type, product.indexName))
                prod.HaveProduct(true);
            else
                prod.HaveProduct(false);
            prod.Cover.gameObject.GetComponent<Button>().onClick.AddListener(() => OnBuyClicked(product, prod));
        }
        else if (type == MerchandiseType.Liquid)
        {
            RectTransform rect = Instantiate(LiquidProductPrefab, liquidShop).GetComponent<RectTransform>();
            float x = index * itemWidth;
            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
            ProductInShop prod = rect.GetComponent<ProductInShop>();
            prod.nameText.text = product.chineseName;
            prod.costText.text = "x" + product.cost;
            prod.SetCostType(product.costType);
            if (EntireGameManager.getInstance().getPlayerData().HaveItem(type, product.indexName))
                prod.HaveProduct(true);
            else
                prod.HaveProduct(false);
            prod.Cover.gameObject.GetComponent<Button>().onClick.AddListener(() => OnBuyClicked(product, prod));
        }
    }

    public bool CheckHaveEquipment(Merchandise product)
    {
        return true;
    }

    public void UpdateMoney()
    {
        int sugar = EntireGameManager.getInstance().getPlayerData().sugar;
        int pearl = EntireGameManager.getInstance().getPlayerData().pearl;
        SugarCount.text = sugar.ToString();
        PearlCount.text = pearl.ToString();
    }

    public void OpenIAPPanel()
    {
        if (isOpenIAPPanel)
            return;
        isOpenIAPPanel = true;

        Instantiate(IAPPanel, GameObject.Find("Canvas").transform);
    }
}
