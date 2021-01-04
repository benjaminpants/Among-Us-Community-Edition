using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using InnerNet;
using MoonSharp.Interpreter;
using PowerTools;
using UnityEngine;

public class ShipStatus : InnerNetObject
{
	public class SystemTypeComparer : IEqualityComparer<SystemTypes>
	{
		public static readonly SystemTypeComparer Instance;

		public bool Equals(SystemTypes x, SystemTypes y)
		{
			return x == y;
		}

		public int GetHashCode(SystemTypes obj)
		{
			return (int)obj;
		}

		static SystemTypeComparer()
		{
			Instance = new SystemTypeComparer();
		}
	}

	private enum RpcCalls
	{
		CloseDoorsOfType,
		RepairSystem
	}

	public static ShipStatus Instance;

	public Color CameraColor = Color.black;

	public float MaxLightRadius = 100f;

	public float MinLightRadius;

	public float MapScale = 4.4f;

	public Vector2 MapOffset = new Vector2(0.54f, 1.25f);

	public MapBehaviour MapPrefab;

	public Transform SpawnCenter;

	public float SpawnRadius = 1.55f;

	public AudioClip shipHum;

	public NormalPlayerTask[] CommonTasks;

	public NormalPlayerTask[] LongTasks;

	public NormalPlayerTask[] NormalTasks;

	public PlayerTask[] SpecialTasks;

	public AutoOpenDoor[] AllDoors;

	public Console[] AllConsoles;

	public Dictionary<SystemTypes, ISystemType> Systems;

	public AnimationClip[] WeaponFires;

	public SpriteAnim WeaponsImage;

	public AnimationClip HatchActive;

	public SpriteAnim Hatch;

	public ParticleSystem HatchParticles;

	public AnimationClip ShieldsActive;

	public SpriteAnim[] ShieldsImages;

	public SpriteRenderer ShieldBorder;

	public Sprite ShieldBorderOn;

	public SpriteRenderer MedScanner;

	private int WeaponFireIdx;

	public float Timer;

	public float TimeSinceLastRound;

	private RaycastHit2D[] volumeBuffer = new RaycastHit2D[5];

	public ShipRoom[] AllRooms
	{
		get;
		private set;
	}

	public Vent[] AllVents
	{
		get;
		private set;
	}

	public ShipStatus()
	{
		Systems = new Dictionary<SystemTypes, ISystemType>(SystemTypeComparer.Instance)
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
		Systems.Add(SystemTypes.Sabotage, new SabotageSystemType(new IActivatable[4]
		{
			(IActivatable)Systems[SystemTypes.Comms],
			(IActivatable)Systems[SystemTypes.Reactor],
			(IActivatable)Systems[SystemTypes.LifeSupp],
			(IActivatable)Systems[SystemTypes.Electrical]
		}));
	}

	private void Awake()
	{
		CE_CustomMap.MapTest(this);
		AllRooms = GetComponentsInChildren<ShipRoom>();
		AllConsoles = GetComponentsInChildren<Console>();
		AllVents = GetComponentsInChildren<Vent>();
		AssignTaskIndexes();
		Instance = this;
	}

	public void Start()
	{
		Camera.main.backgroundColor = CameraColor;
		if (DestroyableSingleton<HudManager>.InstanceExists)
		{
			DestroyableSingleton<HudManager>.Instance.Chat.ForceClosed();
			DestroyableSingleton<HudManager>.Instance.Chat.SetVisible(visible: false);
			DestroyableSingleton<HudManager>.Instance.GameSettings.gameObject.SetActive(value: false);
		}
		DeconSystem componentInChildren = GetComponentInChildren<DeconSystem>();
		if ((bool)componentInChildren)
		{
			Systems.Add(SystemTypes.Decontamination, componentInChildren);
		}
		LobbyBehaviour instance = LobbyBehaviour.Instance;
		if ((bool)instance)
		{
			UnityEngine.Object.Destroy(instance.gameObject);
		}
		SoundManager.Instance.StopAllSound();
		AudioSource audioSource = SoundManager.Instance.PlaySound(shipHum, loop: true);
		if ((bool)audioSource)
		{
			audioSource.pitch = 0.8f;
		}
		if (!Constants.ShouldPlaySfx())
		{
			return;
		}
		for (int i = 0; i < AllRooms.Length; i++)
		{
			ShipRoom room = AllRooms[i];
			if ((bool)room.AmbientSound)
			{
				SoundManager.Instance.PlayDynamicSound("Amb " + room.RoomId, room.AmbientSound, loop: true, delegate(AudioSource player, float dt)
				{
					GetAmbientSoundVolume(room, player, dt);
				});
			}
		}
	}

