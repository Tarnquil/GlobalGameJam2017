using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chord : MonoBehaviour
{
	[SerializeField]
	RectTransform anchor1, anchor2;
	[SerializeField]
	LineRenderer line;

	// Use this for initialization
	void Start()
	{
		line.SetPosition(0, anchor1.position);
		line.SetPosition(2, anchor1.position);
	}
	
	// Update is called once per frame
	void Update()
	{
//		line.SetPosition(1, line.GetPosition(2) += new Vector3(1, 0));
	}
}
