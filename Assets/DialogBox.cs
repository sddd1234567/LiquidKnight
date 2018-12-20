using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour {
    public Button yesButton;
    public Button noButton;
    public Text descriptText;

    private void Start()
    {
        CoroutineUtility.GetInstance()
            .Do()
            .UIMove(gameObject, Vector2.zero, 0.2f)
            .Go();
    }

    private void Update()
    {
        
    }
}
