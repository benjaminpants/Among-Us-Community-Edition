using System;
using System.Collections;
using UnityEngine;

public class ShhhBehaviour : MonoBehaviour
{
	public SpriteRenderer Background;

	public SpriteRenderer Body;

	public SpriteRenderer Hand;

	public SpriteRenderer TextImage;

	public float RotateSpeed = 15f;

	public Vector2Range HandTarget;

	public AnimationCurve PositionEasing;

	public FloatRange HandRotate;

	public AnimationCurve RotationEasing;

	public Vector2Range TextTarget;

	public AnimationCurve TextEasing;

	public float Duration = 0.5f;

	public float Delay = 0.1f;

	public float TextDuration = 0.5f;

	public float PulseDuration = 0.1f;

	public float PulseSize = 0.1f;

	public float HoldDuration = 2f;

	public bool Autoplay;

	public void OnEnable()
	{
		if (Autoplay)
		{
			Vector3 vec = default(Vector3);
			UpdateHand(ref vec, 1f);
			UpdateText(ref vec, 1f);
			vec.Set(1f, 1f, 1f);
			Body.transform.localScale = vec;
			TextImage.color = Color.white;
		}
	}

	public IEnumerator PlayAnimation()
	{
		StartCoroutine(AnimateHand());
		yield return AnimateText();
		yield return WaitWithInterrupt(HoldDuration);
	}

	public void Update()
	{
		Background.transform.Rotate(0f, 0f, Time.deltaTime * RotateSpeed);
	}

	private IEnumerator AnimateText()
	{
		TextImage.color = Palette.ClearWhite;
		for (float t2 = 0f; t2 < Delay; t2 += Time.deltaTime)
		{
			yield return null;
		}
		Vector3 vec = default(Vector3);
		for (float t2 = 0f; t2 < PulseDuration; t2 += Time.deltaTime)
		{
			float num = t2 / PulseDuration;
			float num2 = 1f + Mathf.Sin((float)Math.PI * num) * PulseSize;
			vec.Set(num2, num2, 1f);
			Body.transform.localScale = vec;
			TextImage.color = Color.Lerp(Palette.ClearWhite, Palette.White, num * 2f);
			yield return null;
		}
		vec.Set(1f, 1f, 1f);
		Body.transform.localScale = vec;
		TextImage.color = Color.white;
	}

	private IEnumerator AnimateHand()
	{
		Hand.transform.localPosition = HandTarget.min;
		Vector3 vec = default(Vector3);
		for (float t = 0f; t < Duration; t += Time.deltaTime)
		{
			float p = t / Duration;
			UpdateHand(ref vec, p);
			yield return null;
		}
		UpdateHand(ref vec, 1f);
	}

	private void UpdateHand(ref Vector3 vec, float p)
	{
		HandTarget.LerpUnclamped(ref vec, PositionEasing.Evaluate(p), -1f);
		Hand.transform.localPosition = vec;
		vec.Set(0f, 0f, HandRotate.LerpUnclamped(RotationEasing.Evaluate(p)));
		Hand.transform.eulerAngles = vec;
	}

	private void UpdateText(ref Vector3 vec, float p)
	{
		TextTarget.LerpUnclamped(ref vec, p, -2f);
		TextImage.transform.localPosition = vec;
	}

	public static IEnumerator WaitWithInterrupt(float duration)
	{
		for (float timer = 0f; timer < duration; timer += Time.deltaTime)
		{
			if (CheckForInterrupt())
			{
				break;
			}
			yield return null;
		}
	}

	public static bool CheckForInterrupt()
	{
		return Input.anyKeyDown;
	}
}
