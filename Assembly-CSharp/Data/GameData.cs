using System;
using System.Collections.Generic;
using System.Linq;
using Assets.CoreScripts;
using Hazel;
using InnerNet;
using UnityEngine;

public class GameData : InnerNetObject, IDisconnectHandler
{
	public class TaskInfo
	{
		public uint Id;

		public bool Complete;

		public void Serialize(MessageWriter writer)
		{
			writer.WritePacked(Id);
			writer.Write(Complete);
		}

		public void Deserialize(MessageReader reader)
		{
			Id = reader.ReadPackedUInt32();
			Complete = reader.ReadBoolean();
		}
	}

	public int GetAllTaskDoingPlayersCount() //lazily reused function
	{
		int valid = 0;
		foreach (GameData.PlayerInfo plrfo in AllPlayers)
		{
			if (CE_RoleManager.GetRoleFromID(plrfo.role).HasTasks && !plrfo.Disconnected && !plrfo.IsImpostor)
			{
				valid++;
			}
		}
		return valid;
	}

	public class PlayerInfo
	{

		public readonly byte PlayerId;

		public string PlayerName = string.Empty;

		public uint ColorId;

		public uint HatId;

		public uint SkinId;

		public bool Disconnected;

		public List<TaskInfo> Tasks;

		public bool IsImpostor;

		public bool IsDead;

		private PlayerControl _object;

		public byte role;

        public byte luavalue1;

        public byte luavalue2;

		public byte luavalue3;

		public PlayerControl Object
		{
			get
			{
				if (!_object)
				{
					_object = PlayerControl.AllPlayerControls.FirstOrDefault((PlayerControl p) => p.PlayerId == PlayerId);
				}
				return _object;
			}
		}

		public PlayerInfo(byte playerId)
		{
			PlayerId = playerId;
		}

		public PlayerInfo(PlayerControl pc)
			: this(pc.PlayerId)
		{
			_object = pc;
		}

		public void Serialize(MessageWriter writer)
		{
			writer.Write(PlayerName);
			writer.WritePacked(ColorId);
			writer.WritePacked(HatId);
			writer.WritePacked(SkinId);
			writer.Write((byte)role);
            writer.Write(luavalue1);
            writer.Write(luavalue2);
			writer.Write(luavalue3);
			byte b = 0;
			if (Disconnected)
			{
				b = (byte)(b | 1u);
			}
			if (IsImpostor)
			{
				b = (byte)(b | 2u);
			}
			if (IsDead)
			{
				b = (byte)(b | 4u);
			}
			writer.Write(b);
			if (Tasks != null)
			{
				writer.Write((byte)Tasks.Count);
				for (int i = 0; i < Tasks.Count; i++)
				{
					Tasks[i].Serialize(writer);
				}
			}
			else
			{
				byte value = 0;
				writer.Write(value);
			}
		}

		public void Deserialize(MessageReader reader)
		{
			PlayerName = reader.ReadString();
			ColorId = reader.ReadPackedUInt32();
			HatId = reader.ReadPackedUInt32();
			SkinId = reader.ReadPackedUInt32();
			role = reader.ReadByte();
            luavalue1 = reader.ReadByte();
            luavalue2 = reader.ReadByte();
			luavalue3 = reader.ReadByte();
			byte b = reader.ReadByte();
			Disconnected = (b & 1) > 0;
			IsImpostor = (b & 2) > 0;
			IsDead = (b & 4) > 0;
			byte b2 = reader.ReadByte();
			Tasks = new List<TaskInfo>(b2);
			for (int i = 0; i < b2; i++)
			{
				Tasks.Add(new TaskInfo());
				Tasks[i].Deserialize(reader);
			}
		}

		public TaskInfo FindTaskById(uint taskId)
		{
			for (int i = 0; i < Tasks.Count; i++)
			{
				if (Tasks[i].Id == taskId)
				{
					return Tasks[i];
				}
			}
			return null;
		}
	}

	private enum RpcCalls
	{
		SetTasks
	}

	public static GameData Instance;

	public List<PlayerInfo> AllPlayers = new List<PlayerInfo>();

	public int TotalTasks;

	public int CompletedTasks;

	public DeathReason LastDeathReason;

	public const byte InvalidPlayerId = byte.MaxValue;

	public const byte DisconnectedPlayerId = 254;

	public int PlayerCount => AllPlayers.Count;

