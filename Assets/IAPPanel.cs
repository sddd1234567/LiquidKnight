using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IAPPanel : MonoBehaviour {
    public Text nowPearlCount;
    public static IAPPanel instance;
    // Use this for initialization
    private void Awake()
    {
        instance = this;
        UpdatePearlCount();
    }

    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Buy5Pearls()
    {
        IAPManager.Instance.Buy5Pearls();
    }

    public void Buy20Pearls()
    {
        IAPManager.Instance.Buy20Pearls();
    }

    public void Buy50Pearls()
    {
        IAPManager.Instance.Buy50Pearls();
    }

    public void CloseIAPPanel()
    {
        ShopManager.isOpenIAPPanel = false;
        Destroy(gameObject);
    }

    public void UpdatePearlCount()
    {
        nowPearlCount.text = EntireGameManager.getInstance().playerData.pearl.ToString();
    }
}
