using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
	[SerializeField]
	GameObject[] items;
	[SerializeField]
	Item currentItem = null;
	[SerializeField]
	float currentPlayerPitch = 0.0f;
	[SerializeField]
	AudioInput micInput;
	[SerializeField]
	Text pitchLabel;

	int score;

	// Use this for initialization
	void Start()
	{
		SpawnItem();
		score = 0;
	}
	
	// Update is called once per frame
	void Update()
	{
		if(micInput.pitchValue >= 0)
		{
			currentPlayerPitch = micInput.pitchValue;
		}

		if(currentItem != null)
		{
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

		pitchLabel.text = currentPlayerPitch.ToString();

		Debug.Log(currentPlayerPitch);
	}

	public void ItemSmashed(int scoreValue)
	{
		score += scoreValue;
		SpawnItem();
	}

	void SpawnItem()
	{
		GameObject newObject = (GameObject)Instantiate(items[Random.Range(0, items.Length)]);
		newObject.transform.position = new Vector3(-6, 3);
		newObject.GetComponent<Item>().TweenIn();
		newObject.GetComponent<Item>().cont = this;
		currentItem = newObject.GetComponent<Item>();
	}
}
