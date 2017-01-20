using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioInput : MonoBehaviour
{
	[SerializeField]
	AudioSource src;

	bool hasMic = false;
	float timeSinceRestart;

	// Use this for initialization
	void Start()
	{

		while(Microphone.GetPosition(null) > 0)
		{
		}

		src.clip = Microphone.Start(Microphone.devices[1], true, 10, 44100);
		src.Play();

	}
	
	// Update is called once per frame
	void Update()
	{

	}
}
