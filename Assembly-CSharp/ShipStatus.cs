using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using InnerNet;
using PowerTools;
using UnityEngine;

// Token: 0x020001E5 RID: 485
public class ShipStatus : InnerNetObject
{
	// Token: 0x17000198 RID: 408
	// (get) Token: 0x06000A6D RID: 2669 RVA: 0x00008502 File Offset: 0x00006702
	// (set) Token: 0x06000A6E RID: 2670 RVA: 0x0000850A File Offset: 0x0000670A
	public ShipRoom[] AllRooms { get; private set; }

	// Token: 0x17000199 RID: 409
	// (get) Token: 0x06000A6F RID: 2671 RVA: 0x00008513 File Offset: 0x00006713
	// (set) Token: 0x06000A70 RID: 2672 RVA: 0x0000851B File Offset: 0x0000671B
	public Vent[] AllVents { get; private set; }

	// Token: 0x06000A71 RID: 2673 RVA: 0x0003588C File Offset: 0x00033A8C
	public ShipStatus()
	{
		this.Systems = new Dictionary<SystemTypes, ISystemType>(ShipStatus.SystemTypeComparer.Instance)
		{
			{
				SystemTypes.Electrical,
				new SwitchSystem()
			},
			{
				SystemTypes.MedBay,
				new MedScanSystem()
			},
			{
				SystemTypes.Reactor,
				new ReactorSystemType()
			},
			{
				SystemTypes.LifeSupp,
				new LifeSuppSystemType()
			},
			{
				SystemTypes.Security,
				new SecurityCameraSystemType()
			},
			{
				SystemTypes.Comms,
				new HudOverrideSystemType()
			},
			{
				SystemTypes.Doors,
				new DoorsSystemType()
			}
		};
		this.Systems.Add(SystemTypes.Sabotage, new SabotageSystemType(new IActivatable[]
		{
			(IActivatable)this.Systems[SystemTypes.Comms],
			(IActivatable)this.Systems[SystemTypes.Reactor],
			(IActivatable)this.Systems[SystemTypes.LifeSupp],
			(IActivatable)this.Systems[SystemTypes.Electrical]
		}));
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x00008524 File Offset: 0x00006724
	private void Awake()
	{
		this.AllRooms = base.GetComponentsInChildren<ShipRoom>();
		this.AllConsoles = base.GetComponentsInChildren<global::Console>();
		this.AllVents = base.GetComponentsInChildren<Vent>();
		this.AssignTaskIndexes();
		ShipStatus.Instance = this;
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x000359C0 File Offset: 0x00033BC0
	public void Start()
	{
		Camera.main.backgroundColor = this.CameraColor;
		if (DestroyableSingleton<HudManager>.InstanceExists)
		{
			DestroyableSingleton<HudManager>.Instance.Chat.ForceClosed();
			DestroyableSingleton<HudManager>.Instance.Chat.SetVisible(false);
			DestroyableSingleton<HudManager>.Instance.GameSettings.gameObject.SetActive(false);
		}
		DeconSystem componentInChildren = base.GetComponentInChildren<DeconSystem>();
		if (componentInChildren)
		{
			this.Systems.Add(SystemTypes.Decontamination, componentInChildren);
		}
		LobbyBehaviour instance = LobbyBehaviour.Instance;
		if (instance)
		{
			UnityEngine.Object.Destroy(instance.gameObject);
		}
		SoundManager.Instance.StopAllSound();
		AudioSource audioSource = SoundManager.Instance.PlaySound(this.shipHum, true, 1f);
		if (audioSource)
		{
			audioSource.pitch = 0.8f;
		}
		if (Constants.ShouldPlaySfx())
		{
			for (int i = 0; i < this.AllRooms.Length; i++)
			{
				ShipRoom room = this.AllRooms[i];
				if (room.AmbientSound)
				{
					SoundManager.Instance.PlayDynamicSound("Amb " + room.RoomId, room.AmbientSound, true, delegate(AudioSource player, float dt)
					{
						this.GetAmbientSoundVolume(room, player, dt);
					}, false);
				}
			}
		}
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x00008556 File Offset: 0x00006756
	public override void OnDestroy()
	{
		SoundManager.Instance.StopAllSound();
		base.OnDestroy();
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x00035B10 File Offset: 0x00033D10
	public Vector2 GetSpawnLocation(int playerId, int numPlayer)
	{
		Vector2 vector = Vector2.up;
		vector = vector.Rotate((float)(playerId - 1) * (360f / (float)numPlayer));
		vector *= this.SpawnRadius;
		return (Vector2)this.SpawnCenter.position + vector + new Vector2(0f, 0.3636f);
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x00035B70 File Offset: 0x00033D70
	public void StartShields()
	{
		if (!PlayerControl.GameOptions.Visuals)
		{
			return;
		}
		for (int i = 0; i < this.ShieldsImages.Length; i++)
		{
			this.ShieldsImages[i].Play(this.ShieldsActive, 1f);
		}
		this.ShieldBorder.sprite = this.ShieldBorderOn;
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x00035BC8 File Offset: 0x00033DC8
	public void FireWeapon()
	{
		if (!PlayerControl.GameOptions.Visuals)
		{
			return;
		}
		if (!this.WeaponsImage.IsPlaying((AnimationClip)null))
		{
			this.WeaponsImage.Play(this.WeaponFires[this.WeaponFireIdx], 1f);
			this.WeaponFireIdx = (this.WeaponFireIdx + 1) % 2;
		}
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x00035C20 File Offset: 0x00033E20
	public NormalPlayerTask GetTaskById(byte idx)
	{
		NormalPlayerTask result;
		if ((result = this.CommonTasks.FirstOrDefault((NormalPlayerTask t) => t.Index == (int)idx)) == null)
		{
			result = (this.LongTasks.FirstOrDefault((NormalPlayerTask t) => t.Index == (int)idx) ?? this.NormalTasks.FirstOrDefault((NormalPlayerTask t) => t.Index == (int)idx));
		}
		return result;
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x00008568 File Offset: 0x00006768
	public void OpenHatch()
	{
		if (!PlayerControl.GameOptions.Visuals)
		{
			return;
		}
		if (!this.Hatch.IsPlaying((AnimationClip)null))
		{
			this.Hatch.Play(this.HatchActive, 1f);
			this.HatchParticles.Play();
		}
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x000085A6 File Offset: 0x000067A6
	public void CloseDoorsOfType(SystemTypes room)
	{
		(this.Systems[SystemTypes.Doors] as DoorsSystemType).CloseDoorsOfType(room);
		base.SetDirtyBit(65536U);
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x000085CB File Offset: 0x000067CB
	public void RepairSystem(SystemTypes systemType, PlayerControl player, byte amount)
	{
		this.Systems[systemType].RepairDamage(player, amount);
		base.SetDirtyBit(1U << (int)systemType);
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x00035C88 File Offset: 0x00033E88
	internal void SelectInfected()
	{
		if (PlayerControl.GameOptions.Gamemode == 2)
		{
			this.SelectSherrif();
		}
		List<GameData.PlayerInfo> list = (from pcd in GameData.Instance.AllPlayers
		where !pcd.Disconnected
		select pcd into pc
		where !pc.IsDead
		select pc into pcx
		where pcx.role == GameData.PlayerInfo.Role.None
		select pcx).ToList<GameData.PlayerInfo>();
		list.Shuffle<GameData.PlayerInfo>();
		GameData.PlayerInfo[] infected = list.Take(PlayerControl.GameOptions.NumImpostors).ToArray<GameData.PlayerInfo>();
		PlayerControl.LocalPlayer.RpcSetInfected(infected);
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x00035D4C File Offset: 0x00033F4C
	private void AssignTaskIndexes()
	{
		int num = 0;
		for (int i = 0; i < this.CommonTasks.Length; i++)
		{
			this.CommonTasks[i].Index = num++;
		}
		for (int j = 0; j < this.LongTasks.Length; j++)
		{
			this.LongTasks[j].Index = num++;
		}
		for (int k = 0; k < this.NormalTasks.Length; k++)
		{
			this.NormalTasks[k].Index = num++;
		}
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x00035DCC File Offset: 0x00033FCC
	public void Begin()
	{
		this.AssignTaskIndexes();
		GameOptionsData gameOptions = PlayerControl.GameOptions;
		List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
		HashSet<TaskTypes> hashSet = new HashSet<TaskTypes>();
		List<byte> list = new List<byte>(10);
		List<NormalPlayerTask> list2 = this.CommonTasks.ToList<NormalPlayerTask>();
		list2.Shuffle<NormalPlayerTask>();
		int num = 0;
		this.AddTasksFromList(ref num, gameOptions.NumCommonTasks, list, hashSet, list2);
		for (int i = 0; i < gameOptions.NumCommonTasks; i++)
		{
			if (list2.Count == 0)
			{
				Debug.LogWarning("Not enough common tasks");
				break;
			}
			int index = list2.RandomIdx<NormalPlayerTask>();
			list.Add((byte)list2[index].Index);
			list2.RemoveAt(index);
		}
		List<NormalPlayerTask> list3 = this.LongTasks.ToList<NormalPlayerTask>();
		list3.Shuffle<NormalPlayerTask>();
		List<NormalPlayerTask> list4 = this.NormalTasks.ToList<NormalPlayerTask>();
		list4.Shuffle<NormalPlayerTask>();
		int num2 = 0;
		int num3 = 0;
		byte b = 0;
		while ((int)b < allPlayers.Count)
		{
			hashSet.Clear();
			list.RemoveRange(gameOptions.NumCommonTasks, list.Count - gameOptions.NumCommonTasks);
			this.AddTasksFromList(ref num2, gameOptions.NumLongTasks, list, hashSet, list3);
			this.AddTasksFromList(ref num3, gameOptions.NumShortTasks, list, hashSet, list4);
			GameData.PlayerInfo playerInfo = allPlayers[(int)b];
			if (playerInfo.Object && !playerInfo.Object.GetComponent<DummyBehaviour>().enabled)
			{
				byte[] taskTypeIds = list.ToArray();
				GameData.Instance.RpcSetTasks(playerInfo.PlayerId, taskTypeIds);
			}
			b += 1;
		}
		base.enabled = true;
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x00035F54 File Offset: 0x00034154
	private void AddTasksFromList(ref int start, int count, List<byte> tasks, HashSet<TaskTypes> usedTaskTypes, List<NormalPlayerTask> unusedTasks)
	{
		int num = 0;
		int num2 = 0;
		Func<NormalPlayerTask, bool> UNK_1 = null;
		while (num2 < count && num++ != 1000)
		{
			if (start >= unusedTasks.Count)
			{
				start = 0;
				unusedTasks.Shuffle<NormalPlayerTask>();
				Func<NormalPlayerTask, bool> predicate;
				if ((predicate = UNK_1) == null)
				{
					predicate = (UNK_1 = ((NormalPlayerTask t) => usedTaskTypes.Contains(t.TaskType)));
				}
				if (unusedTasks.All(predicate))
				{
					Debug.Log("Not enough task types");
					usedTaskTypes.Clear();
				}
			}
			int num3 = start;
			start = num3 + 1;
			NormalPlayerTask normalPlayerTask = unusedTasks[num3];
			if (!usedTaskTypes.Add(normalPlayerTask.TaskType))
			{
				num2--;
			}
			else
			{
				tasks.Add((byte)normalPlayerTask.Index);
			}
			num2++;
		}
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x00036020 File Offset: 0x00034220
	public void FixedUpdate()
	{
		if (!AmongUsClient.Instance)
		{
			return;
		}
		this.Timer += Time.fixedDeltaTime;
		if (this.Timer > 75f && SaveManager.LastGameStart != DateTime.MinValue)
		{
			SaveManager.LastGameStart = DateTime.MinValue;
		}
		if (GameData.Instance)
		{
			GameData.Instance.RecomputeTaskCounts();
		}
		if (AmongUsClient.Instance.AmHost)
		{
			this.CheckEndCriteria();
		}
		if (AmongUsClient.Instance.AmClient)
		{
			for (int i = 0; i < SystemTypeHelpers.AllTypes.Length; i++)
			{
				SystemTypes systemTypes = SystemTypeHelpers.AllTypes[i];
				ISystemType systemType;
				if (this.Systems.TryGetValue(systemTypes, out systemType) && systemType.Detoriorate(Time.fixedDeltaTime))
				{
					base.SetDirtyBit(1U << (int)systemTypes);
				}
			}
		}
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x000360EC File Offset: 0x000342EC
	private void GetAmbientSoundVolume(ShipRoom room, AudioSource player, float dt)
	{
		if (!PlayerControl.LocalPlayer)
		{
			player.volume = 0f;
			return;
		}
		Vector2 vector = room.transform.position;
		Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
		float num = Vector2.Distance(vector, truePosition);
		if (num > 8f)
		{
			player.volume = 0f;
			return;
		}
		Vector2 direction = truePosition - vector;
		int num2 = Physics2D.RaycastNonAlloc(vector, direction, this.volumeBuffer, num, Constants.ShipOnlyMask);
		float num3 = 1f - num / 8f - (float)num2 * 0.25f;
		player.volume = Mathf.Lerp(player.volume, num3 * 0.7f, dt);
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x0003619C File Offset: 0x0003439C
	public float CalculateLightRadius(GameData.PlayerInfo player)
	{
		if (player.IsDead)
		{
			return this.MaxLightRadius;
		}
		SwitchSystem switchSystem = (SwitchSystem)this.Systems[SystemTypes.Electrical];
		if (player.IsImpostor)
		{
			return this.MaxLightRadius * PlayerControl.GameOptions.ImpostorLightMod;
		}
		float t = (float)switchSystem.Value / 255f;
		return Mathf.Lerp(this.MinLightRadius, this.MaxLightRadius, t) * PlayerControl.GameOptions.CrewLightMod;
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x00036210 File Offset: 0x00034410
	public override bool Serialize(MessageWriter writer, bool initialState)
	{
		if (initialState)
		{
			(this.Systems[SystemTypes.Doors] as DoorsSystemType).SetDoors(this.AllDoors);
			short num = 0;
			while ((int)num < SystemTypeHelpers.AllTypes.Length)
			{
				SystemTypes key = SystemTypeHelpers.AllTypes[(int)num];
				ISystemType systemType;
				if (this.Systems.TryGetValue(key, out systemType))
				{
					systemType.Serialize(writer, true);
				}
				num += 1;
			}
			return true;
		}
		if (this.DirtyBits != 0U)
		{
			writer.WritePacked(this.DirtyBits);
			short num2 = 0;
			while ((int)num2 < SystemTypeHelpers.AllTypes.Length)
			{
				SystemTypes systemTypes = SystemTypeHelpers.AllTypes[(int)num2];
				ISystemType systemType2;
				if (((ulong)this.DirtyBits & (ulong)(1L << (int)(systemTypes & (SystemTypes)31))) != 0UL && this.Systems.TryGetValue(systemTypes, out systemType2))
				{
					systemType2.Serialize(writer, false);
				}
				num2 += 1;
			}
			this.DirtyBits = 0U;
			return true;
		}
		return false;
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x000362D8 File Offset: 0x000344D8
	public override void Deserialize(MessageReader reader, bool initialState)
	{
		if (initialState)
		{
			(this.Systems[SystemTypes.Doors] as DoorsSystemType).SetDoors(this.AllDoors);
			short num = 0;
			while ((int)num < SystemTypeHelpers.AllTypes.Length)
			{
				SystemTypes key = (SystemTypes)num;
				ISystemType systemType;
				if (this.Systems.TryGetValue(key, out systemType))
				{
					systemType.Deserialize(reader, true);
				}
				num += 1;
			}
			return;
		}
		uint num2 = reader.ReadPackedUInt32();
		short num3 = 0;
		while ((int)num3 < SystemTypeHelpers.AllTypes.Length)
		{
			SystemTypes systemTypes = SystemTypeHelpers.AllTypes[(int)num3];
			ISystemType systemType2;
			if (((ulong)num2 & (ulong)(1L << (int)(systemTypes & (SystemTypes)31))) != 0UL && this.Systems.TryGetValue(systemTypes, out systemType2))
			{
				systemType2.Deserialize(reader, false);
			}
			num3 += 1;
		}
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x00036384 File Offset: 0x00034584
	private void CheckEndCriteria()
	{
		if (!GameData.Instance)
		{
			return;
		}
		LifeSuppSystemType lifeSuppSystemType = (LifeSuppSystemType)this.Systems[SystemTypes.LifeSupp];
		if (lifeSuppSystemType.Countdown < 0f)
		{
			this.EndGameForSabotage();
			lifeSuppSystemType.Countdown = 10000f;
		}
		ReactorSystemType reactorSystemType = (ReactorSystemType)this.Systems[SystemTypes.Reactor];
		if (reactorSystemType.Countdown < 0f)
		{
			this.EndGameForSabotage();
			reactorSystemType.Countdown = 10000f;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < GameData.Instance.PlayerCount; i++)
		{
			GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
			if (!playerInfo.Disconnected)
			{
				if (playerInfo.IsImpostor)
				{
					num3++;
				}
				if (!playerInfo.IsDead)
				{
					if (playerInfo.IsImpostor)
					{
						num2++;
					}
					else
					{
						num++;
					}
				}
			}
		}
		if (num2 <= 0 && (!DestroyableSingleton<TutorialManager>.InstanceExists || num3 > 0))
		{
			if (!DestroyableSingleton<TutorialManager>.InstanceExists)
			{
				base.enabled = false;
				ShipStatus.RpcEndGame((GameData.Instance.LastDeathReason == DeathReason.Disconnect) ? GameOverReason.ImpostorDisconnect : GameOverReason.HumansByVote, !SaveManager.BoughtNoAds);
				return;
			}
			DestroyableSingleton<HudManager>.Instance.ShowPopUp("Normally The Crew would have just won because The Impostor is dead. For free play, we revive everyone instead.");
			ShipStatus.ReviveEveryone();
			return;
		}
		else
		{
			if (num > num2)
			{
				if (!DestroyableSingleton<TutorialManager>.InstanceExists)
				{
					if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
					{
						base.enabled = false;
						ShipStatus.RpcEndGame(GameOverReason.HumansByTask, !SaveManager.BoughtNoAds);
						return;
					}
				}
				else if (PlayerControl.LocalPlayer.myTasks.All((PlayerTask t) => t.IsComplete))
				{
					DestroyableSingleton<HudManager>.Instance.ShowPopUp("Normally The Crew would have just won because the task bar is full. For free play, we issue new tasks instead.");
					this.Begin();
				}
				return;
			}
			if (!DestroyableSingleton<TutorialManager>.InstanceExists)
			{
				base.enabled = false;
				GameOverReason endReason;
				switch (GameData.Instance.LastDeathReason)
				{
				case DeathReason.Exile:
					endReason = GameOverReason.ImpostorByVote;
					break;
				case DeathReason.Kill:
					endReason = GameOverReason.ImpostorByKill;
					break;
				default:
					endReason = GameOverReason.HumansDisconnect;
					break;
				}
				ShipStatus.RpcEndGame(endReason, !SaveManager.BoughtNoAds);
				return;
			}
			DestroyableSingleton<HudManager>.Instance.ShowPopUp("Normally The Impostor would have just won because The Crew can no longer win. For free play, we revive everyone instead.");
			ShipStatus.ReviveEveryone();
			return;
		}
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x000085EC File Offset: 0x000067EC
	private void EndGameForSabotage()
	{
		if (!DestroyableSingleton<TutorialManager>.InstanceExists)
		{
			base.enabled = false;
			ShipStatus.RpcEndGame(GameOverReason.ImpostorBySabotage, !SaveManager.BoughtNoAds);
			return;
		}
		DestroyableSingleton<HudManager>.Instance.ShowPopUp("Normally The Impostor would have just won because of the critical sabotage. Instead we just shut it off.");
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x00036594 File Offset: 0x00034794
	public bool IsGameOverDueToDeath()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < GameData.Instance.PlayerCount; i++)
		{
			GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
			if (!playerInfo.Disconnected)
			{
				if (playerInfo.IsImpostor)
				{
					num3++;
				}
				if (!playerInfo.IsDead)
				{
					if (playerInfo.IsImpostor)
					{
						num2++;
					}
					else
					{
						num++;
					}
				}
			}
		}
		return (num2 <= 0 && (!DestroyableSingleton<TutorialManager>.InstanceExists || num3 > 0)) || num <= num2;
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x00036618 File Offset: 0x00034818
	private static void RpcEndGame(GameOverReason endReason, bool showAd)
	{
		MessageWriter messageWriter = AmongUsClient.Instance.StartEndGame();
		messageWriter.Write((byte)endReason);
		messageWriter.Write(showAd);
		AmongUsClient.Instance.FinishEndGame(messageWriter);
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x0003664C File Offset: 0x0003484C
	private static void ReviveEveryone()
	{
		for (int i = 0; i < GameData.Instance.PlayerCount; i++)
		{
			GameData.Instance.AllPlayers[i].Object.Revive();
		}
		UnityEngine.Object.FindObjectsOfType<DeadBody>().ForEach(delegate(DeadBody b)
		{
			UnityEngine.Object.Destroy(b.gameObject);
		});
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x000366B4 File Offset: 0x000348B4
	public bool CheckTaskCompletion()
	{
		if (DestroyableSingleton<TutorialManager>.InstanceExists)
		{
			if (PlayerControl.LocalPlayer.myTasks.All((PlayerTask t) => t.IsComplete))
			{
				DestroyableSingleton<HudManager>.Instance.ShowPopUp("Normally The Crew would have just won because the task bar is full. For free play, we issue new tasks instead.");
				this.Begin();
			}
			return false;
		}
		GameData.Instance.RecomputeTaskCounts();
		if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
		{
			base.enabled = false;
			ShipStatus.RpcEndGame(GameOverReason.HumansByTask, !SaveManager.BoughtNoAds);
			return true;
		}
		return false;
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x00036748 File Offset: 0x00034948
	public void RpcCloseDoorsOfType(SystemTypes type)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			this.CloseDoorsOfType(type);
			return;
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(this.NetId, 0, SendOption.Reliable, AmongUsClient.Instance.HostId);
		messageWriter.Write((byte)type);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x00036798 File Offset: 0x00034998
	public void RpcRepairSystem(SystemTypes systemType, int amount)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			this.RepairSystem(systemType, PlayerControl.LocalPlayer, (byte)amount);
			return;
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(this.NetId, 1, SendOption.Reliable, AmongUsClient.Instance.HostId);
		messageWriter.Write((byte)systemType);
		messageWriter.WriteNetObject(PlayerControl.LocalPlayer);
		messageWriter.Write((byte)amount);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x00036804 File Offset: 0x00034A04
	public override void HandleRpc(byte callId, MessageReader reader)
	{
		if (callId == 0)
		{
			this.CloseDoorsOfType((SystemTypes)reader.ReadByte());
			return;
		}
		if (callId != 1)
		{
			return;
		}
		this.RepairSystem((SystemTypes)reader.ReadByte(), reader.ReadNetObject<PlayerControl>(), reader.ReadByte());
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x00036840 File Offset: 0x00034A40
	internal void SelectSherrif()
	{
		List<GameData.PlayerInfo> list = (from pcd in GameData.Instance.AllPlayers
		where !pcd.Disconnected
		select pcd into pc
		where !pc.IsDead
		select pc into pci
		where !pci.IsImpostor
		select pci).ToList<GameData.PlayerInfo>();
		list.Shuffle<GameData.PlayerInfo>();
		GameData.PlayerInfo.Role[] roles = new GameData.PlayerInfo.Role[]
		{
			GameData.PlayerInfo.Role.Sheriff
		};
		GameData.PlayerInfo[] persons = list.Take(1).ToArray<GameData.PlayerInfo>();
		PlayerControl.LocalPlayer.RpcSetRole(persons, roles);
	}

	// Token: 0x04000A13 RID: 2579
	public static ShipStatus Instance;

	// Token: 0x04000A14 RID: 2580
	public Color CameraColor = Color.black;

	// Token: 0x04000A15 RID: 2581
	public float MaxLightRadius = 100f;

	// Token: 0x04000A16 RID: 2582
	public float MinLightRadius;

	// Token: 0x04000A17 RID: 2583
	public float MapScale = 4.4f;

	// Token: 0x04000A18 RID: 2584
	public Vector2 MapOffset = new Vector2(0.54f, 1.25f);

	// Token: 0x04000A19 RID: 2585
	public MapBehaviour MapPrefab;

	// Token: 0x04000A1A RID: 2586
	public Transform SpawnCenter;

	// Token: 0x04000A1B RID: 2587
	public float SpawnRadius = 1.55f;

	// Token: 0x04000A1C RID: 2588
	public AudioClip shipHum;

	// Token: 0x04000A1D RID: 2589
	public NormalPlayerTask[] CommonTasks;

	// Token: 0x04000A1E RID: 2590
	public NormalPlayerTask[] LongTasks;

	// Token: 0x04000A1F RID: 2591
	public NormalPlayerTask[] NormalTasks;

	// Token: 0x04000A20 RID: 2592
	public PlayerTask[] SpecialTasks;

	// Token: 0x04000A21 RID: 2593
	public AutoOpenDoor[] AllDoors;

	// Token: 0x04000A22 RID: 2594
	public global::Console[] AllConsoles;

	// Token: 0x04000A23 RID: 2595
	public Dictionary<SystemTypes, ISystemType> Systems;

	// Token: 0x04000A26 RID: 2598
	public AnimationClip[] WeaponFires;

	// Token: 0x04000A27 RID: 2599
	public SpriteAnim WeaponsImage;

	// Token: 0x04000A28 RID: 2600
	public AnimationClip HatchActive;

	// Token: 0x04000A29 RID: 2601
	public SpriteAnim Hatch;

	// Token: 0x04000A2A RID: 2602
	public ParticleSystem HatchParticles;

	// Token: 0x04000A2B RID: 2603
	public AnimationClip ShieldsActive;

	// Token: 0x04000A2C RID: 2604
	public SpriteAnim[] ShieldsImages;

	// Token: 0x04000A2D RID: 2605
	public SpriteRenderer ShieldBorder;

	// Token: 0x04000A2E RID: 2606
	public Sprite ShieldBorderOn;

	// Token: 0x04000A2F RID: 2607
	public SpriteRenderer MedScanner;

	// Token: 0x04000A30 RID: 2608
	private int WeaponFireIdx;

	// Token: 0x04000A31 RID: 2609
	public float Timer;

	// Token: 0x04000A32 RID: 2610
	private RaycastHit2D[] volumeBuffer = new RaycastHit2D[5];

	// Token: 0x020001E6 RID: 486
	public class SystemTypeComparer : IEqualityComparer<SystemTypes>
	{
		// Token: 0x06000A8F RID: 2703 RVA: 0x0000723B File Offset: 0x0000543B
		public bool Equals(SystemTypes x, SystemTypes y)
		{
			return x == y;
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x0000514A File Offset: 0x0000334A
		public int GetHashCode(SystemTypes obj)
		{
			return (int)obj;
		}

		// Token: 0x04000A33 RID: 2611
		public static readonly ShipStatus.SystemTypeComparer Instance = new ShipStatus.SystemTypeComparer();
	}

	// Token: 0x020001E7 RID: 487
	private enum RpcCalls
	{
		// Token: 0x04000A35 RID: 2613
		CloseDoorsOfType,
		// Token: 0x04000A36 RID: 2614
		RepairSystem
	}
}
