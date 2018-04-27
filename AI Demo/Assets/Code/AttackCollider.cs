using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour {
	
	public bool hitCheck = false;//has this collider hit an enemy yet

	void OnEnable()
	{
		hitCheck = false;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag ("Enemy") && !hitCheck) 
		{
			hitCheck = true;
			col.SendMessageUpwards ("OnHit");
			if (gameObject.tag == "Knife")
				Destroy (gameObject);
		}
		if (col.CompareTag ("Player") && !hitCheck) 
		{
			hitCheck = true;
			col.SendMessageUpwards ("OnHit");
			if (gameObject.tag == "Knife")
				Destroy (gameObject);
		}
	}
}
