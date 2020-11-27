using System;
using UnityEngine;

// Token: 0x020000B7 RID: 183
public class AspectSize : MonoBehaviour
{
	// Token: 0x060003D5 RID: 981 RVA: 0x00018C74 File Offset: 0x00016E74
	public void OnEnable()
	{
		Camera main = Camera.main;
		float num = main.orthographicSize * main.aspect;
		float num2 = (this.Background ? this.Background : this.Renderer.sprite).bounds.size.x / 2f;
		float num3 = num / num2 * this.PercentWidth;
		if (num3 < 1f)
		{
			base.transform.localScale = new Vector3(num3, num3, num3);
		}
	}

	// Token: 0x040003C4 RID: 964
	public Sprite Background;

	// Token: 0x040003C5 RID: 965
	public SpriteRenderer Renderer;

	// Token: 0x040003C6 RID: 966
	public float PercentWidth = 0.95f;
}
