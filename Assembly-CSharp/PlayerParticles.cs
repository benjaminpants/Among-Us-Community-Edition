using System;
using UnityEngine;

// Token: 0x02000157 RID: 343
public class PlayerParticles : MonoBehaviour
{
	// Token: 0x06000716 RID: 1814 RVA: 0x00028F28 File Offset: 0x00027128
	public void Start()
	{
		this.fill = new RandomFill<PlayerParticleInfo>();
		this.fill.Set(this.Sprites);
		int num = 0;
		while (this.pool.NotInUse > 0)
		{
			PlayerParticle playerParticle = this.pool.Get<PlayerParticle>();
			PlayerControl.SetPlayerMaterialColors(num++, playerParticle.myRend);
			this.PlacePlayer(playerParticle, true);
		}
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x00028F88 File Offset: 0x00027188
	public void Update()
	{
		while (this.pool.NotInUse > 0)
		{
			PlayerParticle part = this.pool.Get<PlayerParticle>();
			this.PlacePlayer(part, false);
		}
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x00028FBC File Offset: 0x000271BC
	private void PlacePlayer(PlayerParticle part, bool initial)
	{
		Vector3 vector = UnityEngine.Random.insideUnitCircle;
		if (!initial)
		{
			vector.Normalize();
		}
		vector *= this.StartRadius;
		float num = this.scale.Next();
		part.transform.localScale = new Vector3(num, num, num);
		vector.z = -num * 0.001f;
		if (this.FollowCamera)
		{
			Vector3 position = this.FollowCamera.transform.position;
			position.z = 0f;
			vector += position;
			part.FollowCamera = this.FollowCamera;
		}
		part.transform.localPosition = vector;
		PlayerParticleInfo playerParticleInfo = this.fill.Get();
		part.myRend.sprite = playerParticleInfo.image;
		part.myRend.flipX = BoolRange.Next(0.5f);
		Vector2 vector2 = -vector.normalized;
		vector2 = vector2.Rotate(FloatRange.Next(-45f, 45f));
		part.velocity = vector2 * this.velocity.Next();
		part.angularVelocity = playerParticleInfo.angularVel.Next();
		if (playerParticleInfo.alignToVel)
		{
			part.transform.localEulerAngles = new Vector3(0f, 0f, Vector2.up.AngleSigned(vector2));
		}
	}

	// Token: 0x040006D9 RID: 1753
	public PlayerParticleInfo[] Sprites;

	// Token: 0x040006DA RID: 1754
	public FloatRange velocity;

	// Token: 0x040006DB RID: 1755
	public FloatRange scale;

	// Token: 0x040006DC RID: 1756
	public ObjectPoolBehavior pool;

	// Token: 0x040006DD RID: 1757
	public float StartRadius;

	// Token: 0x040006DE RID: 1758
	public Camera FollowCamera;

	// Token: 0x040006DF RID: 1759
	private RandomFill<PlayerParticleInfo> fill;
}
