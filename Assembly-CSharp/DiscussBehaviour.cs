using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000100 RID: 256
public class DiscussBehaviour : MonoBehaviour
{
	// Token: 0x06000572 RID: 1394 RVA: 0x00005769 File Offset: 0x00003969
	public IEnumerator PlayAnimation()
	{
		this.Text.transform.localPosition = this.TextTarget.min;
		yield return this.AnimateText();
		yield return ShhhBehaviour.WaitWithInterrupt(this.HoldDuration);
		yield break;
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x0002321C File Offset: 0x0002141C
	public void Update()
	{
		this.vec.Set(0f, 0f, this.RotateRange.Lerp(Mathf.PerlinNoise(1f, Time.time * 8f)));
		this.LeftPlayer.transform.eulerAngles = this.vec;
		this.vec.Set(0f, 0f, this.RotateRange.Lerp(Mathf.PerlinNoise(2f, Time.time * 8f)));
		this.RightPlayer.transform.eulerAngles = this.vec;
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x00005778 File Offset: 0x00003978
	private IEnumerator AnimateText()
	{
		for (float t = 0f; t < this.Delay; t += Time.deltaTime)
		{
			yield return null;
		}
		Vector3 vec = default(Vector3);
		for (float t = 0f; t < this.TextDuration; t += Time.deltaTime)
		{
			float time = t / this.TextDuration;
			this.UpdateText(ref vec, this.TextEasing.Evaluate(time));
			yield return null;
		}
		this.UpdateText(ref vec, 1f);
		yield break;
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x00005787 File Offset: 0x00003987
	private void UpdateText(ref Vector3 vec, float p)
	{
		this.TextTarget.LerpUnclamped(ref vec, p, -7f);
		this.Text.transform.localPosition = vec;
	}

	// Token: 0x0400053E RID: 1342
	public SpriteRenderer LeftPlayer;

	// Token: 0x0400053F RID: 1343
	public SpriteRenderer RightPlayer;

	// Token: 0x04000540 RID: 1344
	public SpriteRenderer Text;

	// Token: 0x04000541 RID: 1345
	public FloatRange RotateRange = new FloatRange(-5f, 5f);

	// Token: 0x04000542 RID: 1346
	public Vector2Range TextTarget;

	// Token: 0x04000543 RID: 1347
	public AnimationCurve TextEasing;

	// Token: 0x04000544 RID: 1348
	public float Delay = 0.1f;

	// Token: 0x04000545 RID: 1349
	public float TextDuration = 0.5f;

	// Token: 0x04000546 RID: 1350
	public float HoldDuration = 2f;

	// Token: 0x04000547 RID: 1351
	private Vector3 vec;
}