	public void Awake()
	{
		if ((bool)Instance && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		if ((bool)AmongUsClient.Instance)
		{
			AmongUsClient.Instance.DisconnectHandlers.AddUnique(this);
		}
	}

	public PlayerInfo GetHost()
	{
		ClientData host = AmongUsClient.Instance.GetHost();
		if (host != null && (bool)host.Character)
		{
			return host.Character.Data;
		}
		return null;
	}

	public sbyte GetAvailableId()
	{
		sbyte i;
		for (i = 0; i < 20; i++)
		{
			if (!AllPlayers.Any((PlayerInfo p) => p.PlayerId == i))
			{
				return i;
			}
		}
		return -1;
	}

	public PlayerInfo GetPlayerById(byte id)
	{
		if (id == byte.MaxValue)
		{
			return null;
		}
		for (int i = 0; i < AllPlayers.Count; i++)
		{
			if (AllPlayers[i].PlayerId == id)
			{
				return AllPlayers[i];
			}
		}
		return null;
	}

	public void UpdateName(byte playerId, string name)
	{
		PlayerInfo playerById = GetPlayerById(playerId);
		if (playerById != null)
		{
			playerById.PlayerName = name;
		}
	}

	public void UpdateColor(byte playerId, uint color)
	{
		PlayerInfo playerById = GetPlayerById(playerId);
        if (playerById != null)
        {
            playerById.ColorId = color;
			uint hatId = playerById.HatId;
			playerById.Object.UpdateHat(hatId);
		}
	}

	public void UpdateHat(byte playerId, uint hat)
	{
		PlayerInfo playerById = GetPlayerById(playerId);
		if (playerById != null)
		{
			playerById.HatId = hat;
		}
	}

	public void UpdateLuaValue(byte playerId, byte num, byte id)
	{
		PlayerInfo playerById = GetPlayerById(playerId);
		if (playerById != null)
		{
			switch (id)
            {
				case 1:
					playerById.luavalue1 = num;
					break;
				case 2:
					playerById.luavalue2 = num;
                    break;
				case 3:
					playerById.luavalue3 = num;
					break;
				default:
					Debug.Log("Invalid ID:" + id);
					break;
			}
			
		}
	}

	public void UpdateSkin(byte playerId, uint skin)
	{
		PlayerInfo playerById = GetPlayerById(playerId);
		if (playerById != null)
		{
			playerById.SkinId = skin;
		}
	}

	public void AddPlayer(PlayerControl pc)
	{
		PlayerInfo item = new PlayerInfo(pc);
		AllPlayers.Add(item);
		SetDirtyBit((uint)(1 << (int)pc.PlayerId));
	}

	public bool RemovePlayer(byte playerId)
	{
		for (int i = 0; i < AllPlayers.Count; i++)
		{
			if (AllPlayers[i].PlayerId == playerId)
			{
				DirtyBits &= (uint)(~(1 << (int)playerId));
				AllPlayers.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public void RecomputeTaskCounts()
	{
		TotalTasks = 0;
		CompletedTasks = 0;
		for (int i = 0; i < AllPlayers.Count; i++)
		{
			PlayerInfo playerInfo = AllPlayers[i];
			if (playerInfo.Disconnected || playerInfo.Tasks == null || !playerInfo.Object || (!PlayerControl.GameOptions.GhostsDoTasks && playerInfo.IsDead) || (playerInfo.IsImpostor || CE_RoleManager.GetRoleFromID(playerInfo.role).DoesNotDoTasks()))
			{
				continue;
			}
			for (int j = 0; j < playerInfo.Tasks.Count; j++)
			{
				TotalTasks++;
				if (playerInfo.Tasks[j].Complete)
				{
					CompletedTasks++;
				}
			}
		}
	}

	public void TutOnlyRemoveTask(byte playerId, uint taskId)
	{
		PlayerInfo playerById = GetPlayerById(playerId);
		TaskInfo item = playerById.FindTaskById(taskId);
		playerById.Tasks.Remove(item);
		RecomputeTaskCounts();
	}

	public void TutOnlyAddTask(byte playerId, uint taskId)
	{
		GetPlayerById(playerId).Tasks.Add(new TaskInfo
		{
			Id = taskId
		});
		TotalTasks++;
	}

	private void SetTasks(byte playerId, byte[] taskTypeIds)
	{
		PlayerInfo playerById = GetPlayerById(playerId);
		if (playerById == null)
		{
			Debug.Log("Could not set tasks for player id: " + playerId);
		}
		else
		{
			if (playerById.Disconnected)
			{
				return;
			}
			if (!playerById.Object)
			{
				Debug.Log("Could not set tasks for player (" + playerById.PlayerName + "): " + playerId);
				return;
			}
			playerById.Tasks = new List<TaskInfo>(taskTypeIds.Length);
			for (int i = 0; i < taskTypeIds.Length; i++)
			{
				playerById.Tasks.Add(new TaskInfo());
				playerById.Tasks[i].Id = (uint)i;
			}
			playerById.Object.SetTasks(taskTypeIds);
			SetDirtyBit((uint)(1 << (int)playerById.PlayerId));
		}
	}

	public void CompleteTask(PlayerControl pc, uint taskId)
	{
		TaskInfo taskInfo = GetPlayerById(pc.PlayerId).FindTaskById(taskId);
        if (taskInfo != null)
        {
            if (!taskInfo.Complete)
            {
                taskInfo.Complete = true;
                CompletedTasks++;
            }
            else
            {
                Debug.LogWarning("Double complete task: " + taskId);
            }
        }
        else
        {
            Debug.LogWarning("Couldn't find task: " + taskId);
        }
	
	}

	public void HandleDisconnect(PlayerControl player, DisconnectReasons reason)
	{
		if (!player)
		{
			return;
		}
		PlayerInfo playerById = GetPlayerById(player.PlayerId);
		if (playerById == null)
		{
			return;
		}
		if (AmongUsClient.Instance.IsGameStarted)
		{
			if (!playerById.Disconnected)
			{
				playerById.Disconnected = true;
				LastDeathReason = DeathReason.Disconnect;
				ShowNotification(playerById.PlayerName, reason);
			}
		}
		else if (RemovePlayer(player.PlayerId))
		{
			ShowNotification(playerById.PlayerName, reason);
		}
		RecomputeTaskCounts();
	}

	private void ShowNotification(string playerName, DisconnectReasons reason)
	{
		if (!string.IsNullOrEmpty(playerName))
		{
			switch (reason)
			{
			default:
				DestroyableSingleton<HudManager>.Instance.Notifier.AddItem(playerName + " left the game");
				break;
			case DisconnectReasons.Banned:
			{
				PlayerInfo data2 = AmongUsClient.Instance.GetHost().Character.Data;
				DestroyableSingleton<HudManager>.Instance.Notifier.AddItem(playerName + " was banned by " + data2.PlayerName);
				break;
			}
			case DisconnectReasons.Kicked:
            {
                PlayerInfo data = AmongUsClient.Instance.GetHost().Character.Data;
				DestroyableSingleton<HudManager>.Instance.Notifier.AddItem(playerName + " was kicked by " + data.PlayerName);
            break;
            }
			case DisconnectReasons.Error:
            {
				DestroyableSingleton<HudManager>.Instance.Notifier.AddItem(playerName + " left due to an error.");
				break;
            }
			}
		}
	}

	public void HandleDisconnect()
	{
		if (AmongUsClient.Instance.IsGameStarted)
		{
			return;
		}
		for (int num = AllPlayers.Count - 1; num >= 0; num--)
		{
			if (!AllPlayers[num].Object)
			{
				AllPlayers.RemoveAt(num);
			}
		}
	}

	public override bool Serialize(MessageWriter writer, bool initialState)
	{
		if (initialState)
		{
			if (!DestroyableSingleton<Telemetry>.Instance.IsInitialized)
			{
				DestroyableSingleton<Telemetry>.Instance.Initialize();
			}
			writer.WriteBytesAndSize(DestroyableSingleton<Telemetry>.Instance.CurrentGuid.ToByteArray());
			writer.WritePacked(AllPlayers.Count);
			for (int i = 0; i < AllPlayers.Count; i++)
			{
				PlayerInfo playerInfo = AllPlayers[i];
				writer.Write(playerInfo.PlayerId);
				playerInfo.Serialize(writer);
			}
		}
		else
		{
			int position = writer.Position;
			writer.Write((byte)0);
			byte b = 0;
			for (int j = 0; j < AllPlayers.Count; j++)
			{
				PlayerInfo playerInfo2 = AllPlayers[j];
				if ((DirtyBits & (uint)(1 << (int)playerInfo2.PlayerId)) != 0)
				{
					writer.Write(playerInfo2.PlayerId);
					playerInfo2.Serialize(writer);
					b = (byte)(b + 1);
				}
			}
			writer.Position = position;
			writer.Write(b);
			writer.Position = writer.Length;
			DirtyBits = 0u;
		}
		return true;
	}

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
				PlayerInfo playerInfo = new PlayerInfo(reader.ReadByte());
				playerInfo.Deserialize(reader);
				AllPlayers.Add(playerInfo);
			}
		}
		else
		{
			byte b = reader.ReadByte();
			for (int j = 0; j < b; j++)
			{
				byte b2 = reader.ReadByte();
				PlayerInfo playerById = GetPlayerById(b2);
				if (playerById != null)
				{
					playerById.Deserialize(reader);
					continue;
				}
				playerById = new PlayerInfo(b2);
				playerById.Deserialize(reader);
				AllPlayers.Add(playerById);
			}
		}
		RecomputeTaskCounts();
	}

	public void RpcSetTasks(byte playerId, byte[] taskTypeIds)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			SetTasks(playerId, taskTypeIds);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 0);
		messageWriter.Write(playerId);
		messageWriter.WriteBytesAndSize(taskTypeIds);
		messageWriter.EndMessage();
	}

	public override void HandleRpc(byte callId, MessageReader reader)
	{
		if (callId == 0)
		{
			SetTasks(reader.ReadByte(), reader.ReadBytesAndSize());
		}
	}
}
