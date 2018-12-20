using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
    public List<Vector2> spawnPoint;
    public Vector2 LeftDownBoundary;
    public Vector2 RightTopBoundary;
    public int speedBuff;
    public AudioSource bgm;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ApplyMapBuff()
    {
        foreach (PlayerController pl in PhotonManager.getInstance().playerList)
        {
            pl.maxVelcocity += speedBuff;
            pl.maxMaxSpeed += speedBuff;
        }
    }

    public void DisableBGM()
    {
        bgm.volume = 0;
    }

    public void EnalbeBGM()
    {
        bgm.volume = 0.41f;
    }
}
