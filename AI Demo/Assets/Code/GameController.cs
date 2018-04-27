using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public GameObject player;//reference to player object
	public GameObject enemy1;

	public Transform playerStartLoc;//reference to starting locations for spawning and round reset
	public Transform enemyStartLoc;

	public Text generalDisplay1;
	public Text generalDisplay2;
	public Text roundStartText;
	public Text countdown;
	public Text roundTime;
	enum series {Static,Scaling,Procedural}

	public float timer;
	public int round;

	void Start()
	{
		generalDisplay1.enabled = true;
		generalDisplay1.text = ("Press numpad1 to start");
		generalDisplay2.enabled = false;
	}

	void Update()
	{
		countdown.text = (Mathf.Floor(timer).ToString());

		//keypad inputs for setup
		if (Input.GetKeyDown (KeyCode.Keypad1))
			ResetRound ();

		if (Input.GetKeyDown (KeyCode.Keypad5)) 
		{
			generalDisplay1.enabled = true;
			generalDisplay1.text = ("Free Play");
			enemy1.GetComponent<EnemyControler> ().Reset ();
			player.GetComponent<PlayerControler> ().Reset ();
			player.GetComponent<PlayerControler> ().HitStunEnd ();
		}

	}

	void FixedUpdate()
	{
		timer -= Time.deltaTime;
		if (timer < 0f) 
		{
			countdown.enabled = false;
			roundStartText.enabled = false;
		} 

	}
	public void PlayerDeath()// to be called from player controler upon health reaching 0
	{
		player.SetActive(false);
		generalDisplay1.enabled = true;
		generalDisplay1.text = ("How you gonna go and die like that?");
		generalDisplay2.enabled = true;
		generalDisplay2.text = ("Hit numpad1 to restart round");
	}

	public void AiDeath()
	{
		//congrats
		enemy1.SetActive(false);
		RoundEnd();
	}

	void ResetSeries()//returns to first round of series
	{
		ResetRound ();
	}

	void ResetRound()//returns to starting positions and resets health
	{
		generalDisplay1.enabled = false;
		generalDisplay2.enabled = false;
		player.SetActive (true);
		enemy1.SetActive (true);
		player.GetComponent<PlayerControler> ().Reset ();
		enemy1.GetComponent<EnemyControler> ().Reset ();
		RoundStart ();
	}

	void RoundStart()//activates all the things that need to be done for the round to actually start
	{
		roundStartText.enabled = true;
		countdown.enabled = true;
		timer = 4f;
		enemy1.GetComponent<EnemyControler> ().RoundStart ();
		player.GetComponent<PlayerControler> ().RoundStart ();
	}

	void RoundEnd()//when player defeats AI
	{

	}

	public void KnifeReact()
	{
		enemy1.GetComponent<EnemyControler> ().KnifeReact ();
	}
}
