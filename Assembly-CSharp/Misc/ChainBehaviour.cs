using System;
using UnityEngine;

public class ChainBehaviour : MonoBehaviour
{
	public FloatRange SwingRange = new FloatRange(0f, 30f);

	public float SwingPeriod = 2f;

	public float swingTime;

	private Vector3 vec;

	public void Awake()
	{
		swingTime = FloatRange.Next(0f, SwingPeriod);
		vec.z = SwingRange.Lerp(Mathf.Sin(swingTime));
		base.transform.eulerAngles = vec;
	}

	public void Update()
	{
		swingTime += Time.deltaTime / SwingPeriod;
		vec.z = SwingRange.Lerp(Mathf.Sin(swingTime * (float)Math.PI) / 2f + 0.5f);
		base.transform.eulerAngles = vec;
	}
}
