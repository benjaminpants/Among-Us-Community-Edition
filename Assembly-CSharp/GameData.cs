using System;
using System.Collections.Generic;
using System.Linq;
using Assets.CoreScripts;
using Hazel;
using InnerNet;
using UnityEngine;

// Token: 0x02000091 RID: 145
public class GameData : InnerNetObject, IDisconnectHandler
{
	// Token: 0x17000083 RID: 131
	// (get) Token: 0x06000309 RID: 777 RVA: 0x00004020 File Offset: 0x00002220
	public int PlayerCount
	{
		get
		{
			return this.AllPlayers.Count;
		}
	}

	// Token: 0x0600030A RID: 778 RVA: 0x00016770 File Offset: 0x00014970
	public void Awake()
	{
		if (GameData.Instance && GameData.Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		GameData.Instance = this;
		if (AmongUsClient.Instance)
		{
			AmongUsClient.Instance.DisconnectHandlers.AddUnique(this);
		}
	}

	// Token: 0x0600030B RID: 779 RVA: 0x000167C4 File Offset: 0x000149C4
	public GameData.PlayerInfo GetHost()
	{
		ClientData host = AmongUsClient.Instance.GetHost();
		if (host != null && host.Character)
		{
			return host.Character.Data;
		}
		return null;
	}

	// Token: 0x0600030C RID: 780 RVA: 0x000167FC File Offset: 0x000149FC
	public sbyte GetAvailableId()
	{
		sbyte i;
		sbyte j;
		for (i = 0; i < 20; i = (sbyte)(j + 1))
		{
			if (!this.AllPlayers.Any((GameData.PlayerInfo p) => p.PlayerId == (byte)i))
			{
				return i;
			}
			j = i;
		}
		return -1;
	}

	// Token: 0x0600030D RID: 781 RVA: 0x00016854 File Offset: 0x00014A54
	public GameData.PlayerInfo GetPlayerById(byte id)
	{
		if (id == 255)
		{
			return null;
		}
		for (int i = 0; i < this.AllPlayers.Count; i++)
		{
			if (this.AllPlayers[i].PlayerId == id)
			{
				return this.AllPlayers[i];
			}
		}
		return null;
	}

	// Token: 0x0600030E RID: 782 RVA: 0x000168A4 File Offset: 0x00014AA4
	public void UpdateName(byte playerId, string name)
	{
		GameData.PlayerInfo playerById = this.GetPlayerById(playerId);
		if (playerById != null)
		{
			playerById.PlayerName = name;
		}
	}

	// Token: 0x0600030F RID: 783 RVA: 0x000168C4 File Offset: 0x00014AC4
	public void UpdateColor(byte playerId, byte color)
	{
		GameData.PlayerInfo playerById = this.GetPlayerById(playerId);
		if (playerById != null)
		{
			playerById.ColorId = color;
		}
	}

	// Token: 0x06000310 RID: 784 RVA: 0x000168E4 File Offset: 0x00014AE4
	public void UpdateHat(byte playerId, uint hat)
	{
		GameData.PlayerInfo playerById = this.GetPlayerById(playerId);
		if (playerById != null)
		{
			playerById.HatId = hat;
		}
	}

	// Token: 0x06000311 RID: 785 RVA: 0x00016904 File Offset: 0x00014B04
	public void UpdateSkin(byte playerId, uint skin)
	{
		GameData.PlayerInfo playerById = this.GetPlayerById(playerId);
		if (playerById != null)
		{
			playerById.SkinId = skin;
		}
	}

	// Token: 0x06000312 RID: 786 RVA: 0x00016924 File Offset: 0x00014B24
	public void AddPlayer(PlayerControl pc)
	{
		GameData.PlayerInfo item = new GameData.PlayerInfo(pc);
		this.AllPlayers.Add(item);
		base.SetDirtyBit(1U << (int)pc.PlayerId);
	}

	// Token: 0x06000313 RID: 787 RVA: 0x00016958 File Offset: 0x00014B58
	public bool RemovePlayer(byte playerId)
	{
		for (int i = 0; i < this.AllPlayers.Count; i++)
		{
			if (this.AllPlayers[i].PlayerId == playerId)
			{
				this.DirtyBits &= ~(1U << (int)playerId);
				this.AllPlayers.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000314 RID: 788 RVA: 0x000169B4 File Offset: 0x00014BB4
	public void RecomputeTaskCounts()
	{
		this.TotalTasks = 0;
		this.CompletedTasks = 0;
		for (int i = 0; i < this.AllPlayers.Count; i++)
		{
			GameData.PlayerInfo playerInfo = this.AllPlayers[i];
			if (!playerInfo.Disconnected && playerInfo.Tasks != null && playerInfo.Object && (PlayerControl.GameOptions.GhostsDoTasks || !playerInfo.IsDead) && !playerInfo.IsImpostor)
			{
				for (int j = 0; j < playerInfo.Tasks.Count; j++)
				{
					this.TotalTasks++;
					if (playerInfo.Tasks[j].Complete)
					{
						this.CompletedTasks++;
					}
				}
			}
		}
	}

	// Token: 0x06000315 RID: 789 RVA: 0x00016A78 File Offset: 0x00014C78
	public void TutOnlyRemoveTask(byte playerId, uint taskId)
	{
		GameData.PlayerInfo playerById = this.GetPlayerById(playerId);
		GameData.TaskInfo item = playerById.FindTaskById(taskId);
		playerById.Tasks.Remove(item);
		this.RecomputeTaskCounts();
	}

	// Token: 0x06000316 RID: 790 RVA: 0x0000402D File Offset: 0x0000222D
	public void TutOnlyAddTask(byte playerId, uint taskId)
	{
		this.GetPlayerById(playerId).Tasks.Add(new GameData.TaskInfo
		{
			Id = taskId
		});
		this.TotalTasks++;
	}

	// Token: 0x06000317 RID: 791 RVA: 0x00016AA8 File Offset: 0x00014CA8
	private void SetTasks(byte playerId, byte[] taskTypeIds)
	{
		GameData.PlayerInfo playerById = this.GetPlayerById(playerId);
		if (playerById == null)
		{
			Debug.Log("Could not set tasks for player id: " + playerId);
			return;
		}
		if (playerById.Disconnected)
		{
			return;
		}
		if (!playerById.Object)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Could not set tasks for player (",
				playerById.PlayerName,
				"): ",
				playerId
			}));
			return;
		}
		playerById.Tasks = new List<GameData.TaskInfo>(taskTypeIds.Length);
		for (int i = 0; i < taskTypeIds.Length; i++)
		{
			playerById.Tasks.Add(new GameData.TaskInfo());
			playerById.Tasks[i].Id = (uint)i;
		}
		playerById.Object.SetTasks(taskTypeIds);
		base.SetDirtyBit(1U << (int)playerById.PlayerId);
	}

	// Token: 0x06000318 RID: 792 RVA: 0x00016B7C File Offset: 0x00014D7C
	public void CompleteTask(PlayerControl pc, uint taskId)
	{
		GameData.TaskInfo taskInfo = this.GetPlayerById(pc.PlayerId).FindTaskById(taskId);
		if (taskInfo == null)
		{
			Debug.LogWarning("Couldn't find task: " + taskId);
			return;
		}
		if (!taskInfo.Complete)
		{
			taskInfo.Complete = true;
			this.CompletedTasks++;
			return;
		}
		Debug.LogWarning("Double complete task: " + taskId);
	}

	// Token: 0x06000319 RID: 793 RVA: 0x00016BE8 File Offset: 0x00014DE8
	public void HandleDisconnect(PlayerControl player, DisconnectReasons reason)
	{
		if (!player)
		{
			return;
		}
		GameData.PlayerInfo playerById = this.GetPlayerById(player.PlayerId);
		if (playerById == null)
		{
			return;
		}
		if (AmongUsClient.Instance.IsGameStarted)
		{
			if (!playerById.Disconnected)
			{
				playerById.Disconnected = true;
				this.LastDeathReason = DeathReason.Disconnect;
				this.ShowNotification(playerById.PlayerName, reason);
			}
		}
		else if (this.RemovePlayer(player.PlayerId))
		{
			this.ShowNotification(playerById.PlayerName, reason);
		}
		this.RecomputeTaskCounts();
	}

	// Token: 0x0600031A RID: 794 RVA: 0x00016C64 File Offset: 0x00014E64
	private void ShowNotification(string playerName, DisconnectReasons reason)
	{
		if (string.IsNullOrEmpty(playerName))
		{
			return;
		}
		if (reason != DisconnectReasons.ExitGame)
		{
			if (reason == DisconnectReasons.Banned)
			{
				GameData.PlayerInfo data = AmongUsClient.Instance.GetHost().Character.Data;
				DestroyableSingleton<HudManager>.Instance.Notifier.AddItem(playerName + " was banned by " + data.PlayerName);
				return;
			}
			if (reason == DisconnectReasons.Kicked)
			{
				GameData.PlayerInfo data2 = AmongUsClient.Instance.GetHost().Character.Data;
				DestroyableSingleton<HudManager>.Instance.Notifier.AddItem(playerName + " was kicked by " + data2.PlayerName);
				return;
			}
		}
		DestroyableSingleton<HudManager>.Instance.Notifier.AddItem(playerName + " left the game");
	}

	// Token: 0x0600031B RID: 795 RVA: 0x00016D0C File Offset: 0x00014F0C
	public void HandleDisconnect()
	{
		if (!AmongUsClient.Instance.IsGameStarted)
		{
			for (int i = this.AllPlayers.Count - 1; i >= 0; i--)
			{
				if (!this.AllPlayers[i].Object)
				{
					this.AllPlayers.RemoveAt(i);
				}
			}
		}
	}

	// Token: 0x0600031C RID: 796 RVA: 0x00016D64 File Offset: 0x00014F64
	public override bool Serialize(MessageWriter writer, bool initialState)
	{
		if (initialState)
		{
			if (!DestroyableSingleton<Telemetry>.Instance.IsInitialized)
			{
				DestroyableSingleton<Telemetry>.Instance.Initialize();
			}
			writer.WriteBytesAndSize(DestroyableSingleton<Telemetry>.Instance.CurrentGuid.ToByteArray());
			writer.WritePacked(this.AllPlayers.Count);
			for (int i = 0; i < this.AllPlayers.Count; i++)
			{
				GameData.PlayerInfo playerInfo = this.AllPlayers[i];
				writer.Write(playerInfo.PlayerId);
				playerInfo.Serialize(writer);
			}
		}
		else
		{
			int position = writer.Position;
			writer.Write(0);
			byte b = 0;
			for (int j = 0; j < this.AllPlayers.Count; j++)
			{
				GameData.PlayerInfo playerInfo2 = this.AllPlayers[j];
				if ((this.DirtyBits & 1U << (int)playerInfo2.PlayerId) != 0U)
				{
					writer.Write(playerInfo2.PlayerId);
					playerInfo2.Serialize(writer);
					b += 1;
				}
			}
			writer.Position = position;
			writer.Write(b);
			writer.Position = writer.Length;
			this.DirtyBits = 0U;
		}
		return true;
	}

	// Token: 0x0600031D RID: 797 RVA: 0x00016E78 File Offset: 0x00015078
	public override void Deserialize(MessageReader reader, bool initialState)
	{
		if (initialState)
		{
			Guid gameGuid = new Guid(reader.ReadBytesAndSize());
			if (!DestroyableSingleton<Telemetry>.Instance.IsInitialized)
			{
				DestroyableSingleton<Telemetry>.Instance.Initialize(gameGuid);
			}
			int num = reader.ReadPackedInt32();
			for (int i = 0; i < num; i++)
			{
				GameData.PlayerInfo playerInfo = new GameData.PlayerInfo(reader.ReadByte());
				playerInfo.Deserialize(reader);
				this.AllPlayers.Add(playerInfo);
			}
		}
		else
		{
			byte b = reader.ReadByte();
			for (int j = 0; j < (int)b; j++)
			{
				byte b2 = reader.ReadByte();
				GameData.PlayerInfo playerInfo2 = this.GetPlayerById(b2);
				if (playerInfo2 != null)
				{
					playerInfo2.Deserialize(reader);
				}
				else
				{
					playerInfo2 = new GameData.PlayerInfo(b2);
					playerInfo2.Deserialize(reader);
					this.AllPlayers.Add(playerInfo2);
				}
			}
		}
		this.RecomputeTaskCounts();
	}

	// Token: 0x0600031E RID: 798 RVA: 0x0000405A File Offset: 0x0000225A
	public void RpcSetTasks(byte playerId, byte[] taskTypeIds)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.SetTasks(playerId, taskTypeIds);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 0, SendOption.Reliable);
		messageWriter.Write(playerId);
		messageWriter.WriteBytesAndSize(taskTypeIds);
		messageWriter.EndMessage();
	}

	// Token: 0x0600031F RID: 799 RVA: 0x00016F40 File Offset: 0x00015140
	public override void HandleRpc(byte callId, MessageReader reader)
	{
		if (callId == 0)
		{
			this.SetTasks(reader.ReadByte(), reader.ReadBytesAndSize());
		}
	}

	// Token: 0x04000308 RID: 776
	public static GameData Instance;

	// Token: 0x04000309 RID: 777
	public List<GameData.PlayerInfo> AllPlayers = new List<GameData.PlayerInfo>();

	// Token: 0x0400030A RID: 778
	public int TotalTasks;

	// Token: 0x0400030B RID: 779
	public int CompletedTasks;

	// Token: 0x0400030C RID: 780
	public DeathReason LastDeathReason;

	// Token: 0x0400030D RID: 781
	public const byte InvalidPlayerId = 255;

	// Token: 0x0400030E RID: 782
	public const byte DisconnectedPlayerId = 254;

	// Token: 0x02000092 RID: 146
	public class TaskInfo
	{
		// Token: 0x06000321 RID: 801 RVA: 0x000040A8 File Offset: 0x000022A8
		public void Serialize(MessageWriter writer)
		{
			writer.WritePacked(this.Id);
			writer.Write(this.Complete);
		}

		// Token: 0x06000322 RID: 802 RVA: 0x000040C2 File Offset: 0x000022C2
		public void Deserialize(MessageReader reader)
		{
			this.Id = reader.ReadPackedUInt32();
			this.Complete = reader.ReadBoolean();
		}

		// Token: 0x0400030F RID: 783
		public uint Id;

		// Token: 0x04000310 RID: 784
		public bool Complete;
	}

	// Token: 0x02000093 RID: 147
	public class PlayerInfo
	{
		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000324 RID: 804 RVA: 0x000040DC File Offset: 0x000022DC
		public PlayerControl Object
		{
			get
			{
				if (!this._object)
				{
					this._object = PlayerControl.AllPlayerControls.FirstOrDefault((PlayerControl p) => p.PlayerId == this.PlayerId);
				}
				return this._object;
			}
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0000410D File Offset: 0x0000230D
		public PlayerInfo(byte playerId)
		{
			this.PlayerId = playerId;
		}

		// Token: 0x06000326 RID: 806 RVA: 0x00004127 File Offset: 0x00002327
		public PlayerInfo(PlayerControl pc) : this(pc.PlayerId)
		{
			this._object = pc;
		}

		// Token: 0x06000327 RID: 807 RVA: 0x00016F64 File Offset: 0x00015164
		public void Serialize(MessageWriter writer)
		{
			writer.Write(this.PlayerName);
			writer.Write(this.ColorId);
			writer.WritePacked(this.HatId);
			writer.WritePacked(this.SkinId);
			writer.Write((byte)this.role);
			byte b = 0;
			if (this.Disconnected)
			{
				b |= 1;
			}
			if (this.IsImpostor)
			{
				b |= 2;
			}
			if (this.IsDead)
			{
				b |= 4;
			}
			writer.Write(b);
			if (this.Tasks != null)
			{
				writer.Write((byte)this.Tasks.Count);
				for (int i = 0; i < this.Tasks.Count; i++)
				{
					this.Tasks[i].Serialize(writer);
				}
				return;
			}
			byte value = 0;
			writer.Write(value);
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0001702C File Offset: 0x0001522C
		public void Deserialize(MessageReader reader)
		{
			this.PlayerName = reader.ReadString();
			this.ColorId = reader.ReadByte();
			this.HatId = reader.ReadPackedUInt32();
			this.SkinId = reader.ReadPackedUInt32();
			this.role = (GameData.PlayerInfo.Role)reader.ReadByte();
			byte b = reader.ReadByte();
			this.Disconnected = ((b & 1) > 0);
			this.IsImpostor = ((b & 2) > 0);
			this.IsDead = ((b & 4) > 0);
			byte b2 = reader.ReadByte();
			this.Tasks = new List<GameData.TaskInfo>((int)b2);
			for (int i = 0; i < (int)b2; i++)
			{
				this.Tasks.Add(new GameData.TaskInfo());
				this.Tasks[i].Deserialize(reader);
			}
		}

		// Token: 0x06000329 RID: 809 RVA: 0x000170E4 File Offset: 0x000152E4
		public GameData.TaskInfo FindTaskById(uint taskId)
		{
			for (int i = 0; i < this.Tasks.Count; i++)
			{
				if (this.Tasks[i].Id == taskId)
				{
					return this.Tasks[i];
				}
			}
			return null;
		}

		// Token: 0x04000311 RID: 785
		public readonly byte PlayerId;

		// Token: 0x04000312 RID: 786
		public string PlayerName = string.Empty;

		// Token: 0x04000313 RID: 787
		public byte ColorId;

		// Token: 0x04000314 RID: 788
		public uint HatId;

		// Token: 0x04000315 RID: 789
		public uint SkinId;

		// Token: 0x04000316 RID: 790
		public bool Disconnected;

		// Token: 0x04000317 RID: 791
		public List<GameData.TaskInfo> Tasks;

		// Token: 0x04000318 RID: 792
		public bool IsImpostor;

		// Token: 0x04000319 RID: 793
		public bool IsDead;

		// Token: 0x0400031A RID: 794
		private PlayerControl _object;

		// Token: 0x0400031B RID: 795
		public GameData.PlayerInfo.Role role;

		// Token: 0x02000094 RID: 148
		public enum Role : byte
		{
			// Token: 0x0400031D RID: 797
			None,
			// Token: 0x0400031E RID: 798
			Sheriff,
			// Token: 0x0400031F RID: 799
			Joker
		}
	}

	// Token: 0x02000095 RID: 149
	private enum RpcCalls
	{
		// Token: 0x04000321 RID: 801
		SetTasks
	}
}
