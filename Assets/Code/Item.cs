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
	public Controller cont;
	public float pitch, holdTime, buffer, shakeRange;
	public int scoreValue;

	// Use this for initialization
	void Start()
	{
		pitch = Random.Range(0.1f, 1.0f);
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
						prevState = currentState;
						StartShake();
						StopCoroutine("Cracking");
					}

					if((_currentPitch < pitch - (buffer))||(_currentPitch > pitch + (buffer)))
					{
						ChangeState(ItemState.CRACKING);
						iTween.Stop(this.gameObject);
					}

					if((_currentPitch < pitch - (shakeRange + buffer))||(_currentPitch > pitch + (shakeRange + buffer)))
					{
						currentState = ItemState.STILL;
						iTween.Stop(this.gameObject);
					}
					;
					break;

				case ItemState.CRACKING:
					if(currentState != prevState)
					{
						prevState = currentState; 
						OnPointShake();
						StartCoroutine("Cracking");
					}
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
		Debug.Log(_newState);
		prevState = currentState;
		currentState = _newState;
	}
}
