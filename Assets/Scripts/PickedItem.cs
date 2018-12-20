using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedItem : Photon.PunBehaviour
{
    public int ownerID;
    public bool isAttachPlayer;
    float liveTime = 0;
    float maxLiveTime = 10;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        liveTime += Time.deltaTime;
        if (liveTime > maxLiveTime)
            Destroy(gameObject);
	}

    [PunRPC]
    public void SetTransform(int playerID, Vector3 pos, Vector3 scale, float maxLiveTime)
    {
        SetOwnerID(playerID);
        if (GetComponent<CatBall>() != null)
            GetComponent<CatBall>().StartCreate(playerID);
        transform.SetParent(PhotonManager.getInstance().FindPlayerByID(playerID).playerSprite.transform);
        transform.localPosition = pos + new Vector3(0, 0, Random.Range(-0.1f, 0.1f));
        transform.localScale = scale;
        this.maxLiveTime = maxLiveTime;
    }

    [PunRPC]
    public void SetOwnerID(int id)
    {
        ownerID = id;
        if (GetComponent<Collider2D>() != null)
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), PhotonManager.getInstance().FindPlayerByID(ownerID).GetComponent<Collider2D>());
    }

    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }
}
