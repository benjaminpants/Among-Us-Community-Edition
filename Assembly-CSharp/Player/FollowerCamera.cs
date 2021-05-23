using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
internal class FollowerCamera : MonoBehaviour
{
	public MonoBehaviour Target;

	public Vector2 Offset;

	public bool Locked;

	public float shakeAmount;

	public float shakePeriod = 1f;

	public void FixedUpdate()
	{
		if ((bool)Target && !Locked)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, Target.transform.position + (Vector3)Offset, 5f * Time.deltaTime);
			if (LobbyBehaviour.Instance == null)
            {
				return;
            }
			if (shakeAmount > 0f)
			{
				float num = Mathf.PerlinNoise(0.5f, Time.time * shakePeriod) * 2f - 1f;
				float num2 = Mathf.PerlinNoise(Time.time * shakePeriod, 0.5f) * 2f - 1f;
				base.transform.Translate(num * shakeAmount, num2 * shakeAmount, 0f);
			}
		}
	}

	public void ShakeScreen(float duration, float severity)
	{
		StartCoroutine(CoShakeScreen(duration, severity));
	}

	private IEnumerator CoShakeScreen(float duration, float severity)
	{
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		for (float t = duration; t > 0f; t -= Time.fixedDeltaTime)
		{
			float d = t / duration;
			Offset = Random.insideUnitCircle * d * severity;
			yield return wait;
		}
		Offset = Vector2.zero;
	}

	internal void SetTarget(MonoBehaviour target)
	{
		Target = target;
		base.transform.position = Target.transform.position + (Vector3)Offset;
	}
}
