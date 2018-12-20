using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObjectItem : Item {
    public override void ApplyEffect(PlayerController player)
    {
        base.ApplyEffect(player);
        for (int i = 0; i < ObjToCreate.Count; i++)
        {
            GameObject itemObj = PhotonNetwork.InstantiateSceneObject(ObjToCreate[i].name, player.transform.position, ObjToCreate[i].transform.rotation, 0, null);
            itemObj.GetComponent<PickedItem>().ownerID = player.ID;
            if (itemObj.GetComponent<DamageItem>() != null)
            {
                itemObj.GetComponent<DamageItem>().SetOwner(player);
            }
            Vector3 pos = player.transform.position;
            pos.z = -0.5f;
            itemObj.transform.position = pos;
            itemObj.transform.localScale = ObjToCreate[i].transform.localScale;
            ItemFactory.getInstance().nowItemCount--;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PhotonNetwork.isMasterClient && collision.tag == "Player")
        {
            ApplyEffect(collision.GetComponent<PlayerController>());
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
