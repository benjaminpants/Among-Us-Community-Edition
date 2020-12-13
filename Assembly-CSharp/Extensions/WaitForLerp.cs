using System;
using System.Collections;
using UnityEngine;

public class WaitForLerp : IEnumerator
{
	private float duration;

	private float timer;

	private Action<float> act;

	public object Current => null;

	public WaitForLerp(float seconds, Action<float> act)
	{
		duration = seconds;
		this.act = act;
	}

	public bool MoveNext()
	{
		timer = Mathf.Min(timer + Time.deltaTime, duration);
		act(timer / duration);
		return timer < duration;
	}

	public void Reset()
	{
		timer = 0f;
	}
}
