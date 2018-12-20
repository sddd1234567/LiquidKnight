using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shit : MonoBehaviour {
    public PickedItem pickedItem;
    float t;
    List<PlayerController> playerInShit;
    // Use this for initialization
    private void Awake()
    {
        if (playerInShit == null)
            playerInShit = new List<PlayerController>();
    }

    void Start () {
        t = 0;	
	}
	
	// Update is called once per frame
	void Update () {
        t += Time.deltaTime;
        if (t > 1)
        {
            for (int i = 0; i < playerInShit.Count; i++)
            {
                if (playerInShit[i] != null)
                    playerInShit[i].Damaged(2);
                else
                {
                    playerInShit.RemoveAt(i);
                    i--;
                }
            }
            t = 0;
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player.ID == pickedItem.ownerID)
                return;
            player.AddSpeedBuff(-6);
            if (playerInShit == null)
                playerInShit = new List<PlayerController>();
            playerInShit.Add(player);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player.ID == pickedItem.ownerID)
                return;
            player.AddSpeedBuff(6);
            playerInShit.Remove(player);
        }
            
    }
}
