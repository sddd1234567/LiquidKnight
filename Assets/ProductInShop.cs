using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductInShop : MonoBehaviour {
    public Image icon;
    public Text nameText;
    public Image cost;
    public Text costText;
    public Image Lock;
    public Image Cover;
    public Sprite pearlSprite;
    public Sprite sugarSprit;

    public void HaveProduct(bool have)
    {
        if (have)
        {
            cost.gameObject.SetActive(false);
            costText.gameObject.SetActive(false);
            Lock.gameObject.SetActive(false);
            Cover.gameObject.SetActive(false);
        }
        else
        {
            cost.gameObject.SetActive(true);
            costText.gameObject.SetActive(true);
            Lock.gameObject.SetActive(true);
            Cover.gameObject.SetActive(true);
        }
    }

    public void SetCostType(CostType type)
    {
        if (type == CostType.Pearl)
            cost.sprite = pearlSprite;
        else
            cost.sprite = sugarSprit;
    }
}
