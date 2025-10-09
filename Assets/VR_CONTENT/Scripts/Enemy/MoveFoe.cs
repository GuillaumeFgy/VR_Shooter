using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFoe : MonoBehaviour {

    public Transform head;

    public Transform foeHead;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        foeHead.rotation = head.rotation;

        transform.rotation=Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,head.rotation.eulerAngles[1]+180,0),0.05f);

    }
}
