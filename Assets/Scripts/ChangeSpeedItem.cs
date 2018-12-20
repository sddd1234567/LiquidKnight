using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpeedItem : Item {
    public int SpeedUpValue;
    public int time;
    List<GameObject> itemList;
    public override void ApplyEffect(PlayerController player)
    {
        itemList = new List<GameObject>();
        base.ApplyEffect(player);
        for (int i = 0; i < ObjToCreate.Count; i++)
        {
            GameObject itemObj = PhotonNetwork.InstantiateSceneObject(ObjToCreate[i].name, player.transform.position, ObjToCreate[i].transform.rotation, 0, null);
            itemList.Add(itemObj);
            itemObj.GetPhotonView().RPC("SetTransform", PhotonTargets.All, player.ID, new Vector3(0, 0, -2), ObjToCreate[i].transform.localScale, (float)time);
            itemObj.transform.localPosition = new Vector3(0, 0, -2);
            itemObj.transform.localScale = ObjToCreate[i].transform.localScale;
        }
        
        ItemFactory.getInstance().nowItemCount--;
        CoroutineUtility.GetInstance()
            .Do()
            .Then(() => player.AddSpeedBuff(SpeedUpValue))
            .Wait(time)
            .Then(() => player.AddSpeedBuff(-SpeedUpValue))
            .Go();
    }

    //void DestroyItems()
    //{
    //    for (int i = 0; i < itemList.Count; i++)
    //    {
    //        PhotonNetwork.Destroy(itemList[i]);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PhotonNetwork.isMasterClient && collision.tag == "Player")
        {
            ApplyEffect(collision.GetComponent<PlayerController>());
            collision.GetComponent<PlayerController>().gameObject.GetPhotonView().RPC("PlayItemAudio", PhotonTargets.All);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
