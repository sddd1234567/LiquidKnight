using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageItem : MonoBehaviour {
    public Vector2 lastVelocity;
    Rigidbody2D rig;
    public int ownerID;
    public PlayerController owner;
    bool isDestroyed = false;
    // Use this for initialization

    void Start () {
        rig = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void SetOwner(PlayerController player)
    {
        owner = player;
        if (GetComponent<StaticMovingItem>() != null)
            GetComponent<StaticMovingItem>().speed = player.maxVelcocity;
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        GetComponent<Collider2D>().enabled = true;
    }

    private void FixedUpdate()
    {
        lastVelocity = rig.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (collision.gameObject.tag == "Wall")
            {
                DestroyMe();
            }
        }
    }

    public void DestroyMe()
    {
        if (!isDestroyed)
        {
            isDestroyed = true;
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
