using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000103 RID: 259
public class ShhhBehaviour : MonoBehaviour
{
	// Token: 0x06000583 RID: 1411 RVA: 0x00023458 File Offset: 0x00021658
	public void OnEnable()
	{
		if (this.Autoplay)
		{
			Vector3 localScale = default(Vector3);
			this.UpdateHand(ref localScale, 1f);
			this.UpdateText(ref localScale, 1f);
			localScale.Set(1f, 1f, 1f);
			this.Body.transform.localScale = localScale;
			this.TextImage.color = Color.white;
		}
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x0000581D File Offset: 0x00003A1D
	public IEnumerator PlayAnimation()
	{
		base.StartCoroutine(this.AnimateHand());
		yield return this.AnimateText();
		yield return ShhhBehaviour.WaitWithInterrupt(this.HoldDuration);
		yield break;
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x0000582C File Offset: 0x00003A2C
	public void Update()
	{
		this.Background.transform.Rotate(0f, 0f, Time.deltaTime * this.RotateSpeed);
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x00005854 File Offset: 0x00003A54
	private IEnumerator AnimateText()
	{
		this.TextImage.color = Palette.ClearWhite;
		for (float t = 0f; t < this.Delay; t += Time.deltaTime)
		{
			yield return null;
		}
		Vector3 vec = default(Vector3);
		for (float t = 0f; t < this.PulseDuration; t += Time.deltaTime)
		{
			float num = t / this.PulseDuration;
			float num2 = 1f + Mathf.Sin(3.14159274f * num) * this.PulseSize;
			vec.Set(num2, num2, 1f);
			this.Body.transform.localScale = vec;
			this.TextImage.color = Color.Lerp(Palette.ClearWhite, Palette.White, num * 2f);
			yield return null;
		}
		vec.Set(1f, 1f, 1f);
		this.Body.transform.localScale = vec;
		this.TextImage.color = Color.white;
		yield break;
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x00005863 File Offset: 0x00003A63
	private IEnumerator AnimateHand()
	{
		this.Hand.transform.localPosition = this.HandTarget.min;
		Vector3 vec = default(Vector3);
		for (float t = 0f; t < this.Duration; t += Time.deltaTime)
		{
			float p = t / this.Duration;
			this.UpdateHand(ref vec, p);
			yield return null;
		}
		this.UpdateHand(ref vec, 1f);
		yield break;
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x000234C8 File Offset: 0x000216C8
	private void UpdateHand(ref Vector3 vec, float p)
	{
		this.HandTarget.LerpUnclamped(ref vec, this.PositionEasing.Evaluate(p), -1f);
		this.Hand.transform.localPosition = vec;
		vec.Set(0f, 0f, this.HandRotate.LerpUnclamped(this.RotationEasing.Evaluate(p)));
		this.Hand.transform.eulerAngles = vec;
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x00005872 File Offset: 0x00003A72
	private void UpdateText(ref Vector3 vec, float p)
	{
		this.TextTarget.LerpUnclamped(ref vec, p, -2f);
		this.TextImage.transform.localPosition = vec;
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x0000589C File Offset: 0x00003A9C
	public static IEnumerator WaitWithInterrupt(float duration)
	{
		float timer = 0f;
		while (timer < duration && !ShhhBehaviour.CheckForInterrupt())
		{
			yield return null;
			timer += Time.deltaTime;
		}
		yield break;
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x000058AB File Offset: 0x00003AAB
	public static bool CheckForInterrupt()
	{
		return Input.anyKeyDown;
	}

	// Token: 0x04000550 RID: 1360
	public SpriteRenderer Background;

	// Token: 0x04000551 RID: 1361
	public SpriteRenderer Body;

	// Token: 0x04000552 RID: 1362
	public SpriteRenderer Hand;

	// Token: 0x04000553 RID: 1363
	public SpriteRenderer TextImage;

	// Token: 0x04000554 RID: 1364
	public float RotateSpeed = 15f;

	// Token: 0x04000555 RID: 1365
	public Vector2Range HandTarget;

	// Token: 0x04000556 RID: 1366
	public AnimationCurve PositionEasing;

	// Token: 0x04000557 RID: 1367
	public FloatRange HandRotate;

	// Token: 0x04000558 RID: 1368
	public AnimationCurve RotationEasing;

	// Token: 0x04000559 RID: 1369
	public Vector2Range TextTarget;

	// Token: 0x0400055A RID: 1370
	public AnimationCurve TextEasing;

	// Token: 0x0400055B RID: 1371
	public float Duration = 0.5f;

	// Token: 0x0400055C RID: 1372
	public float Delay = 0.1f;

	// Token: 0x0400055D RID: 1373
	public float TextDuration = 0.5f;

	// Token: 0x0400055E RID: 1374
	public float PulseDuration = 0.1f;

	// Token: 0x0400055F RID: 1375
	public float PulseSize = 0.1f;

	// Token: 0x04000560 RID: 1376
	public float HoldDuration = 2f;

	// Token: 0x04000561 RID: 1377
	public bool Autoplay;
}
