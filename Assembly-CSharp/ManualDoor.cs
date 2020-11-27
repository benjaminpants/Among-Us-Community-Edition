using System;
using Hazel;
using PowerTools;
using UnityEngine;

// Token: 0x020001D1 RID: 465
public class ManualDoor : MonoBehaviour
{
	// Token: 0x06000A13 RID: 2579 RVA: 0x00034A24 File Offset: 0x00032C24
	public virtual void SetDoorway(bool open)
	{
		if (this.Open == open)
		{
			return;
		}
		if (this.animator)
		{
			this.animator.Play(open ? this.OpenDoorAnim : this.CloseDoorAnim, 1f);
		}
		this.Open = open;
		this.myCollider.isTrigger = open;
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x00008267 File Offset: 0x00006467
	public virtual void Serialize(MessageWriter writer)
	{
		writer.Write(this.Open);
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x00008275 File Offset: 0x00006475
	public virtual void Deserialize(MessageReader reader)
	{
		this.SetDoorway(reader.ReadBoolean());
	}

	// Token: 0x040009B2 RID: 2482
	private const float ClosedDuration = 10f;

	// Token: 0x040009B3 RID: 2483
	public const float CooldownDuration = 30f;

	// Token: 0x040009B4 RID: 2484
	public bool Open;

	// Token: 0x040009B5 RID: 2485
	public Collider2D myCollider;

	// Token: 0x040009B6 RID: 2486
	public SpriteAnim animator;

	// Token: 0x040009B7 RID: 2487
	public AnimationClip OpenDoorAnim;

	// Token: 0x040009B8 RID: 2488
	public AnimationClip CloseDoorAnim;
}