	public override void OnDestroy()
	{
		SoundManager.Instance.StopAllSound();
		base.OnDestroy();
	}

	public Vector2 GetSpawnLocation(int playerId, int numPlayer)
	{
		Vector2 up = Vector2.up;
		up = up.Rotate((float)(playerId - 1) * (360f / (float)numPlayer));
		up *= SpawnRadius;
		return (Vector2)SpawnCenter.position + up + new Vector2(0f, 0.3636f);
	}

	public void StartShields()
	{
		if (PlayerControl.GameOptions.Visuals)
		{
			for (int i = 0; i < ShieldsImages.Length; i++)
			{
				ShieldsImages[i].Play(ShieldsActive);
			}
			ShieldBorder.sprite = ShieldBorderOn;
		}
	}

	public void FireWeapon()
	{
		if (PlayerControl.GameOptions.Visuals && !WeaponsImage.IsPlaying())
		{
			WeaponsImage.Play(WeaponFires[WeaponFireIdx]);
			WeaponFireIdx = (WeaponFireIdx + 1) % 2;
		}
	}

	public NormalPlayerTask GetTaskById(byte idx)
	{
		return CommonTasks.FirstOrDefault((NormalPlayerTask t) => t.Index == idx) ?? LongTasks.FirstOrDefault((NormalPlayerTask t) => t.Index == idx) ?? NormalTasks.FirstOrDefault((NormalPlayerTask t) => t.Index == idx);
	}

	public void OpenHatch()
	{
		if (PlayerControl.GameOptions.Visuals && !Hatch.IsPlaying())
		{
			Hatch.Play(HatchActive);
			HatchParticles.Play();
		}
	}

	public void CloseDoorsOfType(SystemTypes room)
	{
		(Systems[SystemTypes.Doors] as DoorsSystemType).CloseDoorsOfType(room);
		SetDirtyBit(65536u);
	}

	public void RepairSystem(SystemTypes systemType, PlayerControl player, byte amount)
	{
		Systems[systemType].RepairDamage(player, amount);
		SetDirtyBit((uint)(1 << (int)systemType));
	}

