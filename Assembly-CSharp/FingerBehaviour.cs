using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class FingerBehaviour : MonoBehaviour
{
	// Token: 0x06000377 RID: 887 RVA: 0x00004405 File Offset: 0x00002605
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
					float fingerAngle = Mathf.Lerp(this.liftedAngle, this.liftedAngle * 2f, 1f + Mathf.Abs(num2));
					this.SetFingerAngle(fingerAngle);
				}
				else
				{
					float fingerAngle2 = Mathf.Lerp(this.liftedAngle * 2f, 0f, num2);
					this.SetFingerAngle(fingerAngle2);
				}
			}
			else if (num < 0.7f)
			{
				this.ClickOn();
			}
			else
			{
				float t = (num - 0.7f) / 0.3f;
				this.Click.enabled = false;
				float fingerAngle3 = Mathf.Lerp(0f, this.liftedAngle, t);
				this.SetFingerAngle(fingerAngle3);
			}
			yield return null;
		}
		this.ClickOff();
		yield break;
	}

	// Token: 0x06000378 RID: 888 RVA: 0x0000441B File Offset: 0x0000261B
	private void SetFingerAngle(float angle)
	{
		this.Finger.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
	}

	// Token: 0x06000379 RID: 889 RVA: 0x0000443D File Offset: 0x0000263D
	public void ClickOff()
	{
		this.Click.enabled = false;
		this.SetFingerAngle(this.liftedAngle);
	}

	// Token: 0x0600037A RID: 890 RVA: 0x00004457 File Offset: 0x00002657
	public void ClickOn()
	{
		this.Click.enabled = true;
		this.SetFingerAngle(0f);
	}

	// Token: 0x0600037B RID: 891 RVA: 0x00004470 File Offset: 0x00002670
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
		yield break;
	}

	// Token: 0x04000365 RID: 869
	public SpriteRenderer Finger;

	// Token: 0x04000366 RID: 870
	public SpriteRenderer Click;

	// Token: 0x04000367 RID: 871
	public float liftedAngle = -20f;

	// Token: 0x020000A6 RID: 166
	public static class Quadratic
	{
		// Token: 0x0600037D RID: 893 RVA: 0x00017E8C File Offset: 0x0001608C
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
}
