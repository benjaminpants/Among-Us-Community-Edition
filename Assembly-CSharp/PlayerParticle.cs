using System;
using UnityEngine;

// Token: 0x02000155 RID: 341
public class PlayerParticle : PoolableBehavior
{
	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000711 RID: 1809 RVA: 0x00006633 File Offset: 0x00004833
	// (set) Token: 0x06000712 RID: 1810 RVA: 0x0000663B File Offset: 0x0000483B
	public Camera FollowCamera { get; set; }

	// Token: 0x06000713 RID: 1811 RVA: 0x00028E2C File Offset: 0x0002702C
	public void Update()
	{
		Vector3 vector = base.transform.localPosition;
		float sqrMagnitude = vector.sqrMagnitude;
		if (this.FollowCamera)
		{
			Vector3 position = this.FollowCamera.transform.position;
			position.z = 0f;
			vector += (position - this.lastCamera) * (1f - base.transform.localScale.x);
			this.lastCamera = position;
			sqrMagnitude = (vector - position).sqrMagnitude;
		}
		if (sqrMagnitude > this.maxDistance * this.maxDistance)
		{
			this.OwnerPool.Reclaim(this);
			return;
		}
		vector += (Vector3)(this.velocity * Time.deltaTime);
		base.transform.localPosition = vector;
		base.transform.Rotate(0f, 0f, Time.deltaTime * this.angularVelocity);
	}

	// Token: 0x040006D0 RID: 1744
	public SpriteRenderer myRend;

	// Token: 0x040006D1 RID: 1745
	public float maxDistance = 6f;

	// Token: 0x040006D2 RID: 1746
	public Vector2 velocity;

	// Token: 0x040006D3 RID: 1747
	public float angularVelocity;

	// Token: 0x040006D5 RID: 1749
	private Vector3 lastCamera;
}
