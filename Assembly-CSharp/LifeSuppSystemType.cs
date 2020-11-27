using System;
using System.Collections.Generic;
using Hazel;

// Token: 0x020001A1 RID: 417
public class LifeSuppSystemType : ISystemType, IActivatable
{
	// Token: 0x1700014E RID: 334
	// (get) Token: 0x060008D2 RID: 2258 RVA: 0x0000764D File Offset: 0x0000584D
	public int UserCount
	{
		get
		{
			return this.CompletedConsoles.Count;
		}
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x0000765A File Offset: 0x0000585A
	public bool GetConsoleComplete(int consoleId)
	{
		return this.CompletedConsoles.Contains(consoleId);
	}

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x060008D4 RID: 2260 RVA: 0x00007668 File Offset: 0x00005868
	public bool IsActive
	{
		get
		{
			return this.Countdown < 10000f;
		}
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x0002F988 File Offset: 0x0002DB88
	public void RepairDamage(PlayerControl player, byte opCode)
	{
		int item = (int)(opCode & 3);
		if (opCode == 128 && !this.IsActive)
		{
			this.Countdown = 45f;
			this.CompletedConsoles.Clear();
			return;
		}
		if (opCode == 16)
		{
			this.Countdown = 10000f;
			return;
		}
		if (opCode.HasAnyBit(64))
		{
			this.CompletedConsoles.Add(item);
		}
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x0002F9E8 File Offset: 0x0002DBE8
	public bool Detoriorate(float deltaTime)
	{
		if (this.IsActive)
		{
			if (DestroyableSingleton<HudManager>.Instance.OxyFlash == null)
			{
				PlayerControl.LocalPlayer.AddSystemTask(SystemTypes.LifeSupp);
			}
			this.Countdown -= deltaTime;
			if (this.UserCount >= 2)
			{
				this.Countdown = 10000f;
				return true;
			}
			this.timer += deltaTime;
			if (this.timer > 2f)
			{
				this.timer = 0f;
				return true;
			}
		}
		else if (DestroyableSingleton<HudManager>.Instance.OxyFlash != null)
		{
			DestroyableSingleton<HudManager>.Instance.StopOxyFlash();
		}
		return false;
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x0002FA78 File Offset: 0x0002DC78
	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.Write(this.Countdown);
		writer.WritePacked(this.CompletedConsoles.Count);
		foreach (int value in this.CompletedConsoles)
		{
			writer.WritePacked(value);
		}
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x0002FAE8 File Offset: 0x0002DCE8
	public void Deserialize(MessageReader reader, bool initialState)
	{
		this.Countdown = reader.ReadSingle();
		if (reader.Position < reader.Length)
		{
			this.CompletedConsoles.Clear();
			int num = reader.ReadPackedInt32();
			for (int i = 0; i < num; i++)
			{
				this.CompletedConsoles.Add(reader.ReadPackedInt32());
			}
		}
	}

	// Token: 0x04000888 RID: 2184
	private const float SyncRate = 2f;

	// Token: 0x04000889 RID: 2185
	private float timer;

	// Token: 0x0400088A RID: 2186
	public const byte StartCountdown = 128;

	// Token: 0x0400088B RID: 2187
	public const byte AddUserOp = 64;

	// Token: 0x0400088C RID: 2188
	public const byte ClearCountdown = 16;

	// Token: 0x0400088D RID: 2189
	public const float CountdownStopped = 10000f;

	// Token: 0x0400088E RID: 2190
	public const float LifeSuppDuration = 45f;

	// Token: 0x0400088F RID: 2191
	public const byte ConsoleIdMask = 3;

	// Token: 0x04000890 RID: 2192
	public const byte RequiredUserCount = 2;

	// Token: 0x04000891 RID: 2193
	public float Countdown = 10000f;

	// Token: 0x04000892 RID: 2194
	private HashSet<int> CompletedConsoles = new HashSet<int>();
}
