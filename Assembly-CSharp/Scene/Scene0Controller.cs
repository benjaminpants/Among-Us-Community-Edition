using System;
using System.Collections;
using UnityEngine;

public class Scene0Controller : SceneController
{
	public float Duration = 3f;

	public SpriteRenderer[] ExtraBoys;

	public AnimationCurve PopInCurve;

	public AnimationCurve PopOutCurve;

	public float OutDuration = 0.2f;

	public void OnEnable()
	{
		StartCoroutine(Run());
	}

	public void OnDisable()
	{
		for (int i = 0; i < ExtraBoys.Length; i++)
		{
			ExtraBoys[i].enabled = false;
		}
	}

	private IEnumerator Run()
	{
		int lastBoy = 0;
		float start = Time.time;
		while (true)
		{
			float num = (Time.time - start) / Duration;
			int num2 = Mathf.RoundToInt((Mathf.Cos((float)Math.PI * num + (float)Math.PI) + 1f) / 2f * (float)ExtraBoys.Length);
			if (lastBoy < num2)
			{
				StartCoroutine(PopIn(ExtraBoys[lastBoy]));
				lastBoy = num2;
			}
			else if (lastBoy > num2)
			{
				lastBoy = num2;
				StartCoroutine(PopOut(ExtraBoys[lastBoy]));
			}
			yield return null;
		}
	}

	private IEnumerator PopIn(SpriteRenderer boy)
	{
		boy.enabled = true;
		for (float timer = 0f; timer < 0.2f; timer += Time.deltaTime)
		{
			float num = PopInCurve.Evaluate(timer / 0.2f);
			boy.transform.localScale = new Vector3(num, num, num);
			yield return null;
		}
		boy.transform.localScale = Vector3.one;
	}

	private IEnumerator PopOut(SpriteRenderer boy)
	{
		boy.enabled = true;
		for (float timer = 0f; timer < OutDuration; timer += Time.deltaTime)
		{
			float num = PopOutCurve.Evaluate(timer / OutDuration);
			boy.transform.localScale = new Vector3(num, num, num);
			yield return null;
		}
		boy.transform.localScale = Vector3.one;
		boy.enabled = false;
	}
}
