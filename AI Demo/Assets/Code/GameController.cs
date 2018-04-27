using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public GameObject player;//reference to player object
	public GameObject enemy1;
	public GameObject enemy2;

	public Transform playerStartLoc;//reference to starting locations for spawning and round reset
	public Transform enemyStartLoc;

	public Text generalDisplay1;
	public Text generalDisplay2;
	public Text roundStartText;
	public Text countdown;
	public Text pSkillDisplay;
	public Text roundTime;
	enum series {Static,Scaling,Procedural}

	public float timer;
	public float pSkill = 1;
	public int seriesNo = 1;//the series type, 1 is static,2 is scaling, 3 is procedural
	public int round;

	void Start()
	{
		generalDisplay1.enabled = true;
		generalDisplay1.text = ("Input A Series Number");
		generalDisplay2.enabled = false;
	}

	void Update()
	{
		pSkillDisplay.text = ("Round " + round);
		countdown.text = (Mathf.Floor(timer).ToString());

		//keypad inputs for examiner use
		if (Input.GetKeyDown (KeyCode.Keypad1))
			ResetRound ();

		if (Input.GetKeyDown (KeyCode.Keypad2)) 
		{
			pSkill = 5;
			seriesNo = (int)series.Procedural;
			ResetRound ();
			generalDisplay1.enabled = true;
			generalDisplay1.text = "High Skill Debug";
		}

		if (Input.GetKeyDown (KeyCode.Keypad3))
			generalDisplay1.enabled = false;

		if (Input.GetKeyDown(KeyCode.Keypad4))
		{
			generalDisplay1.enabled = true;
			generalDisplay1.text = (pSkill.ToString());
		}

		if (Input.GetKeyDown (KeyCode.Keypad5)) 
		{
			generalDisplay1.enabled = true;
			generalDisplay1.text = ("Free Play");
			enemy1.GetComponent<EnemyControler> ().Reset ();
			player.GetComponent<PlayerControler> ().Reset ();
			player.GetComponent<PlayerControler> ().HitStunEnd ();
		}

		if (Input.GetKeyDown (KeyCode.Keypad7)) //7 on the numpad starts the static playthrough
		{
			pSkill = 1;
			seriesNo = (int)series.Static;
			round = 1;
			ResetRound ();
			generalDisplay1.enabled = false;
		}

		if (Input.GetKeyDown (KeyCode.Keypad8)) //8 on the numpad starts the scaling playthrough
		{
			pSkill = 1;
			seriesNo = (int)series.Scaling;
			round = 1;
			ResetRound ();
			generalDisplay1.enabled = false;
		}

		if (Input.GetKeyDown (KeyCode.Keypad9)) //9 on the numpad starts the procedural playthrough
		{
			pSkill = 1;
			seriesNo = (int)series.Procedural;
			round = 1;
			ResetRound ();
			generalDisplay1.enabled = false;
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

	public void EnemyDeath()
	{
		//congrats
		enemy1.SetActive(false);
		RoundEnd();
	}

	void ResetSeries()//returns to first round of series
	{
		pSkill = 1;
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

	void RoundEnd()//when player defeats opponent
	{
		round += 1;
		if (round > 5)
		{
			generalDisplay1.enabled = true;
			generalDisplay1.text = ("congrats, you're a third done");
			generalDisplay2.enabled = true;
			generalDisplay2.text = ("now answer some questions please");
		}

		else if (seriesNo == (int)series.Static)
			ResetRound ();
		else if (seriesNo == (int)series.Scaling) 
		{
			pSkill += 1;
			ResetRound ();
		} 
		else if (seriesNo == (int)series.Procedural) 
		{
			if (player == null) {}
			else
			{
				float remainingHealth = player.GetComponent<PlayerControler> ().health;
				pSkill += (remainingHealth / 100 * 2); //get player health /100 
				//time based skill addition to be added later
				ResetRound();
			
			}
		}
	}
	public void KnifeReact()
	{
		enemy1.GetComponent<EnemyControler> ().KnifeReact ();
	}
}
