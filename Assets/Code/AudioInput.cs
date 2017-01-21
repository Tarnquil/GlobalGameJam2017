using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioInput : MonoBehaviour
{
	[SerializeField]
	AudioSource src;

	bool hasMic = false;
	public float pitchValue, prevPitch;
	public float minVol, maxVol;
	int recordLength = 1, samplingSize = 125;
	AudioClip storedClip, playingClip;
	float[] samples;

	List<float> normalisingSamples;

	// Use this for initialization
	void Start()
	{
		normalisingSamples = new List<float>();
		samples = new float[1024];
		storedClip = Microphone.Start(Microphone.devices[0], true, 999, AudioSettings.outputSampleRate);
	}

	void Update()
	{
		prevPitch = pitchValue;
		pitchValue = LevelMax();
		if(pitchValue > 0)
		{
			float vel = 0.5f;
			pitchValue = Mathf.SmoothDamp(prevPitch, pitchValue, ref vel, 0.5f);
		}
	}

	float  LevelMax()
	{
		float levelMax = 0;
		float[] waveData = new float[samplingSize];
		int micPosition = Microphone.GetPosition(null) - (samplingSize + 1);
		if(micPosition < 0)
		{
			return 0;
		}

		storedClip.GetData(waveData, micPosition);

		for(int i = 0; i < samplingSize; i++)
		{
			float wavePeak = waveData[i] * waveData[i];
			if(levelMax < wavePeak)
			{
				levelMax = wavePeak;
			}
		}

		levelMax = (float)Mathf.RoundToInt(levelMax * 1000);

		levelMax = Mathf.Clamp(levelMax, minVol, maxVol);


		return levelMax;
	}


	IEnumerator AudioRecordingLoop()
	{
		while(true)
		{
			yield return new WaitForSeconds(recordLength);
			src.clip = Microphone.Start(Microphone.devices[1], true, recordLength, 44100);
			src.Play();
		}

	}
}
