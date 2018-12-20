using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static Dictionary<string, AudioClip> audios;
    public GameObject AudioHandler;
    public AudioList list;
    private void Awake()
    {
        if(audios == null)
            InitialDictionary();
        DontDestroyOnLoad(gameObject);
    }

    public void InitialDictionary()
    {
        list = Resources.Load<AudioList>("AudioList");
        audios = new Dictionary<string, AudioClip>();
        for (int i = 0; i < list.audioName.Count; i++)
        {
            audios.Add(list.audioName[i], list.clip[i]);
        }
    }

    public void PlayAudio(string audioName)
    {
        if (audios == null)
            InitialDictionary();
        SoundEffect se = Instantiate(AudioHandler).GetComponent<SoundEffect>();
        se.Play(audios[audioName]);
    }
    public void PlayAudio(string audioName, float afterTime)
    {
        if (audios == null)
            InitialDictionary();
        SoundEffect se = Instantiate(AudioHandler).GetComponent<SoundEffect>();
        CoroutineUtility.GetInstance()
            .Do()
            .Wait(afterTime)
            .Then(() => se.Play(audios[audioName]))
            .Go();
    }

    public static void Play(string audioName)
    {
        AudioManager am = Resources.Load<AudioManager>("AudioManager");
        am.PlayAudio(audioName);
    }

    public static void PlayCollisionSound()
    {
        string[] collisionSounds = new string[]{ "hit1", "hit2", "hit2" };
        Play(collisionSounds[Random.Range(0, 3)]);
    }
}