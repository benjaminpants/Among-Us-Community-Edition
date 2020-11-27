using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000233 RID: 563
public class Asteroid : PoolableBehavior
{
	// Token: 0x170001CF RID: 463
	// (get) Token: 0x06000C04 RID: 3076 RVA: 0x00009368 File Offset: 0x00007568
	// (set) Token: 0x06000C05 RID: 3077 RVA: 0x00009370 File Offset: 0x00007570
	public Vector3 TargetPosition { get; internal set; }

	// Token: 0x06000C06 RID: 3078 RVA: 0x0003AE6C File Offset: 0x0003906C
	public void FixedUpdate()
	{
		base.transform.localRotation = Quaternion.Euler(0f, 0f, base.transform.localRotation.eulerAngles.z + this.RotateSpeed.Last * Time.fixedDeltaTime);
		Vector3 a = this.TargetPosition - base.transform.localPosition;
		if (a.sqrMagnitude > 0.05f)
		{
			a.Normalize();
			base.transform.localPosition += a * this.MoveSpeed.Last * Time.fixedDeltaTime;
			return;
		}
		this.OwnerPool.Reclaim(this);
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x0003AF28 File Offset: 0x00039128
	public override void Reset()
	{
		base.enabled = true;
		this.Explosion.enabled = false;
		SpriteRenderer component = base.GetComponent<SpriteRenderer>();
		this.imgIdx = this.AsteroidImages.RandomIdx<Sprite>();
		component.sprite = this.AsteroidImages[this.imgIdx];
		component.enabled = true;
		ButtonBehavior component2 = base.GetComponent<ButtonBehavior>();
		component2.enabled = true;
		component2.OnClick.RemoveAllListeners();
		base.transform.Rotate(0f, 0f, this.RotateSpeed.Next());
		this.MoveSpeed.Next();
		base.Reset();
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x00009379 File Offset: 0x00007579
	public IEnumerator CoBreakApart()
	{
		base.enabled = false;
		base.GetComponent<ButtonBehavior>().enabled = false;
		this.Explosion.enabled = true;
		yield return new WaitForLerp(0.1f, delegate(float t)
		{
			this.Explosion.transform.localScale = new Vector3(t, t, t);
		});
		yield return new WaitForSeconds(0.05f);
		yield return new WaitForLerp(0.05f, delegate(float t)
		{
			this.Explosion.transform.localScale = new Vector3(1f - t, 1f - t, 1f - t);
		});
		SpriteRenderer rend = base.GetComponent<SpriteRenderer>();
		yield return null;
		rend.sprite = this.BrokenImages[this.imgIdx];
		yield return new WaitForSeconds(0.2f);
		this.OwnerPool.Reclaim(this);
		yield break;
	}

	// Token: 0x04000B94 RID: 2964
	public Sprite[] AsteroidImages;

	// Token: 0x04000B95 RID: 2965
	public Sprite[] BrokenImages;

	// Token: 0x04000B96 RID: 2966
	private int imgIdx;

	// Token: 0x04000B97 RID: 2967
	public FloatRange MoveSpeed = new FloatRange(2f, 5f);

	// Token: 0x04000B98 RID: 2968
	public FloatRange RotateSpeed = new FloatRange(-10f, 10f);

	// Token: 0x04000B9A RID: 2970
	public SpriteRenderer Explosion;
}
