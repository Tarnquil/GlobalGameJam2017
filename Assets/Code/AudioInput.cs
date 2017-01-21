using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioInput : MonoBehaviour
{
	[SerializeField]
	AudioSource src;

	bool hasMic = false;
	public float timeSinceRestart, pitchValue, prevPitch;
	int recordLength = 1, samplingSize = 2048;
	AudioClip storedClip, playingClip;
	private float[] samples;
	private float[] spectrum;
	private const float THRESHOLD = 0.02f;

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
		
		pitchValue = LevelMax();

	}

	float  LevelMax()
	{
		float levelMax = 0;
		float[] waveData = new float[samplingSize];
		int micPosition = Microphone.GetPosition(null) - (samplingSize + 1); // null means the first microphone
		if(micPosition < 0)
			return 0;
		storedClip.GetData(waveData, micPosition);
		// Getting a peak on the last 128 samples
		for(int i = 0; i < samplingSize; i++)
		{
			float wavePeak = waveData[i] * waveData[i];
			if(levelMax < wavePeak)
			{
				levelMax = wavePeak;
			}
		}

		levelMax = (float)Mathf.RoundToInt(levelMax * 1000);

		levelMax = Mathf.Clamp(levelMax, 0, 400);

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
