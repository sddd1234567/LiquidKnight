using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatBall : MonoBehaviour {
    public List<GameObject> catBalls;
    public List<Vector2> dir;
    int ownerID;
    public float waitTime;
	// Use this for initialization
	void Start () {
        
	}
    public void StartCreate(int owner)
    {
        if (PhotonNetwork.isMasterClient)
        {
            ownerID = owner;
            CoroutineUtility.GetInstance()
                .Do()
                .Then(() => CreateCatBall(dir[0], 0))
                .Then(() => CreateCatBall(dir[1], 1))
                .Then(() => CreateCatBall(dir[2], 2))
                .Wait(waitTime)
                .Then(() => CreateCatBall(dir[3], 0))
                .Then(() => CreateCatBall(dir[4], 1))
                .Then(() => CreateCatBall(dir[5], 2))
                .Wait(waitTime)
                .Then(() => CreateCatBall(dir[6], 0))
                .Then(() => CreateCatBall(dir[7], 1))
                .Then(() => CreateCatBall(dir[8], 2))
                .Wait(waitTime)
                .Then(() => CreateCatBall(dir[9], 0))
                .Then(() => CreateCatBall(dir[10], 1))
                .Then(() => CreateCatBall(dir[11], 2))
                .Then(() => Destroy(gameObject))
                .Go();
        }
        else
            Destroy(gameObject);
    }

    public void CreateCatBall(Vector2 forward, int i)
    {
        GameObject itemObj = PhotonNetwork.InstantiateSceneObject(catBalls[i].name, transform.position, catBalls[0].transform.rotation, 0, null);
        itemObj.GetPhotonView().RPC("SetOwnerID", PhotonTargets.All, ownerID);
        Vector3 pos = transform.position;
        pos.z = -0.5f;

        itemObj.transform.position = pos;
        itemObj.transform.localScale = catBalls[i].transform.localScale;
        itemObj.GetComponent<StaticMovingItem>().direction = forward;
    }
	// Update is called once per frame
	void Update () {
		
	}
}
