using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour {

	Animator playerAnimator;
	Rigidbody2D rb;
	public Vector3 startLoc;
	public Transform groundCheck;
	public GameObject attackCheck;
	public GameObject gameControler;
	public GameObject healthBar;

	public GameObject knife;
	public Transform knifeSpawnLoc;

	bool facingRight = true;
	bool jump = false;
	bool grounded = false;
	bool attacking = false;
	bool blocking = false;
	bool dodge = false;
	bool hitStun = false;
	bool knifeReady = true;

	float attackTiming = 0f;
	float attackCd = 0.3f;
	float dodgeTiming = 0f;
	float dodgeCd = 0.1f;
	float moveForce = 365f;
	float maxspeed = 10f;
	float jumpForce = 600f;
	float fallMult = 1.03f;
	float iFrames = 0.3f;
	float difficultyLevel;

	float startHealth = 200;
	public float health = 200;

	// Use this for initialization
	void Start () 
	{
		playerAnimator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		attackCheck.SetActive(false);
		startLoc = this.gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));

		if (grounded)
			playerAnimator.SetBool ("Jump", false);

		if (Input.GetKeyDown(KeyCode.W) && grounded && !hitStun) 
		{
			jump = true;
		}

		if (Input.GetKeyDown (KeyCode.LeftShift) && !dodge && attackTiming <= 0f && !hitStun) 
		{
			Dodge ();
			playerAnimator.SetBool ("Dodging", true);
		}

		if (Input.GetKeyDown (KeyCode.Space) && !attacking && !hitStun) 
		{
			attackTiming = attackCd;
			attacking = true;
			attackCheck.SetActive(true);
			playerAnimator.SetTrigger ("Attack");
		}

		if (Input.GetKeyDown(KeyCode.E)) 
		{
			playerAnimator.SetBool ("Blocking", true);
			blocking = true;
		}
		if (Input.GetKeyUp (KeyCode.E)) 
		{
			playerAnimator.SetBool ("Blocking", false);
			blocking = false;
		}

		if (Input.GetKeyDown (KeyCode.F) && !hitStun && knifeReady) 
		{
			playerAnimator.SetTrigger ("Throw");
			Throw ();
		}
		//if (Input.GetKeyDown (KeyCode.Keypad1))
		//Reset();
	}


	void FixedUpdate()
	{
		float h = Input.GetAxis ("Horizontal");

		if (h * rb.velocity.x < maxspeed && !hitStun)
			rb.AddForce (Vector2.right * h * moveForce);//if h is negative will move left
		
		if (rb.velocity.y < 0)
			rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMult - 1);// fall slightly faster than you rise
		
		if (Mathf.Abs (rb.velocity.x) > maxspeed && dodgeTiming == 0)
			rb.velocity = new Vector2 (Mathf.Sign (rb.velocity.x) * maxspeed, rb.velocity.y);//sign finds direction, and by multiplying it by max speed you move at max speed

		if (h > 0 && !facingRight)
			Flip ();
		else if (h < 0 && facingRight)
			Flip ();

		if (dodge) 
		{
			if (dodgeTiming > 0) 
			{
				dodgeTiming -= Time.deltaTime;
				rb.AddForce (Vector2.right * 1500f * h);
			} 
			else 
			{
				dodge = false;
				playerAnimator.SetBool ("Dodging", false);
			}
		}

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
	
		if (jump) 
		{
			playerAnimator.SetBool ("Jump", true);
			rb.AddForce (new Vector2 (0f, jumpForce));
			jump = false;
		}
	}
		
	public void Reset()
	{
		this.gameObject.transform.position = startLoc;
		health = startHealth;
		knifeReady = true;
		rb.velocity = Vector3.zero;
		rb.Sleep ();
		hitStun = true;
	}

	public void	RoundStart()
	{
		HealthUpdate ();
		Invoke ("HitStunEnd", 4f);//used to delay caracter actions untill round countdown is done
	}

	public void HitStunEnd ()//used to give control back to the character
	{
		hitStun = false;
		playerAnimator.SetBool ("HitStun", false);
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

	void Dodge()
	{
		Invoke ("HitStunEnd", iFrames);
		playerAnimator.SetTrigger ("Dodge");
		playerAnimator.SetBool ("HitStun", true);
		if (facingRight) 
			rb.AddForce (Vector2.right * jumpForce * 3);
		else
			rb.AddForce (Vector2.left * jumpForce * 3);
	}
	void ReadyKnife()
	{
		knifeReady = true;
	}

	void Throw()
	{
		// Create the Bullet from the Bullet Prefab
		knifeReady = false;
		Invoke ("ReadyKnife", 2f);
		var bullet = (GameObject)Instantiate(
			knife,
			knifeSpawnLoc.position,
			knifeSpawnLoc.rotation);

		// Add velocity to the bullet
		if (facingRight)
			bullet.GetComponent<Rigidbody2D>().AddForce(Vector2.right *1000f);
		else
			bullet.GetComponent<Rigidbody2D>().AddForce(Vector2.left *1000f);

		Destroy (bullet, 3f);
		if (difficultyLevel > 5)
			gameControler.GetComponent<GameController> ().KnifeReact ();
	}

	void OnHit(float damage)
	{
		if (!hitStun) 
		{
			if (blocking)
				health -= damage / 10;
			else
				health -= damage;
			hitStun = true;
			playerAnimator.SetBool ("HitStun",true);
			Invoke ("HitStunEnd", iFrames);
			rb.velocity = Vector3.zero;
			if (facingRight)//inelegant form of knockback
				rb.AddForce (Vector2.left * moveForce);
			else
				rb.AddForce (Vector2.right * moveForce);
			HealthUpdate ();

			if (health <= 0)
				gameControler.GetComponent<GameController> ().PlayerDeath ();
		}
	}
}