	internal void SelectInfected()
	{
		if (GameOptionsData.GamemodesAreLua[PlayerControl.GameOptions.Gamemode])
		{

			List<GameData.PlayerInfo> listrole = (from pcd in GameData.Instance.AllPlayers
											  where !pcd.Disconnected
											  select pcd into pc
											  where !pc.IsDead
											  select pc).ToList();
			List<CE_PlayerInfoLua> list2role = new List<CE_PlayerInfoLua>();
			foreach (GameData.PlayerInfo item in listrole)
			{
				list2role.Add(new CE_PlayerInfoLua(item));
			}
			List<GameData.PlayerInfo> list3role = new List<GameData.PlayerInfo>();
			List<byte> Roles = new List<byte>();
			Table RolesTable = CE_LuaLoader.GetGamemodeResult("DecideRoles", list2role).Table;
            foreach (DynValue value in RolesTable.Get(1).Table.Values)
            {
                CE_PlayerInfoLua cE_PlayerInfoLua = (CE_PlayerInfoLua)value.UserData.Object;
                list3role.Add(cE_PlayerInfoLua.refplayer);
            }
			foreach (DynValue value in RolesTable.Get(2).Table.Values)
			{
				Roles.Add(CE_RoleManager.GetRoleFromUUID(value.String));
			}
			PlayerControl.LocalPlayer.RpcSetRole(list3role.ToArray(),Roles.ToArray());


			List<GameData.PlayerInfo> list = (from pcd in GameData.Instance.AllPlayers
				where !pcd.Disconnected
				select pcd into pc
				where !pc.IsDead
				select pc into pcx
				where pcx.role == 0
				select pcx).ToList();
			List<CE_PlayerInfoLua> list2 = new List<CE_PlayerInfoLua>();
			foreach (GameData.PlayerInfo item in list)
			{
				list2.Add(new CE_PlayerInfoLua(item));
			}
			List<GameData.PlayerInfo> list3 = new List<GameData.PlayerInfo>();
			foreach (DynValue value in CE_LuaLoader.GetGamemodeResult("DecideImpostors", PlayerControl.GameOptions.NumImpostors, list2).Table.Values)
			{
				CE_PlayerInfoLua cE_PlayerInfoLua = (CE_PlayerInfoLua)value.UserData.Object;
				list3.Add(cE_PlayerInfoLua.refplayer);
			}
			PlayerControl.LocalPlayer.RpcSetInfected(list3.ToArray());
		}
		else
		{
			List<GameData.PlayerInfo> list4 = (from pcd in GameData.Instance.AllPlayers
				where !pcd.Disconnected
				select pcd into pc
				where !pc.IsDead
				select pc into pcx
				where pcx.role == 0
				select pcx).ToList();
			list4.Shuffle();
			GameData.PlayerInfo[] infected = list4.Take(PlayerControl.GameOptions.NumImpostors).ToArray();
			PlayerControl.LocalPlayer.RpcSetInfected(infected);
		}
	}

	private void AssignTaskIndexes()
	{
		int num = 0;
		for (int i = 0; i < CommonTasks.Length; i++)
		{
			CommonTasks[i].Index = num++;
		}
		for (int j = 0; j < LongTasks.Length; j++)
		{
			LongTasks[j].Index = num++;
		}
		for (int k = 0; k < NormalTasks.Length; k++)
		{
			NormalTasks[k].Index = num++;
		}
	}

	public void Begin()
	{
		AssignTaskIndexes();
		GameOptionsData gameOptions = PlayerControl.GameOptions;
		List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
		HashSet<TaskTypes> hashSet = new HashSet<TaskTypes>();
		List<byte> list = new List<byte>(10);
		List<NormalPlayerTask> list2 = CommonTasks.ToList();
		list2.Shuffle();
		int start = 0;
		AddTasksFromList(ref start, gameOptions.NumCommonTasks, list, hashSet, list2);
		for (int i = 0; i < gameOptions.NumCommonTasks; i++)
		{
			if (list2.Count == 0)
			{
				Debug.LogWarning("Not enough common tasks");
				break;
			}
			int index = list2.RandomIdx();
			list.Add((byte)list2[index].Index);
			list2.RemoveAt(index);
		}
		List<NormalPlayerTask> list3 = LongTasks.ToList();
		list3.Shuffle();
		List<NormalPlayerTask> list4 = NormalTasks.ToList();
		list4.Shuffle();
		int start2 = 0;
		int start3 = 0;
		for (byte b = 0; b < allPlayers.Count; b = (byte)(b + 1))
		{
			hashSet.Clear();
			list.RemoveRange(gameOptions.NumCommonTasks, list.Count - gameOptions.NumCommonTasks);
			AddTasksFromList(ref start2, gameOptions.NumLongTasks, list, hashSet, list3);
			AddTasksFromList(ref start3, gameOptions.NumShortTasks, list, hashSet, list4);
			GameData.PlayerInfo playerInfo = allPlayers[b];
			if ((bool)playerInfo.Object && !playerInfo.Object.GetComponent<DummyBehaviour>().enabled)
			{
				byte[] taskTypeIds = list.ToArray();
				bool alreadyassigned = false;
				if (CE_LuaLoader.CurrentGMLua)
                {
					if (!CE_LuaLoader.GetGamemodeResult("GiveTasks",new CE_PlayerInfoLua(playerInfo)).Boolean)
                    {
						alreadyassigned = true;
						GameData.Instance.RpcSetTasks(playerInfo.PlayerId, new byte[0]);
					}
                }
				if (!alreadyassigned)
				{
					GameData.Instance.RpcSetTasks(playerInfo.PlayerId, taskTypeIds);
				}
			}
		}
		base.enabled = true;
	}

