using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000022 RID: 34
public static class Effects
{
	// Token: 0x06000098 RID: 152 RVA: 0x00002792 File Offset: 0x00000992
	public static IEnumerator Wait(float duration)
	{
		for (float t = 0f; t < duration; t += Time.deltaTime)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000099 RID: 153 RVA: 0x000027A1 File Offset: 0x000009A1
	public static IEnumerator All(params IEnumerator[] items)
	{
		Stack<IEnumerator>[] enums = new Stack<IEnumerator>[items.Length];
		for (int i = 0; i < items.Length; i++)
		{
			enums[i] = new Stack<IEnumerator>();
			enums[i].Push(items[i]);
		}
		int num;
		for (int cap = 0; cap < 100000; cap = num)
		{
			bool flag = false;
			for (int j = 0; j < enums.Length; j++)
			{
				if (enums[j].Count > 0)
				{
					flag = true;
					IEnumerator enumerator = enums[j].Peek();
					if (enumerator.MoveNext())
					{
						if (enumerator.Current is IEnumerator)
						{
							enums[j].Push((IEnumerator)enumerator.Current);
						}
					}
					else
					{
						enums[j].Pop();
					}
				}
			}
			if (!flag)
			{
				break;
			}
			yield return null;
			num = cap + 1;
		}
		yield break;
	}

	// Token: 0x0600009A RID: 154 RVA: 0x000027B0 File Offset: 0x000009B0
	internal static IEnumerator ScaleIn(Transform self, float source, float target, float duration)
	{
		if (!self)
		{
			yield break;
		}
		Vector3 localScale = default(Vector3);
		for (float t = 0f; t < duration; t += Time.deltaTime)
		{
			localScale.x = (localScale.y = (localScale.z = Mathf.SmoothStep(source, target, t / duration)));
			self.localScale = localScale;
			yield return null;
		}
		localScale.z = target;
		localScale.y = target;
		localScale.x = target;
		self.localScale = localScale;
		yield break;
	}

	// Token: 0x0600009B RID: 155 RVA: 0x000027D4 File Offset: 0x000009D4
	internal static IEnumerator CycleColors(SpriteRenderer self, Color source, Color target, float rate, float duration)
	{
		if (!self)
		{
			yield break;
		}
		self.enabled = true;
		for (float t = 0f; t < duration; t += Time.deltaTime)
		{
			float t2 = Mathf.Sin(t * 3.14159274f / rate) / 2f + 0.5f;
			self.color = Color.Lerp(source, target, t2);
			yield return null;
		}
		self.color = source;
		yield break;
	}

	// Token: 0x0600009C RID: 156 RVA: 0x00002800 File Offset: 0x00000A00
	internal static IEnumerator PulseColor(SpriteRenderer self, Color source, Color target, float duration = 0.5f)
	{
		if (!self)
		{
			yield break;
		}
		self.enabled = true;
		for (float t = 0f; t < duration; t += Time.deltaTime)
		{
			self.color = Color.Lerp(target, source, t / duration);
			yield return null;
		}
		self.color = source;
		yield break;
	}

	// Token: 0x0600009D RID: 157 RVA: 0x00002824 File Offset: 0x00000A24
	public static IEnumerator ColorFade(SpriteRenderer self, Color source, Color target, float duration)
	{
		if (!self)
		{
			yield break;
		}
		self.enabled = true;
		for (float t = 0f; t < duration; t += Time.deltaTime)
		{
			self.color = Color.Lerp(source, target, t / duration);
			yield return null;
		}
		self.color = target;
		yield break;
	}

	// Token: 0x0600009E RID: 158 RVA: 0x00002848 File Offset: 0x00000A48
	public static IEnumerator Rotate2D(Transform target, float source, float dest, float duration = 0.75f)
	{
		Vector3 temp = target.localEulerAngles;
		for (float time = 0f; time < duration; time += Time.deltaTime)
		{
			float t = time / duration;
			temp.z = Mathf.SmoothStep(source, dest, t);
			target.localEulerAngles = temp;
			yield return null;
		}
		temp.z = dest;
		target.localEulerAngles = temp;
		yield break;
	}

	// Token: 0x0600009F RID: 159 RVA: 0x0000286C File Offset: 0x00000A6C
	public static IEnumerator Slide2D(Transform target, Vector2 source, Vector2 dest, float duration = 0.75f)
	{
		Vector3 temp = default(Vector3);
		temp.z = target.localPosition.z;
		for (float time = 0f; time < duration; time += Time.deltaTime)
		{
			float t = time / duration;
			temp.x = Mathf.SmoothStep(source.x, dest.x, t);
			temp.y = Mathf.SmoothStep(source.y, dest.y, t);
			target.localPosition = temp;
			yield return null;
		}
		temp.x = dest.x;
		temp.y = dest.y;
		target.localPosition = temp;
		yield break;
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x00002890 File Offset: 0x00000A90
	public static IEnumerator Bounce(Transform target, float duration = 0.3f, float height = 0.15f)
	{
		if (!target)
		{
			yield break;
		}
		Vector3 origin = target.localPosition;
		Vector3 temp = origin;
		for (float timer = 0f; timer < duration; timer += Time.deltaTime)
		{
			float num = timer / duration;
			float num2 = 1f - num;
			temp.y = origin.y + height * Mathf.Abs(Mathf.Sin(num * 3.14159274f * 3f)) * num2;
			if (!target)
			{
				yield break;
			}
			target.localPosition = temp;
			yield return null;
		}
		if (target)
		{
			target.transform.localPosition = origin;
		}
		yield break;
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x000028AD File Offset: 0x00000AAD
	public static IEnumerator Shake(Transform target, float duration = 0.75f, float halfWidth = 0.25f)
	{
		if (Effects.activeShakes.Add(target))
		{
			Vector3 origin = target.localPosition;
			for (float timer = 0f; timer < duration; timer += Time.deltaTime)
			{
				float num = timer / duration;
				target.localPosition = origin + Vector3.right * (halfWidth * Mathf.Sin(num * 30f) * (1f - num));
				yield return null;
			}
			target.transform.localPosition = origin;
			Effects.activeShakes.Remove(target);
			origin = default(Vector3);
			origin = default(Vector3);
			origin = default(Vector3);
		}
		yield break;
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x000028CA File Offset: 0x00000ACA
	public static IEnumerator Bloop(float delay, Transform target, float duration = 0.5f)
	{
		for (float t = 0f; t < delay; t += Time.deltaTime)
		{
			yield return null;
		}
		Vector3 localScale = default(Vector3);
		for (float t = 0f; t < duration; t += Time.deltaTime)
		{
			float z = Effects.ElasticOut(t, duration);
			localScale.x = (localScale.y = (localScale.z = z));
			target.localScale = localScale;
			yield return null;
		}
		localScale.x = (localScale.y = (localScale.z = 1f));
		target.localScale = localScale;
		yield break;
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x0000D1FC File Offset: 0x0000B3FC
	private static float ElasticOut(float time, float duration)
	{
		time /= duration;
		float num = time * time;
		float num2 = num * time;
		return 33f * num2 * num + -106f * num * num + 126f * num2 + -67f * num + 15f * time;
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x000028F3 File Offset: 0x00000AF3
	public static IEnumerator BloopHalf(float delay, Transform target, float duration = 0.5f)
	{
		for (float t = 0f; t < delay; t += Time.deltaTime)
		{
			yield return null;
		}
		Vector3 a = default(Vector3);
		Vector3 mult = new Vector3(0.5f, 1f, 1f);
		for (float t = 0f; t < duration; t += Time.deltaTime)
		{
			float z = Effects.ElasticOut(t, duration);
			a.x = (a.y = (a.z = z));
			target.localScale = Vector3.Scale(a, mult);
			yield return null;
		}
		a.x = (a.y = (a.z = 1f));
		target.localScale = Vector3.Scale(a, mult);
		yield break;
	}

	// Token: 0x040000C8 RID: 200
	private static HashSet<Transform> activeShakes = new HashSet<Transform>();
}
