using System;
using UnityEngine;

// Token: 0x0200005B RID: 91
public class TextLink : MonoBehaviour
{
	// Token: 0x060001F2 RID: 498 RVA: 0x000111F4 File Offset: 0x0000F3F4
	public void Set(Vector2 from, Vector2 to, string target)
	{
		this.targetUrl = target;
		Vector2 vector = to + from;
		base.transform.localPosition = new Vector3(vector.x / 2f, vector.y / 2f, -1f);
		vector = to - from;
		vector.y = -vector.y;
		this.boxCollider.size = vector;
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x000033E9 File Offset: 0x000015E9
	public void Click()
	{
		Application.OpenURL(this.targetUrl);
	}

	// Token: 0x040001E0 RID: 480
	public BoxCollider2D boxCollider;

	// Token: 0x040001E1 RID: 481
	public string targetUrl;

	// Token: 0x040001E2 RID: 482
	public bool needed;
}
