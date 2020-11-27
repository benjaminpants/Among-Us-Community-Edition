using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;

// Token: 0x020001A3 RID: 419
public class ReactorSystemType : ISystemType, IActivatable
{
	// Token: 0x17000150 RID: 336
	// (get) Token: 0x060008DF RID: 2271 RVA: 0x0002FDB0 File Offset: 0x0002DFB0
	public int UserCount
	{
		get
		{
			int num = 0;
			int num2 = 0;
			foreach (Tuple<byte, byte> tuple in this.UserConsolePairs)
			{
				int num3 = 1 << (int)tuple.Item2;
				if ((num3 & num2) == 0)
				{
					num++;
					num2 |= num3;
				}
			}
			return num;
		}
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x0002FE20 File Offset: 0x0002E020
	public bool GetConsoleComplete(int consoleId)
	{
		return this.UserConsolePairs.Any((Tuple<byte, byte> kvp) => (int)kvp.Item2 == consoleId);
	}

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x060008E1 RID: 2273 RVA: 0x000076CE File Offset: 0x000058CE
	public bool IsActive
	{
		get
		{
			return this.Countdown < 10000f;
		}
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x0002FE54 File Offset: 0x0002E054
	public void RepairDamage(PlayerControl player, byte opCode)
	{
		int num = (int)(opCode & 3);
		if (opCode == 128 && !this.IsActive)
		{
			this.Countdown = 30f;
			this.UserConsolePairs.Clear();
			return;
		}
		if (opCode == 16)
		{
			this.Countdown = 10000f;
			return;
		}
		if (opCode.HasAnyBit(64))
		{
			this.UserConsolePairs.Add(new Tuple<byte, byte>(player.PlayerId, (byte)num));
			if (this.UserCount >= 2)
			{
				this.Countdown = 10000f;
				return;
			}
		}
		else if (opCode.HasAnyBit(32))
		{
			this.UserConsolePairs.Remove(new Tuple<byte, byte>(player.PlayerId, (byte)num));
		}
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x0002FEF8 File Offset: 0x0002E0F8
	public bool Detoriorate(float deltaTime)
	{
		if (this.IsActive)
		{
			if (DestroyableSingleton<HudManager>.Instance.ReactorFlash == null)
			{
				PlayerControl.LocalPlayer.AddSystemTask(SystemTypes.Reactor);
			}
			this.Countdown -= deltaTime;
			this.timer += deltaTime;
			if (this.timer > 2f)
			{
				this.timer = 0f;
				return true;
			}
		}
		else if (DestroyableSingleton<HudManager>.Instance.ReactorFlash != null)
		{
			((ReactorShipRoom)ShipStatus.Instance.AllRooms.First((ShipRoom r) => r.RoomId == SystemTypes.Reactor)).StopMeltdown();
		}
		return false;
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x0002FFA0 File Offset: 0x0002E1A0
	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.Write(this.Countdown);
		writer.WritePacked(this.UserConsolePairs.Count);
		foreach (Tuple<byte, byte> tuple in this.UserConsolePairs)
		{
			writer.Write(tuple.Item1);
			writer.Write(tuple.Item2);
		}
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x00030024 File Offset: 0x0002E224
	public void Deserialize(MessageReader reader, bool initialState)
	{
		this.Countdown = reader.ReadSingle();
		this.UserConsolePairs.Clear();
		int num = reader.ReadPackedInt32();
		for (int i = 0; i < num; i++)
		{
			this.UserConsolePairs.Add(new Tuple<byte, byte>(reader.ReadByte(), reader.ReadByte()));
		}
	}

	// Token: 0x0400089C RID: 2204
	private const float SyncRate = 2f;

	// Token: 0x0400089D RID: 2205
	private float timer;

	// Token: 0x0400089E RID: 2206
	public const byte StartCountdown = 128;

	// Token: 0x0400089F RID: 2207
	public const byte AddUserOp = 64;

	// Token: 0x040008A0 RID: 2208
	public const byte RemoveUserOp = 32;

	// Token: 0x040008A1 RID: 2209
	public const byte ClearCountdown = 16;

	// Token: 0x040008A2 RID: 2210
	public const float CountdownStopped = 10000f;

	// Token: 0x040008A3 RID: 2211
	public const float ReactorDuration = 30f;

	// Token: 0x040008A4 RID: 2212
	public const byte ConsoleIdMask = 3;

	// Token: 0x040008A5 RID: 2213
	public const byte RequiredUserCount = 2;

	// Token: 0x040008A6 RID: 2214
	public float Countdown = 10000f;

	// Token: 0x040008A7 RID: 2215
	private HashSet<Tuple<byte, byte>> UserConsolePairs = new HashSet<Tuple<byte, byte>>();
}
