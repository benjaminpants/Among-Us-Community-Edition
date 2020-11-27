using System;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public class TumbleBoxBehaviour : MonoBehaviour
{
	// Token: 0x06000ACC RID: 2764 RVA: 0x00036B50 File Offset: 0x00034D50
	public void FixedUpdate()
	{
		float z = Time.time * 15f;
		float v = Mathf.Cos(Time.time * 3.14159274f / 10f) / 2f + 0.5f;
		float num = this.shadowScale.Lerp(v);
		this.Shadow.transform.localScale = new Vector3(num, num, num);
		float y = this.BoxHeight.Lerp(v);
		this.Box.transform.localPosition = new Vector3(0f, y, -0.01f);
		this.Box.transform.eulerAngles = new Vector3(0f, 0f, z);
	}

	// Token: 0x04000A7F RID: 2687
	public FloatRange BoxHeight;

	// Token: 0x04000A80 RID: 2688
	public FloatRange shadowScale;

	// Token: 0x04000A81 RID: 2689
	public SpriteRenderer Shadow;

	// Token: 0x04000A82 RID: 2690
	public SpriteRenderer Box;
}
