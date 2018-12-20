using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTail : MonoBehaviour {
    public Vector2 lastVelocity;
    Rigidbody2D rig;
    // Use this for initialization
    void Start () {
        rig = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        lastVelocity = rig.velocity;
    }
}
