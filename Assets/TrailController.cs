using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailController : MonoBehaviour {
    public RectTransform targetUI;
    bool isAssigned = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isAssigned)
            transform.position = UIPosition.UIToWorld(MainUIManager.instance.canvas.GetComponent<RectTransform>(), targetUI.anchoredPosition);
	}

    public void Assign(RectTransform rect) {
        targetUI = rect;
        isAssigned = true;
    }
}
