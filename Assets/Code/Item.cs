using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
	public enum ItemState
	{
		NULL,
		TWEENING_IN,
		STILL,
		SHAKING,
		CRACKING,
		SMASHED
	}

	[SerializeField]
	ItemState currentState, prevState;
	[SerializeField]
	GameObject puff;
	public Controller cont;
	public float pitch, holdTime, buffer, shakeRange, pitchRangeLow, pitchRangeHigh;
	public int scoreValue;

	// Use this for initialization
	void Start()
	{
		pitch = Random.Range(pitchRangeLow, pitchRangeHigh);
		TweenIn();
		currentState = ItemState.TWEENING_IN;
		prevState = ItemState.NULL;
	}

	void StartCracking()
	{
		StartCoroutine("Cracking");
	}

	public void ReportPitch(float _currentPitch)
	{
		if(currentState != ItemState.TWEENING_IN)
		{
			switch(currentState)
			{
				case ItemState.STILL:
					if(currentState != prevState)
					{
						prevState = currentState;
						this.gameObject.transform.rotation = Quaternion.identity;
						this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
						cont.StopShake();
					}

					if((_currentPitch > pitch - (shakeRange))&&(_currentPitch < pitch + (shakeRange)))
					{
						ChangeState(ItemState.SHAKING);
					}
					;
					break;

				case ItemState.SHAKING:
					if(currentState != prevState)
					{
						this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
						this.gameObject.transform.rotation = Quaternion.identity;
						prevState = currentState;
						StartShake();
						StopCoroutine("Cracking");
						cont.StopShake();
					}

					if((_currentPitch > pitch - (buffer))&&(_currentPitch < pitch + (buffer)))
					{
						ChangeState(ItemState.CRACKING);
						iTween.Stop(this.gameObject);
					} else if((_currentPitch > pitch - (shakeRange + buffer))||(_currentPitch < pitch + (shakeRange + buffer)))
					{
						ChangeState(ItemState.STILL);
						iTween.Stop(this.gameObject);
					}
					;
					break;

				case ItemState.CRACKING:
					if(currentState != prevState)
					{
						this.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
						prevState = currentState; 
						OnPointShake();
						StartCoroutine("Cracking");
						cont.StartShake();
					}

					if((_currentPitch < pitch - (buffer))||(_currentPitch > pitch + (buffer)))
					{
						ChangeState(ItemState.SHAKING); 
						iTween.Stop(this.gameObject);
					}

					Handheld.Vibrate();
					;
					break;
			}
		}
	}

	IEnumerator Cracking()
	{
		yield return new WaitForSeconds(holdTime / 2);
		yield return new WaitForSeconds(holdTime / 2);
		Smash();
	}

	public void Smash()
	{
		cont.ItemSmashed(scoreValue);
		Instantiate(puff, this.gameObject.transform.position, Quaternion.Euler(new Vector3(-180, 0, 0)));
		Destroy(this.gameObject);
	}

	public void TweenIn()
	{
		Hashtable twnHsh = new Hashtable();
		twnHsh.Add("position", GameObject.Find("frame").transform.position);
		twnHsh.Add("time", 0.5f);
		twnHsh.Add("easetype", iTween.EaseType.easeOutSine);
		twnHsh.Add("oncomplete", "Stopped");
		twnHsh.Add("oncompletetarget", this.gameObject);
		iTween.MoveTo(this.gameObject, twnHsh);
	}

	public void StartShake()
	{
		Hashtable twnHsh = new Hashtable();
		twnHsh.Add("z", 4.0f);
		twnHsh.Add("time", 1.0f);
		twnHsh.Add("looptype", iTween.LoopType.loop);
		twnHsh.Add("easetype", iTween.EaseType.linear);
		iTween.ShakeRotation(this.gameObject, twnHsh);
	}

	public void OnPointShake()
	{
		Hashtable twnHsh = new Hashtable();
		twnHsh.Add("z", 8.0f);
		twnHsh.Add("time", 1.0f);
		twnHsh.Add("looptype", iTween.LoopType.loop);
		twnHsh.Add("easetype", iTween.EaseType.linear);
		iTween.ShakeRotation(this.gameObject, twnHsh);
	}

	void Stopped()
	{
		ChangeState(ItemState.STILL);
	}

	void ChangeState(ItemState _newState)
	{
		prevState = currentState;
		currentState = _newState;
	}
}
