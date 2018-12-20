using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialObject : MonoBehaviour {
    [SerializeField]
    public UnityEvent TriggerTurtorialEvent;
    public TriggerType triggerType;
    // Use this for initialization


    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggerType == TriggerType.OnTriggerEnter)
        {
            Do();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggerType == TriggerType.OnCollisionEnter)
        {
            Do();
        }
    }

    public void Do()
    {
        TriggerTurtorialEvent.Invoke();
    }

    public void MoveNext()
    {
        TutorialManager.GetInstance().MoveNext();
    }

    public void StopPlayer()
    {
        TutorialManager.GetInstance().StopPlayer();
    }

    public void CanPlayerMove(bool canMove)
    {
        TutorialManager.GetInstance().SetPlayerCanMove(canMove);
    }

    public void ShowHealthBarUI()
    {
        TutorialManager.GetInstance().ShowHealthBarUI();
    }

    public void HideHealthBarUI()
    {
        TutorialManager.GetInstance().HideHealthBarUI();
    }
}

public enum TriggerType {
    OnTriggerEnter, OnCollisionEnter
}