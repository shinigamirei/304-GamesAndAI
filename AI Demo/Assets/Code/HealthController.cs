using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour {

	public Transform target;
	public int userHealth;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (target == null) 
		{
			
		}
		else if (target.CompareTag ("Enemy")) 
		{
			Vector3 targetPos = Camera.main.WorldToScreenPoint (target.position);
			Vector3 wantedPos = targetPos + (Vector3.up * 50);
			transform.position = wantedPos;
		}
	}
}
