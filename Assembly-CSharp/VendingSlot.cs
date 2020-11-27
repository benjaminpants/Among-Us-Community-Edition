using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000A2 RID: 162
public class VendingSlot : MonoBehaviour
{
	// Token: 0x0600036A RID: 874 RVA: 0x000043D1 File Offset: 0x000025D1
	public IEnumerator CoBuy()
	{
		yield return new WaitForLerp(0.75f, delegate(float v)
		{
			this.GlassImage.size = new Vector2(1f, Mathf.Lerp(1.7f, 0f, v));
			this.GlassImage.transform.localPosition = new Vector3(0f, Mathf.Lerp(0f, 0.85f, v), -1f);
		});
		yield return Effects.Shake(this.DrinkImage.transform, 0.75f, 0.075f);
		Vector3 localPosition = this.DrinkImage.transform.localPosition;
		localPosition.z = -5f;
		this.DrinkImage.transform.localPosition = localPosition;
		Vector3 v2 = localPosition;
		v2.y = -8f - localPosition.y;
		yield return Effects.All(new IEnumerator[]
		{
			Effects.Slide2D(this.DrinkImage.transform, localPosition, v2, 0.75f),
			Effects.Rotate2D(this.DrinkImage.transform, 0f, -FloatRange.Next(-45f, 45f), 0.75f)
		});
		yield return new WaitForLerp(0.75f, delegate(float v)
		{
			this.GlassImage.size = new Vector2(1f, Mathf.Lerp(0f, 1.7f, v));
			this.GlassImage.transform.localPosition = new Vector3(0f, Mathf.Lerp(0.85f, 0f, v), -1f);
		});
		this.DrinkImage.enabled = false;
		yield break;
	}

	// Token: 0x0400035C RID: 860
	public SpriteRenderer DrinkImage;

	// Token: 0x0400035D RID: 861
	public SpriteRenderer GlassImage;
}
