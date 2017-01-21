using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioInput : MonoBehaviour
{
	[SerializeField]
	AudioSource src;

	bool hasMic = false;
	public float timeSinceRestart, pitchValue, prevPitch;
	int recordLength = 1, samplingSize = 30;
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
		src.clip = Microphone.Start(Microphone.devices[0], true, 999, AudioSettings.outputSampleRate);//Microphone.Start(Microphone.devices[1], false, 999, 44100);
		src.Play();
	}

	void Update()
	{
		
		src.GetOutputData(samples, 0);
		if(normalisingSamples.Count > samplingSize)
		{
			normalisingSamples.RemoveAt(0);
		}

		float sum = 0;
		for(int i = 0; i < 1024; i++)
		{
			sum += Mathf.Pow(samples[i], 2);
		}

		float maxV = 0;
		int maxN = 0;

		for(int i = 0; i < samples.Length; i++)
		{
			if(samples[i] > maxV&&samples[i] > THRESHOLD)
			{
				maxV = samples[i];
				maxN = i;
			}
		}

		float freqN = maxN;

		if(maxN > 0&&maxN < 1024 - 1)
		{
			float dL = samples[maxN - 1] / samples[maxN];
			float dR = samples[maxN + 1] / samples[maxN];
			freqN += 0.5f * (dR * dR - dL * dL);
		}
	
		pitchValue = freqN * 24000 / 1024;

		pitchValue = Mathf.Clamp(pitchValue, 1000, 22689);
		pitchValue = ((pitchValue - 1000) / (22689 - 1000));

		normalisingSamples.Add(pitchValue);
		foreach(float samp in normalisingSamples)
		{
			pitchValue = Mathf.Lerp(pitchValue, samp, 0.5f);
		}

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
