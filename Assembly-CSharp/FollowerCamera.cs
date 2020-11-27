using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200008F RID: 143
[ExecuteInEditMode]
internal class FollowerCamera : MonoBehaviour
{
	// Token: 0x060002FE RID: 766 RVA: 0x000165D4 File Offset: 0x000147D4
	public void FixedUpdate()
	{
		if (this.Target && !this.Locked)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.Target.transform.position + (Vector3)this.Offset, 5f * Time.deltaTime);
			if (this.shakeAmount > 0f)
			{
				float num = Mathf.PerlinNoise(0.5f, Time.time * this.shakePeriod) * 2f - 1f;
				float num2 = Mathf.PerlinNoise(Time.time * this.shakePeriod, 0.5f) * 2f - 1f;
				base.transform.Translate(num * this.shakeAmount, num2 * this.shakeAmount, 0f);
			}
		}
	}

	// Token: 0x060002FF RID: 767 RVA: 0x00003F94 File Offset: 0x00002194
	public void ShakeScreen(float duration, float severity)
	{
		base.StartCoroutine(this.CoShakeScreen(duration, severity));
	}

	// Token: 0x06000300 RID: 768 RVA: 0x00003FA5 File Offset: 0x000021A5
	private IEnumerator CoShakeScreen(float duration, float severity)
	{
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		for (float t = duration; t > 0f; t -= Time.fixedDeltaTime)
		{
			float d = t / duration;
			this.Offset = UnityEngine.Random.insideUnitCircle * d * severity;
			yield return wait;
		}
		this.Offset = Vector2.zero;
		yield break;
	}

	// Token: 0x06000301 RID: 769 RVA: 0x00003FC2 File Offset: 0x000021C2
	internal void SetTarget(MonoBehaviour target)
	{
		this.Target = target;
		base.transform.position = this.Target.transform.position + (Vector3)this.Offset;
	}

	// Token: 0x040002FC RID: 764
	public MonoBehaviour Target;

	// Token: 0x040002FD RID: 765
	public Vector2 Offset;

	// Token: 0x040002FE RID: 766
	public bool Locked;

	// Token: 0x040002FF RID: 767
	public float shakeAmount;

	// Token: 0x04000300 RID: 768
	public float shakePeriod = 1f;
}
