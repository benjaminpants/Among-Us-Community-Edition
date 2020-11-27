using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200011A RID: 282
public class CrystalBehaviour : MonoBehaviour
{
	// Token: 0x060005F7 RID: 1527 RVA: 0x00024E00 File Offset: 0x00023000
	private void Update()
	{
		Vector3 position = base.transform.position;
		Vector3 vector;
		if (this.ParentSide == CrystalBehaviour.Parentness.Above)
		{
			if (!this.Above)
			{
				return;
			}
			vector = this.Above.transform.position - new Vector3(0f, this.Above.Padding.min + this.Padding.max, 0f);
		}
		else
		{
			if (this.ParentSide != CrystalBehaviour.Parentness.Below)
			{
				return;
			}
			if (!this.Below)
			{
				return;
			}
			vector = this.Below.transform.position + new Vector3(0f, this.Below.Padding.max + this.Padding.min, 0f);
		}
		float num = Time.time / 0.35f;
		vector.x += (Mathf.PerlinNoise(num, position.z * 20f) * 2f - 1f) * 0.05f;
		vector.y += (Mathf.PerlinNoise(position.z * 20f, num) * 2f - 1f) * 0.05f;
		vector.z = position.z;
		position.x = Mathf.SmoothStep(position.x, vector.x, Time.deltaTime * 15f);
		position.y = Mathf.SmoothStep(position.y, vector.y, Time.deltaTime * 15f);
		base.transform.position = position;
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x00005C30 File Offset: 0x00003E30
	public void FlashUp(float delay = 0f)
	{
		base.StopAllCoroutines();
		base.StartCoroutine(CrystalBehaviour.Flash(this, delay));
		if (this.Above)
		{
			this.Above.FlashUp(delay + 0.1f);
		}
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x00005C65 File Offset: 0x00003E65
	public void FlashDown(float delay = 0f)
	{
		base.StopAllCoroutines();
		base.StartCoroutine(CrystalBehaviour.Flash(this, delay));
		if (this.Below)
		{
			this.Below.FlashDown(delay + 0.1f);
		}
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x00005C9A File Offset: 0x00003E9A
	private static IEnumerator Flash(CrystalBehaviour c, float delay)
	{
		for (float time = 0f; time < delay; time += Time.deltaTime)
		{
			yield return null;
		}
		Color col = Color.clear;
		for (float time = 0f; time < 0.1f; time += Time.deltaTime)
		{
			float t = time / 0.1f;
			col.r = (col.g = (col.b = Mathf.Lerp(0f, 1f, t)));
			c.Renderer.material.SetColor("_AddColor", col);
			yield return null;
		}
		for (float time = 0f; time < 0.1f; time += Time.deltaTime)
		{
			float t2 = time / 0.1f;
			col.r = (col.g = (col.b = Mathf.Lerp(1f, 0f, t2)));
			c.Renderer.material.SetColor("_AddColor", col);
			yield return null;
		}
		col.r = (col.g = (col.b = 0f));
		c.Renderer.material.SetColor("_AddColor", col);
		yield break;
	}

	// Token: 0x040005D5 RID: 1493
	public CrystalBehaviour Above;

	// Token: 0x040005D6 RID: 1494
	public CrystalBehaviour Below;

	// Token: 0x040005D7 RID: 1495
	public CrystalBehaviour.Parentness ParentSide;

	// Token: 0x040005D8 RID: 1496
	public SpriteRenderer Renderer;

	// Token: 0x040005D9 RID: 1497
	public BoxCollider2D Collider;

	// Token: 0x040005DA RID: 1498
	public FloatRange Padding;

	// Token: 0x040005DB RID: 1499
	private const float Speed = 15f;

	// Token: 0x040005DC RID: 1500
	private const float FloatMag = 0.05f;

	// Token: 0x040005DD RID: 1501
	private const float FloatSpeed = 0.35f;

	// Token: 0x0200011B RID: 283
	public enum Parentness
	{
		// Token: 0x040005DF RID: 1503
		None,
		// Token: 0x040005E0 RID: 1504
		Above,
		// Token: 0x040005E1 RID: 1505
		Below
	}
}
