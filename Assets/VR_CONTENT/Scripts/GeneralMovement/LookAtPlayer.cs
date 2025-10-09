using UnityEngine;
using System.Collections;

public class LookAtPlayer : MonoBehaviour {

	// Use this for initialization
	GameObject player;
	
	
	void Start () {
		player=GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
	
		transform.forward=-( player.transform.position-transform.position);
	
	}
}
