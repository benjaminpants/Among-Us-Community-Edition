using System;
using UnityEngine;

// Token: 0x02000124 RID: 292
public class LeafBehaviour : MonoBehaviour
{
	// Token: 0x0600061F RID: 1567 RVA: 0x00025B78 File Offset: 0x00023D78
	public void Start()
	{
		LeafBehaviour.ImageFiller.Set(this.Images);
		base.GetComponent<SpriteRenderer>().sprite = LeafBehaviour.ImageFiller.Get();
		this.body = base.GetComponent<Rigidbody2D>();
		this.body.angularVelocity = this.SpinSpeed.Next();
		this.body.velocity = this.StartVel.Next();
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x00005D91 File Offset: 0x00003F91
	public void FixedUpdate()
	{
		if (!this.Held && (double)base.transform.localPosition.x < -2.5)
		{
			this.Parent.LeafDone(this);
		}
	}

	// Token: 0x04000608 RID: 1544
	public Sprite[] Images;

	// Token: 0x04000609 RID: 1545
	public FloatRange SpinSpeed = new FloatRange(-45f, 45f);

	// Token: 0x0400060A RID: 1546
	public Vector2Range StartVel;

	// Token: 0x0400060B RID: 1547
	public float AccelRate = 30f;

	// Token: 0x0400060C RID: 1548
	[HideInInspector]
	public LeafMinigame Parent;

	// Token: 0x0400060D RID: 1549
	public bool Held;

	// Token: 0x0400060E RID: 1550
	private static RandomFill<Sprite> ImageFiller = new RandomFill<Sprite>();

	// Token: 0x0400060F RID: 1551
	[HideInInspector]
	public Rigidbody2D body;
}
