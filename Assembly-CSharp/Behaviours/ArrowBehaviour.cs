using System;
using UnityEngine;

// Token: 0x02000229 RID: 553
public class ArrowBehaviour : MonoBehaviour
{
	// Token: 0x06000BD7 RID: 3031 RVA: 0x000091C3 File Offset: 0x000073C3
	public void Start()
	{
		this.image = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06000BD8 RID: 3032 RVA: 0x0003A304 File Offset: 0x00038504
	public void Update()
	{
		Camera main = Camera.main;
		Vector2 vector = this.target - main.transform.position;
		float num = vector.magnitude / (main.orthographicSize * this.perc);
		this.image.enabled = ((double)num > 0.3);
		Vector2 vector2 = main.WorldToViewportPoint(this.target);
		if (this.Between(vector2.x, 0f, 1f) && this.Between(vector2.y, 0f, 1f))
		{
			base.transform.position = (Vector2)this.target - vector.normalized * 0.6f;
			float num2 = Mathf.Clamp(num, 0f, 1f);
			base.transform.localScale = new Vector3(num2, num2, num2);
		}
		else
		{
			Vector2 vector3 = new Vector2(Mathf.Clamp(vector2.x * 2f - 1f, -1f, 1f), Mathf.Clamp(vector2.y * 2f - 1f, -1f, 1f));
			float orthographicSize = main.orthographicSize;
			float num3 = main.orthographicSize * main.aspect;
			Vector3 b = new Vector3(Mathf.LerpUnclamped(0f, num3 * 0.88f, vector3.x), Mathf.LerpUnclamped(0f, orthographicSize * 0.79f, vector3.y), 0f);
			base.transform.position = main.transform.position + b;
			base.transform.localScale = Vector3.one;
		}
		base.transform.LookAt2d(this.target);
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x000091D1 File Offset: 0x000073D1
	private bool Between(float value, float min, float max)
	{
		return value > min && value < max;
	}

	// Token: 0x04000B6E RID: 2926
	public Vector3 target;

	// Token: 0x04000B6F RID: 2927
	public float perc = 0.925f;

	// Token: 0x04000B70 RID: 2928
	private SpriteRenderer image;
}
