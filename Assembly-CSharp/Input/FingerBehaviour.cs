using System.Collections;
using UnityEngine;

public class FingerBehaviour : MonoBehaviour
{
	public static class Quadratic
	{
		public static float InOut(float k)
		{
			if (k < 0f)
			{
				k = 0f;
			}
			if (k > 1f)
			{
				k = 1f;
			}
			if ((k *= 2f) < 1f)
			{
				return 0.5f * k * k;
			}
			return -0.5f * ((k -= 1f) * (k - 2f) - 1f);
		}
	}

	public SpriteRenderer Finger;

	public SpriteRenderer Click;

	public float liftedAngle = -20f;

	public IEnumerator DoClick(float duration)
	{
		for (float time = 0f; time < duration; time += Time.deltaTime)
		{
			float num = time / duration;
			if (num < 0.4f)
			{
				float num2 = num / 0.4f;
				num2 = num2 * 2f - 1f;
				if (num2 < 0f)
				{
					float fingerAngle = Mathf.Lerp(liftedAngle, liftedAngle * 2f, 1f + Mathf.Abs(num2));
					SetFingerAngle(fingerAngle);
				}
				else
				{
					float fingerAngle2 = Mathf.Lerp(liftedAngle * 2f, 0f, num2);
					SetFingerAngle(fingerAngle2);
				}
			}
			else if (num < 0.7f)
			{
				ClickOn();
			}
			else
			{
				float t = (num - 0.7f) / 0.3f;
				Click.enabled = false;
				float fingerAngle3 = Mathf.Lerp(0f, liftedAngle, t);
				SetFingerAngle(fingerAngle3);
			}
			yield return null;
		}
		ClickOff();
	}

	private void SetFingerAngle(float angle)
	{
		Finger.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
	}

	public void ClickOff()
	{
		Click.enabled = false;
		SetFingerAngle(liftedAngle);
	}

	public void ClickOn()
	{
		Click.enabled = true;
		SetFingerAngle(0f);
	}

	public IEnumerator MoveTo(Vector2 target, float duration)
	{
		Vector3 startPos = base.transform.position;
		Vector3 targetPos = target;
		targetPos.z = startPos.z;
		for (float time = 0f; time < duration; time += Time.deltaTime)
		{
			float t = time / duration;
			base.transform.position = Vector3.Lerp(startPos, targetPos, t);
			yield return null;
		}
		base.transform.position = targetPos;
	}
}
