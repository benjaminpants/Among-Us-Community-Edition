using System;

// Token: 0x020001CE RID: 462
public class AutoOpenDoor : ManualDoor
{
	// Token: 0x06000A03 RID: 2563 RVA: 0x00034800 File Offset: 0x00032A00
	public bool DoUpdate(float dt)
	{
		this.CooldownTimer = Math.Max(this.CooldownTimer - dt, 0f);
		if (this.ClosedTimer > 0f)
		{
			this.ClosedTimer = Math.Max(this.ClosedTimer - dt, 0f);
			if (this.ClosedTimer == 0f)
			{
				this.SetDoorway(true);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x000081CA File Offset: 0x000063CA
	public override void SetDoorway(bool open)
	{
		if (!open)
		{
			this.ClosedTimer = 10f;
			this.CooldownTimer = 30f;
		}
		base.SetDoorway(open);
	}

	// Token: 0x040009AA RID: 2474
	private const float ClosedDuration = 10f;

	// Token: 0x040009AB RID: 2475
	public SystemTypes Room;

	// Token: 0x040009AC RID: 2476
	public float ClosedTimer;

	// Token: 0x040009AD RID: 2477
	public float CooldownTimer;
}
