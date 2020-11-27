using System;
using UnityEngine;

// Token: 0x020000A4 RID: 164
public class DummyConsole : MonoBehaviour
{
	// Token: 0x06000374 RID: 884 RVA: 0x000043F7 File Offset: 0x000025F7
	public void Start()
	{
		this.rend = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06000375 RID: 885 RVA: 0x00017D20 File Offset: 0x00015F20
	public void FixedUpdate()
	{
		this.rend.material.SetColor("_OutlineColor", Color.yellow);
		float num = float.MaxValue;
		for (int i = 0; i < this.Players.Length; i++)
		{
			PlayerAnimator playerAnimator = this.Players[i];
			Vector2 vector = base.transform.position - playerAnimator.transform.position;
			vector.y += 0.3636f;
			float magnitude = vector.magnitude;
			if (magnitude < num)
			{
				num = magnitude;
			}
			if (magnitude < this.UseDistance)
			{
				playerAnimator.NearbyConsoles |= 1 << this.ConsoleId;
			}
			else
			{
				playerAnimator.NearbyConsoles &= ~(1 << this.ConsoleId);
			}
		}
		if (num >= this.UseDistance * 2f)
		{
			this.rend.material.SetFloat("_Outline", 0f);
			this.rend.material.SetColor("_AddColor", Color.clear);
			return;
		}
		this.rend.material.SetFloat("_Outline", 1f);
		if (num < this.UseDistance)
		{
			this.rend.material.SetColor("_AddColor", Color.yellow);
			return;
		}
		this.rend.material.SetColor("_AddColor", Color.clear);
	}

	// Token: 0x04000361 RID: 865
	public int ConsoleId;

	// Token: 0x04000362 RID: 866
	public PlayerAnimator[] Players;

	// Token: 0x04000363 RID: 867
	public float UseDistance;

	// Token: 0x04000364 RID: 868
	[HideInInspector]
	private SpriteRenderer rend;
}
