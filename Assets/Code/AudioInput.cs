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
		hasMic = Microphone.devices.Length > 0;

		if(!hasMic)
		{
			return;
		}

		while(Microphone.GetPosition(null) > 0)
		{
		}

		src.clip = Microphone.Start(Microphone.devices[1], true, 10, 44100);
		src.Play();

	}
	
	// Update is called once per frame
	void Update()
	{

		//pause a little before setting clip to avoid lag and bugginess
		if(Time.time - timeSinceRestart > 0.5f&&!Microphone.IsRecording(null))
		{
			src.clip = Microphone.Start(null, true, 10, 44100);

			//wait until microphone position is found (?)
			while(!(Microphone.GetPosition(null) > 0))
			{
			}

			src.Play(); // Play the audio source
		}

	}
}
