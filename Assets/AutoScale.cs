using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScale : MonoBehaviour {
	// Use this for initialization
	void Start ()
    {
        if (Screen.width / Screen.height > (float)16 / 9)
        {
            GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
