using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchandise : ScriptableObject {
    public string chineseName;
    public string indexName;
    public string descript;
    public string skillName;
    public string skillDescript;
    public int cost;
    public CostType costType;
    public MerchandiseType type;
    public Sprite SelectViewSprite;
}

public enum MerchandiseType {
    Animal, Cup, Liquid
}

public enum CostType {
    Sugar, Pearl
}
