using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour {
    private AudioSource ad;
    bool isStartPlaying = false;
    // Use this for initialization

    void Start() {
        ad = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        if (ad == null)
            ad = GetComponent<AudioSource>();
        ad.clip = clip;
        ad.Play();
        isStartPlaying = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (!ad.isPlaying && isStartPlaying)
            Destroy(gameObject);
	}
}
