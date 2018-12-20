using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour {
    public List<GameObject> itemList;
    float createTime = 0;
    public int nowItemCount = 0;
    public static ItemFactory instance;
    public float frequency;

    public static ItemFactory getInstance()
    {
        if (instance == null)
        {
            instance = new GameObject("ItemFactory").AddComponent<ItemFactory>();
            return instance;
        }
        else
            return instance;
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (PhotonNetwork.isMasterClient && !MainGameManager.getInstance().isGameOver && MainGameManager.getInstance().isGameStart)
        {
            createTime += Time.deltaTime;
            if (createTime > frequency)
            {
                if (nowItemCount < 5)
                {
                    GameObject item = RandomItem();
                    PhotonNetwork.InstantiateSceneObject(item.name, RandomPosition(), Quaternion.identity, 0, null);
                    nowItemCount++;
                }
                createTime = 0;
            }
        }
	}

    Vector3 RandomPosition()
    {
        Vector2 min = MainGameManager.getInstance().map.LeftDownBoundary;
        Vector2 max = MainGameManager.getInstance().map.RightTopBoundary;

        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);
        return new Vector3(x, y, -0.5f);
    }

    GameObject RandomItem()
    {
        int r = Random.Range(0, itemList.Count);
        return itemList[r];
    }
}
