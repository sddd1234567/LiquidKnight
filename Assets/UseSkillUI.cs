using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseSkillUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        CoroutineUtility.GetInstance().Do().Wait(2).Then(() => Destroy(gameObject)).Go();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