	private void AddTasksFromList(ref int start, int count, List<byte> tasks, HashSet<TaskTypes> usedTaskTypes, List<NormalPlayerTask> unusedTasks)
	{
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			if (num++ == 1000)
			{
				break;
			}
			if (start >= unusedTasks.Count)
			{
				start = 0;
				unusedTasks.Shuffle();
				if (unusedTasks.All((NormalPlayerTask t) => usedTaskTypes.Contains(t.TaskType)))
				{
					Debug.Log("Not enough task types");
					usedTaskTypes.Clear();
				}
			}
			NormalPlayerTask normalPlayerTask = unusedTasks[start++];
			if (!usedTaskTypes.Add(normalPlayerTask.TaskType))
			{
				i--;
			}
			else
			{
				tasks.Add((byte)normalPlayerTask.Index);
			}
		}
	}

	public void FixedUpdate()
	{
		if (!AmongUsClient.Instance)
		{
			return;
		}
		Timer += Time.fixedDeltaTime;
		TimeSinceLastRound += Time.fixedDeltaTime;
        if (CE_LuaLoader.CurrentGMLua && AmongUsClient.Instance.AmHost)
        {
            CE_LuaLoader.GetGamemodeResult("OnHostUpdate", Timer, TimeSinceLastRound);
        }

		if (CE_LuaLoader.CurrentGMLua)
		{
			CE_LuaLoader.GetGamemodeResult("OnClientUpdate", Timer, TimeSinceLastRound);
		}
		if (Timer > 75f && SaveManager.LastGameStart != DateTime.MinValue)
		{
			SaveManager.LastGameStart = DateTime.MinValue;
		}
		if ((bool)GameData.Instance)
		{
			GameData.Instance.RecomputeTaskCounts();
		}
		if (AmongUsClient.Instance.AmHost)
		{
			CheckEndCriteria();
		}
		if (!AmongUsClient.Instance.AmClient)
		{
			return;
		}
		for (int i = 0; i < SystemTypeHelpers.AllTypes.Length; i++)
		{
			SystemTypes systemTypes = SystemTypeHelpers.AllTypes[i];
			if (Systems.TryGetValue(systemTypes, out var value) && value.Detoriorate(Time.fixedDeltaTime))
			{
				SetDirtyBit((uint)(1 << (int)systemTypes));
			}
		}
	}

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
		int num2 = Physics2D.RaycastNonAlloc(vector, direction, volumeBuffer, num, Constants.ShipOnlyMask);
		float num3 = 1f - num / 8f - (float)num2 * 0.25f;
		player.volume = Mathf.Lerp(player.volume, num3 * 0.7f, dt);
	}

	public float CalculateLightRadius(GameData.PlayerInfo player)
	{
		if (player.IsDead)
		{
			return MaxLightRadius;
		}
		SwitchSystem switchSystem = (SwitchSystem)Systems[SystemTypes.Electrical];
		if (player.IsImpostor || CE_RoleManager.GetRoleFromID(player.role).UseImpVision)
		{
			return MaxLightRadius * PlayerControl.GameOptions.ImpostorLightMod;
		}
		float t = (float)(int)switchSystem.Value / 255f;
		return Mathf.Lerp(MinLightRadius, MaxLightRadius, t) * PlayerControl.GameOptions.CrewLightMod;
	}

	public override bool Serialize(MessageWriter writer, bool initialState)
	{
		if (initialState)
		{
			(Systems[SystemTypes.Doors] as DoorsSystemType).SetDoors(AllDoors);
			for (short num = 0; num < SystemTypeHelpers.AllTypes.Length; num = (short)(num + 1))
			{
				SystemTypes key = SystemTypeHelpers.AllTypes[num];
				if (Systems.TryGetValue(key, out var value))
				{
					value.Serialize(writer, initialState: true);
				}
			}
			return true;
		}
		if (DirtyBits != 0)
		{
			writer.WritePacked(DirtyBits);
			for (short num2 = 0; num2 < SystemTypeHelpers.AllTypes.Length; num2 = (short)(num2 + 1))
			{
				SystemTypes systemTypes = SystemTypeHelpers.AllTypes[num2];
				if ((DirtyBits & (1 << (int)systemTypes)) != 0L && Systems.TryGetValue(systemTypes, out var value2))
				{
					value2.Serialize(writer, initialState: false);
				}
			}
			DirtyBits = 0u;
			return true;
		}
		return false;
	}

	public override void Deserialize(MessageReader reader, bool initialState)
	{
		if (initialState)
		{
			(Systems[SystemTypes.Doors] as DoorsSystemType).SetDoors(AllDoors);
			for (short num = 0; num < SystemTypeHelpers.AllTypes.Length; num = (short)(num + 1))
			{
				SystemTypes key = (SystemTypes)num;
				if (Systems.TryGetValue(key, out var value))
				{
					value.Deserialize(reader, initialState: true);
				}
			}
			return;
		}
		uint num2 = reader.ReadPackedUInt32();
		for (short num3 = 0; num3 < SystemTypeHelpers.AllTypes.Length; num3 = (short)(num3 + 1))
		{
			SystemTypes systemTypes = SystemTypeHelpers.AllTypes[num3];
			if ((num2 & (1 << (int)systemTypes)) != 0L && Systems.TryGetValue(systemTypes, out var value2))
			{
				value2.Deserialize(reader, initialState: false);
			}
		}
	}

	private void CheckEndCriteria()
	{
		if (!GameData.Instance)
		{
			return;
		}
		bool currentGMLua = CE_LuaLoader.CurrentGMLua;
		bool flag = false;
		bool flag2 = false;
		LifeSuppSystemType lifeSuppSystemType = (LifeSuppSystemType)Systems[SystemTypes.LifeSupp];
		if (lifeSuppSystemType.Countdown < 0f)
		{
			if (!currentGMLua)
			{
				EndGameForSabotage();
			}
			else
			{
				flag = true;
			}
			lifeSuppSystemType.Countdown = 10000f;
			RpcRepairSystem(SystemTypes.LifeSupp, 0);
		}
		ReactorSystemType reactorSystemType = (ReactorSystemType)Systems[SystemTypes.Reactor];
		if (reactorSystemType.Countdown < 0f)
		{
			if (!currentGMLua)
			{
				EndGameForSabotage();
			}
			else
			{
				flag = true;
			}
            reactorSystemType.Countdown = 10000f;
			RpcRepairSystem(SystemTypes.Reactor, 0);
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
        List<CE_PlayerInfoLua> imps = new List<CE_PlayerInfoLua>();
        List<CE_PlayerInfoLua> crew = new List<CE_PlayerInfoLua>();
		List<CE_PlayerInfoLua> all = new List<CE_PlayerInfoLua>();
		for (int i = 0; i < GameData.Instance.PlayerCount; i++)
		{
			GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
			if (playerInfo.Disconnected)
			{
				continue;
			}
			if (playerInfo.IsImpostor)
			{
				num3++; //this variable is literally never used so
			}
			if (!playerInfo.IsDead)
			{
				if (playerInfo.IsImpostor)
				{
					num2++;
                    imps.Add(new CE_PlayerInfoLua(playerInfo));
					all.Add(new CE_PlayerInfoLua(playerInfo));
				}
				else
				{
                    num++;
                    crew.Add(new CE_PlayerInfoLua(playerInfo));
					all.Add(new CE_PlayerInfoLua(playerInfo));
				}
			}
		}
		if (currentGMLua)
		{
			if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
			{
				flag2 = true;
			}
			DynValue output = CE_LuaLoader.GetGamemodeResult("CheckWinCondition", imps, crew, flag, flag2);
			if (output.Type == DataType.String)
			{
				string winnerstring = output.String;
				if (winnerstring == "impostors")
				{
					base.enabled = false;
					RpcEndGame(GameOverReason.ImpostorByVote, !SaveManager.BoughtNoAds);
				}
				else if (winnerstring == "crewmates")
				{
					base.enabled = false;
					RpcEndGame(GameOverReason.HumansByVote, !SaveManager.BoughtNoAds);
				}
			}
			else if (output.Type == DataType.Table) //itsa custom win condition!!!
            {
				Debug.Log("wintable");
                Table winnertable = output.Table;
				Debug.Log("actualwintable");
                Table actualwinnertable = winnertable.Get(1).Table;
				Debug.Log("get winnerinfo");
				List<GameData.PlayerInfo> winnerinfo = new List<GameData.PlayerInfo>();
                foreach (DynValue dyn in actualwinnertable.Values)
                {
                    CE_PlayerInfoLua plyinfo = (CE_PlayerInfoLua)dyn.UserData.Object;
                    winnerinfo.Add(plyinfo.refplayer);
                }
				Debug.Log("send");
                RpcCustomEndGame(winnerinfo.ToArray(), winnertable.Get(2).String);
				Debug.Log("complete");

			}
			else
            {
				Debug.LogError("unknown datatype: " + Enum.GetName(typeof(DataType), output.Type));
			}
		}
		else if (num2 <= 0 && (!DestroyableSingleton<TutorialManager>.InstanceExists || num3 > 0))
		{
			if (!DestroyableSingleton<TutorialManager>.InstanceExists)
			{
				base.enabled = false;
				RpcEndGame((GameData.Instance.LastDeathReason == DeathReason.Disconnect) ? GameOverReason.ImpostorDisconnect : GameOverReason.HumansByVote, !SaveManager.BoughtNoAds);
			}
			else
			{
				DestroyableSingleton<HudManager>.Instance.ShowPopUp("Normally The Crew would have just won because The Impostor is dead. For free play, we revive everyone instead.");
				ReviveEveryone();
			}
		}
		else if (num > num2)
		{
			if (!DestroyableSingleton<TutorialManager>.InstanceExists)
			{
				if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
				{
					base.enabled = false;
					RpcEndGame(GameOverReason.HumansByTask, !SaveManager.BoughtNoAds);
				}
			}
			else if (PlayerControl.LocalPlayer.myTasks.All((PlayerTask t) => t.IsComplete))
			{
				DestroyableSingleton<HudManager>.Instance.ShowPopUp("Normally The Crew would have just won because the task bar is full. For free play, we issue new tasks instead.");
				Begin();
			}
		}
		else if (!DestroyableSingleton<TutorialManager>.InstanceExists)
		{
			base.enabled = false;
			RpcEndGame(GameData.Instance.LastDeathReason switch
			{
				DeathReason.Kill => GameOverReason.ImpostorByKill, 
				DeathReason.Exile => GameOverReason.ImpostorByVote, 
				_ => GameOverReason.HumansDisconnect, 
			}, !SaveManager.BoughtNoAds);
		}
		else
		{
			DestroyableSingleton<HudManager>.Instance.ShowPopUp("Normally The Impostor would have just won because The Crew can no longer win. For free play, we revive everyone instead.");
			ReviveEveryone();
		}
	}

	private void EndGameForSabotage()
	{
		if (!DestroyableSingleton<TutorialManager>.InstanceExists)
		{
			base.enabled = false;
			RpcEndGame(GameOverReason.ImpostorBySabotage, !SaveManager.BoughtNoAds);
		}
		else
		{
			DestroyableSingleton<HudManager>.Instance.ShowPopUp("Normally The Impostor would have just won because of the critical sabotage. Instead we just shut it off.");
		}
	}

	public bool IsGameOverDueToDeath()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < GameData.Instance.PlayerCount; i++)
		{
			GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
			if (playerInfo.Disconnected)
			{
				continue;
			}
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
		if (CE_LuaLoader.CurrentGMLua)
		{
			return false; //hopefully this won't cause problems
		}
		if (num2 <= 0 && (!DestroyableSingleton<TutorialManager>.InstanceExists || num3 > 0))
		{
			return true;
		}
		if (num <= num2)
		{
			return true;
		}
		return false;
	}

    public static void RpcEndGame(GameOverReason endReason, bool showAd)
    {
        MessageWriter messageWriter = AmongUsClient.Instance.StartEndGame();
        messageWriter.Write((byte)endReason);
        messageWriter.Write(showAd);
        AmongUsClient.Instance.FinishEndGame(messageWriter);
    }

	private static void RpcCustomEndGame(GameData.PlayerInfo[] plrs, string song)
	{
		MessageWriter messageWriter = AmongUsClient.Instance.StartCustomEndGame();
		messageWriter.WritePacked(plrs.Length);
		for (int i = 0; i < plrs.Length; i++)
		{
			messageWriter.Write(plrs[i].PlayerId);
		}
		messageWriter.Write(song); //write the song's filename relative to CE_Assets/Audio/CustomWins
		AmongUsClient.Instance.FinishEndGame(messageWriter);
	}

	private static void WriteRPCObject(CE_LuaSpawnableObject obj)
    {
		if (AmongUsClient.Instance.AmHost)
		{
			MessageWriter messageWriter = AmongUsClient.Instance.StartSendObject();
			messageWriter.Write(obj.ID);
			messageWriter.Write(obj.Position.x);
			messageWriter.Write(obj.Position.y);
			AmongUsClient.Instance.FinishEndGame(messageWriter);
			Debug.Log("Sent Object to Server/Host!");
		}
	}

    public static void RpcCustomEndGamePublic(GameData.PlayerInfo[] plrs, string song)
    {
        RpcCustomEndGame(plrs, song);
    }

	public static void WriteRPCObjectPublic(CE_LuaSpawnableObject obj)
	{
		WriteRPCObject(obj);
	}

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

	public bool CheckTaskCompletion()
	{
		if (DestroyableSingleton<TutorialManager>.InstanceExists)
		{
			if (PlayerControl.LocalPlayer.myTasks.All((PlayerTask t) => t.IsComplete))
			{
				DestroyableSingleton<HudManager>.Instance.ShowPopUp("Normally The Crew would have just won because the task bar is full. For free play, we issue new tasks instead.");
				Begin();
			}
			return false;
		}
		GameData.Instance.RecomputeTaskCounts();
		if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
		{
			base.enabled = false;
			RpcEndGame(GameOverReason.HumansByTask, !SaveManager.BoughtNoAds);
			return true;
		}
		return false;
	}

	public void RpcCloseDoorsOfType(SystemTypes type)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			CloseDoorsOfType(type);
			return;
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(NetId, 0, SendOption.Reliable, AmongUsClient.Instance.HostId);
		messageWriter.Write((byte)type);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
	}

	public void RpcRepairSystem(SystemTypes systemType, int amount)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			RepairSystem(systemType, PlayerControl.LocalPlayer, (byte)amount);
			return;
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(NetId, 1, SendOption.Reliable, AmongUsClient.Instance.HostId);
		messageWriter.Write((byte)systemType);
		messageWriter.WriteNetObject(PlayerControl.LocalPlayer);
		messageWriter.Write((byte)amount);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
	}

	public override void HandleRpc(byte callId, MessageReader reader)
	{
		switch (callId)
		{
		case 0:
			CloseDoorsOfType((SystemTypes)reader.ReadByte());
			break;
		case 1:
			RepairSystem((SystemTypes)reader.ReadByte(), reader.ReadNetObject<PlayerControl>(), reader.ReadByte());
			break;
		}
	}


	public void JokerWin()
	{
		if ((bool)GameData.Instance)
		{
			RpcEndGame(GameOverReason.JokerEjected, showAd: false);
		}
	}

}
