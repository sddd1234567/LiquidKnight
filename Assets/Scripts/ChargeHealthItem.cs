using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChargeHealthItem : Item
{
    public int hpValue;
    public int mpValue;
    public float waitTime;
    public List<string> UIObjList;
    public override void ApplyEffect(PlayerController player)
    {
        base.ApplyEffect(player);
        for (int i = 0; i < UIObjList.Count; i++)
        {
            player.gameObject.GetPhotonView().RPC("CreateUIObject", PhotonTargets.AllBuffered, UIObjList[i]);
        }
        if (SceneManager.GetActiveScene().name != "Tutorial")
            ItemFactory.getInstance().nowItemCount--;
        else if (player.isAI)
            return;
        CoroutineUtility.GetInstance()
            .Do()
            .Wait(waitTime)
            .Then(() => player.gameObject.GetPhotonView().RPC("AddHP", PhotonTargets.AllBuffered, hpValue))
            .Then(() => player.gameObject.GetPhotonView().RPC("AddMP", PhotonTargets.AllBuffered, mpValue))
            .Go();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PhotonNetwork.isMasterClient && collision.tag == "Player")
        {
            ApplyEffect(collision.GetComponent<PlayerController>());
            if (SceneManager.GetActiveScene().name == "Tutorial")
                Destroy(gameObject);
            else
                PhotonNetwork.Destroy(gameObject);
        }
    }
}
