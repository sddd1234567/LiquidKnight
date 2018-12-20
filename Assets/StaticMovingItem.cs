using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class StaticMovingItem : MonoBehaviour {
    public Vector2 direction;
    public float speed;
    Rigidbody2D rig;
    // Use this for initialization
    void Start () {
        rig = GetComponent<Rigidbody2D>();
	}

    private void FixedUpdate()
    {
        rig.velocity = direction.normalized * speed;
    }
}
