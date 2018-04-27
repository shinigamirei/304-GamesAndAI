using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyControler : MonoBehaviour {

	Rigidbody2D rb;
	Animator anim;
	public GameObject target;
	public GameObject healthBar;
	public Vector3 startLoc;
	public Transform groundCheck;
	public GameObject attackCheck;
	public GameObject gameControler;
	public GameObject knife;
	public Transform knifeSpawnLoc;

	bool facingRight = true;
	bool targetRight = false;
	public bool jump = false;
	bool grounded = false;
	public bool attacking = false;
	public bool blocking = false;
	public bool dodge = false;
	public bool hitStun = false;
	bool knifeReady = true;

	//ai specific 
	bool approaching = false;
	bool running = false;
	enum aiStateOptions{Agressive,Passive,Defensive}
	float aiPollRate = 0.5f;
	float aiState;
	public Text stateDisplay;
	public Text distanceDisplay;

	public float attackTiming = 0f;
	public float attackCd = 0.3f;
	public float moveForce = 365f;
	public float maxspeed = 10f;
	public float distance;
	public float jumpForce = 1000f;
	public float fallMult = 1.03f;
	float iFrames = 0.5f;
	float attackDelay = 0.2f;

	float startHealth = 200;
	public float health = 200;

	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		startLoc = this.gameObject.transform.position;
		attackCheck.SetActive (false);
	}

	// Update is called once per frame
	void Update () 
	{
		if (aiState == (int)aiStateOptions.Agressive)
			stateDisplay.text = ("AI State: Agressive");
		else if (aiState == (int)aiStateOptions.Defensive)
			stateDisplay.text = ("AI State: Defensive");
		else
			stateDisplay.text = ("AI State: Passive");

		grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));

		if (target != null)
			distance = gameObject.transform.position.x - target.transform.position.x;

		distanceDisplay.text = ("Distance:" + distance);

		if (attacking)
		{
			if (attackTiming > 0)
				attackTiming -= Time.deltaTime;
			else 
			{
				attacking = false;
				attackCheck.SetActive(false);
			}
		}
	}

	void FixedUpdate()
	{
		if (approaching == true && !hitStun) // when ai decides it wants to move towards opponent, this part of the code will make it do so
		{
			if (distance > 1 || distance < -1) 
			{
				if (Mathf.Sign (-distance) * rb.velocity.x < maxspeed)
					rb.AddForce (Vector2.right * Mathf.Sign (-distance) * moveForce);//if h is negative will move left		
			}
			if (distance < 1.5 && distance > -1.5)
				rb.velocity = Vector3.zero;
		}
		if (running == true && !hitStun) // when ai decides it wants to move towards opponent, this part of the code will make it do so
		{
			if (distance < 7 && distance > -7) 
			{
				if (Mathf.Sign (-distance) * rb.velocity.x < maxspeed)
					rb.AddForce (Vector2.right * Mathf.Sign (distance) * moveForce);//if h is negative will move left		
			}
			if (distance > 8 || distance < -8)
				rb.velocity = Vector3.zero;
		}

		//all copied from player, working to adapt		
		if (rb.velocity.y < 0)
			rb.velocity += Vector2.up * Physics2D.gravity.y * (0.03f);// fall slightly faster than you rise		

		if (dodge)
		{
			rb.AddForce (Vector2.right * 1500f);
		}

		if (jump)
		{
			rb.AddForce (new Vector2 (0f, jumpForce));
			jump = false;
		}

		if (distance < -1f && !targetRight) 
		{
			targetRight = true;
			Flip ();
		} 

		else if (distance > 1f && targetRight) 
		{
			targetRight = false;
			Flip ();
		}


	}

	void AiCheck()
	{		
		if ((health / startHealth) > 0.4) //agressive if hp over 50%
			aiState = (float)aiStateOptions.Agressive;
		else if ((health / startHealth) < 0.3)
			aiState = (float)aiStateOptions.Defensive;
		else
			aiState = (float)aiStateOptions.Defensive;
				 
		if (aiState == (float)aiStateOptions.Agressive) 
		{//behaviour for agressive ai 
			if (distance > 1.5f || distance < -1.5f)
				approaching = true;
			else
				approaching = false;
			if (distance < 1.5f && distance > -1.5f)
				Invoke ("Attack", attackDelay);
			if (distance > 4 || distance < -4) 					
					Throw ();
		} 
		else if (aiState == (float)aiStateOptions.Passive) 
		{
			
		} 
		else if (aiState == (float)aiStateOptions.Defensive) 
		{
			approaching = false;
			if (distance < 1.5f && distance > -1.5f)
				Attack ();
			else if (distance < 2f && distance > -2f) 
			{
				blocking = true;
				Invoke ("DropBlock", 1f);		
			}
			if (distance < 3 && distance > -3)
				running = true;
			else
				running = false;
			if (distance > 3 || distance < -3) 					
					Throw ();
		}
	}

	void Approach()
	{

	}

	void Attack()  
	{
		if (!hitStun && !blocking) 
		{
			anim.SetTrigger ("Attack");
			attackTiming = attackCd;
			attacking = true;
			attackCheck.SetActive (true);
		}
	}

	public void Throw()
	{
		if (knifeReady) 
		{
			knifeReady = false;
			Invoke ("ReadyKnife", 3f);

			var bullet = (GameObject)Instantiate (
				            knife,
				            knifeSpawnLoc.position,
				            knifeSpawnLoc.rotation);

			// Add velocity to the bullet
			if (targetRight)
				bullet.GetComponent<Rigidbody2D> ().AddForce (Vector2.right * 1000f);
			else
				bullet.GetComponent<Rigidbody2D> ().AddForce (Vector2.left * 1000f);

			Destroy (bullet, 3f);
		}
	}
	void ReadyKnife()
	{
		knifeReady = true;
	}

	void Flip()
	{
		float currentX = rb.velocity.x;
		rb.AddForce (new Vector2 (currentX, 0f));
		facingRight= !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void HealthUpdate()//updates the healthbar
	{
		Vector3 healthBarScale = healthBar.transform.localScale;
		healthBarScale.x = ((health / startHealth));
		healthBar.transform.localScale = healthBarScale;
	}

	public void HitstunEnd ()
	{
		anim.SetBool ("HitStun", false);
		hitStun = false;
	}

	void OnHit(float damage)
	{
		if (blocking)
			health -= damage/5;
		else
			health -= damage;
		anim.SetBool ("HitStun", true);
		hitStun = true;
		Invoke ("HitstunEnd", iFrames);
		rb.velocity = Vector3.zero;
		if (targetRight)//inelegant form of knockback
			rb.AddForce (Vector2.left * moveForce);
		else
			rb.AddForce (Vector2.right * moveForce);
		HealthUpdate ();


		if (health <= 0)
			gameControler.GetComponent<GameController> ().AiDeath ();
		
	}

	public void KnifeReact ()
	{
		blocking = true;
		Invoke ("DropBlock", 1f);
	}

	void DropBlock()
	{
		blocking = false;
	}

	public void RoundStart()
	{
		hitStun = false;
		knifeReady = true;
		HealthUpdate ();
		attackCheck.SetActive(false);
		aiState = (float)aiStateOptions.Agressive;
		InvokeRepeating ("AiCheck", 4f, aiPollRate);
	}

	public void Reset()//used by game conroler to reset and setup rounds
	{
		this.gameObject.transform.position = startLoc;
		health = startHealth;
		approaching = false;
		running = false;
		CancelInvoke ();
		rb.velocity = Vector3.zero;
		rb.Sleep ();
	}
}
