using System.Collections;
using UnityEngine;

public class DiscussBehaviour : MonoBehaviour
{
	public SpriteRenderer LeftPlayer;

	public SpriteRenderer RightPlayer;

	public SpriteRenderer Text;

	public FloatRange RotateRange = new FloatRange(-5f, 5f);

	public Vector2Range TextTarget;

	public AnimationCurve TextEasing;

	public float Delay = 0.1f;

	public float TextDuration = 0.5f;

	public float HoldDuration = 2f;

	private Vector3 vec;

	public IEnumerator PlayAnimation()
	{
		Text.transform.localPosition = TextTarget.min;
		yield return AnimateText();
		yield return ShhhBehaviour.WaitWithInterrupt(HoldDuration);
	}

	public void Update()
	{
		vec.Set(0f, 0f, RotateRange.Lerp(Mathf.PerlinNoise(1f, Time.time * 8f)));
		LeftPlayer.transform.eulerAngles = vec;
		vec.Set(0f, 0f, RotateRange.Lerp(Mathf.PerlinNoise(2f, Time.time * 8f)));
		RightPlayer.transform.eulerAngles = vec;
	}

	private IEnumerator AnimateText()
	{
		for (float t2 = 0f; t2 < Delay; t2 += Time.deltaTime)
		{
			yield return null;
		}
		Vector3 vec = default(Vector3);
		for (float t2 = 0f; t2 < TextDuration; t2 += Time.deltaTime)
		{
			float time = t2 / TextDuration;
			UpdateText(ref vec, TextEasing.Evaluate(time));
			yield return null;
		}
		UpdateText(ref vec, 1f);
	}

	private void UpdateText(ref Vector3 vec, float p)
	{
		TextTarget.LerpUnclamped(ref vec, p, -7f);
		Text.transform.localPosition = vec;
	}
}
