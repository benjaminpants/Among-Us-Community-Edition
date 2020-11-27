using System;
using Hazel;
using UnityEngine;

// Token: 0x02000074 RID: 116
public class DeconSystem : MonoBehaviour, ISystemType
{
	// Token: 0x06000273 RID: 627 RVA: 0x00013DA4 File Offset: 0x00011FA4
	public bool Detoriorate(float dt)
	{
		int num = Mathf.CeilToInt(this.timer);
		this.timer = Mathf.Max(0f, this.timer - dt);
		int num2 = Mathf.CeilToInt(this.timer);
		if (num != num2)
		{
			if (num2 == 0)
			{
				if (this.curState.HasFlag(DeconSystem.States.Enter))
				{
					this.curState = ((this.curState & ~DeconSystem.States.Enter) | DeconSystem.States.Closed);
					this.timer = this.DeconTime;
				}
				else if (this.curState.HasFlag(DeconSystem.States.Closed))
				{
					this.curState = ((this.curState & ~DeconSystem.States.Closed) | DeconSystem.States.Exit);
					this.timer = this.DoorOpenTime;
				}
				else if (this.curState.HasFlag(DeconSystem.States.Exit))
				{
					this.curState = DeconSystem.States.Idle;
				}
			}
			this.UpdateDoorsViaState();
			return true;
		}
		return false;
	}

	// Token: 0x06000274 RID: 628 RVA: 0x00003851 File Offset: 0x00001A51
	public void OpenDoor(bool upper)
	{
		if (this.curState == DeconSystem.States.Idle)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Decontamination, upper ? 2 : 1);
		}
	}

	// Token: 0x06000275 RID: 629 RVA: 0x0000386E File Offset: 0x00001A6E
	public void OpenFromInside(bool upper)
	{
		if (this.curState == DeconSystem.States.Idle)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Decontamination, upper ? 3 : 4);
		}
	}

	// Token: 0x06000276 RID: 630 RVA: 0x00013E88 File Offset: 0x00012088
	public void RepairDamage(PlayerControl player, byte amount)
	{
		if (this.curState != DeconSystem.States.Idle)
		{
			return;
		}
		switch (amount)
		{
		case 1:
			this.curState = (DeconSystem.States.Enter | DeconSystem.States.HeadingUp);
			this.timer = this.DoorOpenTime;
			break;
		case 2:
			this.curState = DeconSystem.States.Enter;
			this.timer = this.DoorOpenTime;
			break;
		case 3:
			this.curState = (DeconSystem.States.Exit | DeconSystem.States.HeadingUp);
			this.timer = this.DoorOpenTime;
			break;
		case 4:
			this.curState = DeconSystem.States.Exit;
			this.timer = this.DoorOpenTime;
			break;
		}
		this.UpdateDoorsViaState();
	}

	// Token: 0x06000277 RID: 631 RVA: 0x0000388B File Offset: 0x00001A8B
	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.Write((byte)Mathf.CeilToInt(this.timer));
		writer.Write((byte)this.curState);
	}

	// Token: 0x06000278 RID: 632 RVA: 0x000038AB File Offset: 0x00001AAB
	public void Deserialize(MessageReader reader, bool initialState)
	{
		this.timer = (float)reader.ReadByte();
		this.curState = (DeconSystem.States)reader.ReadByte();
		this.UpdateDoorsViaState();
	}

	// Token: 0x06000279 RID: 633 RVA: 0x00013F14 File Offset: 0x00012114
	private void UpdateDoorsViaState()
	{
		int num = Mathf.CeilToInt(this.timer);
		if (num > 0)
		{
			this.FloorText.Text = string.Join("\n", new object[]
			{
				num,
				num,
				num,
				num
			});
		}
		else
		{
			this.FloorText.Text = string.Empty;
		}
		if (this.curState.HasFlag(DeconSystem.States.Enter))
		{
			bool flag = this.curState.HasFlag(DeconSystem.States.HeadingUp);
			this.LowerDoor.SetDoorway(flag);
			this.UpperDoor.SetDoorway(!flag);
			return;
		}
		if (this.curState.HasFlag(DeconSystem.States.Closed) || this.curState == DeconSystem.States.Idle)
		{
			this.LowerDoor.SetDoorway(false);
			this.UpperDoor.SetDoorway(false);
			return;
		}
		if (this.curState.HasFlag(DeconSystem.States.Exit))
		{
			bool flag2 = this.curState.HasFlag(DeconSystem.States.HeadingUp);
			this.LowerDoor.SetDoorway(!flag2);
			this.UpperDoor.SetDoorway(flag2);
		}
	}

	// Token: 0x04000268 RID: 616
	private const byte HeadUpCmd = 1;

	// Token: 0x04000269 RID: 617
	private const byte HeadDownCmd = 2;

	// Token: 0x0400026A RID: 618
	private const byte HeadUpInsideCmd = 3;

	// Token: 0x0400026B RID: 619
	private const byte HeadDownInsideCmd = 4;

	// Token: 0x0400026C RID: 620
	public ManualDoor UpperDoor;

	// Token: 0x0400026D RID: 621
	public ManualDoor LowerDoor;

	// Token: 0x0400026E RID: 622
	public float DoorOpenTime = 5f;

	// Token: 0x0400026F RID: 623
	public float DeconTime = 5f;

	// Token: 0x04000270 RID: 624
	private DeconSystem.States curState;

	// Token: 0x04000271 RID: 625
	private float timer;

	// Token: 0x04000272 RID: 626
	public TextRenderer FloorText;

	// Token: 0x02000075 RID: 117
	[Flags]
	private enum States : byte
	{
		// Token: 0x04000274 RID: 628
		Idle = 0,
		// Token: 0x04000275 RID: 629
		Enter = 1,
		// Token: 0x04000276 RID: 630
		Closed = 2,
		// Token: 0x04000277 RID: 631
		Exit = 4,
		// Token: 0x04000278 RID: 632
		HeadingUp = 8
	}
}
