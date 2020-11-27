using System;
using Hazel;

// Token: 0x02000082 RID: 130
public class SwitchSystem : ISystemType, IActivatable
{
	// Token: 0x17000079 RID: 121
	// (get) Token: 0x060002BC RID: 700 RVA: 0x00003C53 File Offset: 0x00001E53
	public float Level
	{
		get
		{
			return (float)this.Value / 255f;
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x060002BD RID: 701 RVA: 0x00003C62 File Offset: 0x00001E62
	public bool IsActive
	{
		get
		{
			return this.ExpectedSwitches != this.ActualSwitches;
		}
	}

	// Token: 0x060002BE RID: 702 RVA: 0x00015588 File Offset: 0x00013788
	public SwitchSystem()
	{
		Random random = new Random();
		this.ExpectedSwitches = (byte)(random.Next() & 31);
		this.ActualSwitches = this.ExpectedSwitches;
	}

	// Token: 0x060002BF RID: 703 RVA: 0x000155D4 File Offset: 0x000137D4
	public bool Detoriorate(float deltaTime)
	{
		this.timer += deltaTime;
		if (this.timer >= this.DetoriorationTime)
		{
			this.timer = 0f;
			if (this.ExpectedSwitches != this.ActualSwitches)
			{
				if (this.Value > 0)
				{
					this.Value = (byte)Math.Max((int)(this.Value - 3), 0);
				}
				if (!SwitchSystem.HasTask<ElectricTask>())
				{
					PlayerControl.LocalPlayer.AddSystemTask(SystemTypes.Electrical);
				}
			}
			else if (this.Value < 255)
			{
				this.Value = (byte)Math.Min((int)(this.Value + 3), 255);
			}
		}
		return false;
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x00003C75 File Offset: 0x00001E75
	public void RepairDamage(PlayerControl player, byte amount)
	{
		if (amount.HasBit(128))
		{
			this.ActualSwitches ^= (byte)(amount & 31);
			return;
		}
		this.ActualSwitches ^= (byte)(1 << (int)amount);
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x00003CAD File Offset: 0x00001EAD
	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.Write(this.ExpectedSwitches);
		writer.Write(this.ActualSwitches);
		writer.Write(this.Value);
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x00003CD3 File Offset: 0x00001ED3
	public void Deserialize(MessageReader reader, bool initialState)
	{
		this.ExpectedSwitches = reader.ReadByte();
		this.ActualSwitches = reader.ReadByte();
		this.Value = reader.ReadByte();
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x00015670 File Offset: 0x00013870
	protected static bool HasTask<T>()
	{
		for (int i = PlayerControl.LocalPlayer.myTasks.Count - 1; i > 0; i--)
		{
			if (PlayerControl.LocalPlayer.myTasks[i] is T)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040002B5 RID: 693
	public const byte MaxValue = 255;

	// Token: 0x040002B6 RID: 694
	public const int NumSwitches = 5;

	// Token: 0x040002B7 RID: 695
	public const byte DamageSystem = 128;

	// Token: 0x040002B8 RID: 696
	public const byte SwitchesMask = 31;

	// Token: 0x040002B9 RID: 697
	public float DetoriorationTime = 0.03f;

	// Token: 0x040002BA RID: 698
	public byte Value = byte.MaxValue;

	// Token: 0x040002BB RID: 699
	private float timer;

	// Token: 0x040002BC RID: 700
	public byte ExpectedSwitches;

	// Token: 0x040002BD RID: 701
	public byte ActualSwitches;
}
