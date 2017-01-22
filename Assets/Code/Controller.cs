using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
	public enum GameState
	{
		NULL,
		START,
		IN_GAME,
		END,
	}

	[SerializeField]
	GameObject[] items;
	[SerializeField]
	Item currentItem = null;
	[SerializeField]
	float currentPlayerPitch = 0.0f, timerTime;
	[SerializeField]
	AudioInput micInput;
	[SerializeField]
	Slider volSlider, aimSlider;
	[SerializeField]
	Text scoreLabel, timeLabel, endGameScore;
	[SerializeField]
	ParticleSystem rubble;
	[SerializeField]
	List<GameObject> uiGroups;

	GameState currentState, prevState;

	int score;

	// Use this for initialization
	void Start()
	{
		SpawnItem();
		timerTime = 60;
		score = 0;
		currentState = GameState.NULL;
		prevState = GameState.NULL;
		ChangeState(GameState.START);
	}
		
	// Update is called once per frame
	void Update()
	{
		switch(currentState)
		{
			case GameState.IN_GAME:

				if(prevState != currentState)
				{
					Debug.Log("Pings");
					StartCoroutine("StartTimer");
					prevState = currentState;

				}

				if(micInput.pitchValue >= 0)
				{
					currentPlayerPitch = micInput.pitchValue;
				}

				if(currentItem != null)
				{
					aimSlider.value = (1.0f / 100.0f) * (float)currentItem.pitch;
					currentItem.ReportPitch(currentPlayerPitch);
				}

				if(Input.GetKeyDown(KeyCode.UpArrow))
				{
					currentPlayerPitch += 0.01f;
				}

				if(Input.GetKeyDown(KeyCode.DownArrow))
				{
					currentPlayerPitch -= 0.01f;
				}
				volSlider.value = (1.0f / 100.0f) * (float)currentPlayerPitch;
				//
				break;
		}
	}

	public void StartShake()
	{
		Hashtable twnHsh = new Hashtable();
		twnHsh.Add("x", 0.1f);
		twnHsh.Add("y", 0.1f);
		twnHsh.Add("time", 10.0f);
		twnHsh.Add("looptype", iTween.LoopType.loop);
		twnHsh.Add("easetype", iTween.EaseType.linear);
		iTween.ShakePosition(this.gameObject, twnHsh);
		rubble.Play();
	}

	public void StopShake()
	{
		iTween.Stop(this.gameObject);
		rubble.Stop();
	}

	public void ItemSmashed(int scoreValue)
	{
		score += scoreValue;
		scoreLabel.text = string.Format("Score: {0}", score);
		this.gameObject.GetComponent<AudioSource>().Play();
		SpawnItem();
	}


	void SpawnItem()
	{
		GameObject newObject = (GameObject)Instantiate(items[Random.Range(0, items.Length)]);
		newObject.transform.position = new Vector3(-6, 3);
		newObject.GetComponent<
		Item>().TweenIn();
		newObject.GetComponent<Item>().cont = this;
		currentItem = newObject.GetComponent<Item>();
	}

	public void Vib()
	{
		Handheld.Vibrate();
	}

	public void StartButton()
	{
		ChangeState(GameState.IN_GAME);
	}

	public void ReStartButton()
	{
		timerTime = 60;
		scoreLabel.text = string.Format("Score: {0}", 0);
		score = 0;
		ChangeState(GameState.START);
	}

	public void ChangeState(GameState _toSwitchTo)
	{
		prevState = currentState;
		currentState = _toSwitchTo;
		SwitchUiGroups(_toSwitchTo.ToString());
	}

	public void SwitchUiGroups(string _name)
	{
		foreach(GameObject uiGroup in uiGroups)
		{
			if(uiGroup.name == _name)
			{
				uiGroup.SetActive(true);
			} else
			{
				uiGroup.SetActive(false);
			}
		}
	}

	IEnumerator StartTimer()
	{
		while(timerTime > 0)
		{
			Debug.Log("cjfckg");
			timerTime -= (1.0f * Time.deltaTime);
			timerTime = Mathf.Clamp(timerTime, 0, 60);
			timeLabel.text = string.Format("Time: {0}", Mathf.RoundToInt(timerTime));
			yield return null;
		}
		endGameScore.text = string.Format("Score: {0}", score);
		ChangeState(GameState.END);
	}
}
