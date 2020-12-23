using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.CoreScripts;
using Hazel;
using InnerNet;
using PowerTools;
using UnityEngine;

public class PlayerControl : InnerNetObject
{
	public class ColliderComparer : IEqualityComparer<Collider2D>
	{
		public static readonly ColliderComparer Instance;

		public bool Equals(Collider2D x, Collider2D y)
		{
			return x == y;
		}

		public int GetHashCode(Collider2D obj)
		{
			return obj.GetInstanceID();
		}

		static ColliderComparer()
		{
			Instance = new ColliderComparer();
		}
	}

	public class UsableComparer : IEqualityComparer<IUsable>
	{
		public static readonly UsableComparer Instance;

		public bool Equals(IUsable x, IUsable y)
		{
			return x == y;
		}

		public int GetHashCode(IUsable obj)
		{
			return obj.GetHashCode();
		}

		static UsableComparer()
		{
			Instance = new UsableComparer();
		}
	}

	public enum RpcCalls : byte
	{
		PlayAnimation,
		CompleteTask,
		SyncSettings,
		SetInfected,
		Exiled,
		CheckName,
		SetName,
		CheckColor,
		SetColor,
		SetHat,
		SetSkin,
		ReportDeadBody,
		MurderPlayer,
		SendChat,
		TimesImpostor,
		StartMeeting,
		SetScanner
	}

	public byte PlayerId = byte.MaxValue;

	public float MaxReportDistance = 5f;

	public bool moveable = true;

	public bool inVent;

	public static PlayerControl LocalPlayer;

	private GameData.PlayerInfo _cachedData;

	public AudioSource FootSteps;

	public AudioClip KillSfx;

	public KillAnimation[] KillAnimations;

	[SerializeField]
	private float killTimer;

	public int RemainingEmergencies;

	public TextRenderer nameText;

	public LightSource LightPrefab;

	private LightSource myLight;

	[HideInInspector]
	public Collider2D Collider;

	[HideInInspector]
	public PlayerPhysics MyPhysics;

	[HideInInspector]
	public CustomNetworkTransform NetTransform;

	public SpriteRenderer HatRenderer;

	private SpriteRenderer myRend;

	private Collider2D[] hitBuffer = new Collider2D[20];

	public static GameOptionsData GameOptions;

	public List<PlayerTask> myTasks = new List<PlayerTask>();

	[NonSerialized]
	public uint TaskIdCount;

	public SpriteAnim[] ScannerAnims;

	public SpriteRenderer[] ScannersImages;

	public AudioClip[] VentMoveSounds;

	public AudioClip VentEnterSound;

	private IUsable closest;

	private bool isNew = true;

	public float percImpostor;

	public static List<PlayerControl> AllPlayerControls;

	private Dictionary<Collider2D, IUsable> cache = new Dictionary<Collider2D, IUsable>(ColliderComparer.Instance);

	private List<IUsable> itemsInRange = new List<IUsable>();

	private List<IUsable> newItemsInRange = new List<IUsable>();

	private byte scannerCount;

	public bool CanMove
	{
		get
		{
			if (moveable && !Minigame.Instance && (!DestroyableSingleton<HudManager>.InstanceExists || (!DestroyableSingleton<HudManager>.Instance.Chat.IsOpen && !DestroyableSingleton<HudManager>.Instance.KillOverlay.IsOpen && !DestroyableSingleton<HudManager>.Instance.GameMenu.IsOpen)) && (!MapBehaviour.Instance || !MapBehaviour.Instance.IsOpenStopped) && !MeetingHud.Instance)
			{
				return !IntroCutscene.Instance;
			}
			return false;
		}
	}

	public GameData.PlayerInfo Data
	{
		get
		{
			if (_cachedData == null)
			{
				if (!GameData.Instance)
				{
					return null;
				}
				_cachedData = GameData.Instance.GetPlayerById(PlayerId);
			}
			return _cachedData;
		}
	}

	public bool Visible
	{
		set
		{
			myRend.enabled = value;
			MyPhysics.Skin.Visible = value;
			HatRenderer.enabled = value;
			nameText.gameObject.SetActive(value);
		}
	}

	public void SetKillTimer(float time)
	{
		killTimer = time;
		if (GameOptions.KillCooldown > 0f)
		{
			DestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(killTimer, GameOptions.KillCooldown);
		}
		else
		{
			DestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(0f, GameOptions.KillCooldown);
		}
	}

	private void Awake()
	{
		myRend = GetComponent<SpriteRenderer>();
		MyPhysics = GetComponent<PlayerPhysics>();
		NetTransform = GetComponent<CustomNetworkTransform>();
		Collider = GetComponent<Collider2D>();
		AllPlayerControls.Add(this);
	}

	private void Start()
	{
		RemainingEmergencies = GameOptions.NumEmergencyMeetings;
		if (base.AmOwner)
		{
			myLight = UnityEngine.Object.Instantiate(LightPrefab);
			myLight.transform.SetParent(base.transform);
			myLight.transform.localPosition = Collider.offset;
			LocalPlayer = this;
			Camera.main.GetComponent<FollowerCamera>().SetTarget(this);
			SetName(SaveManager.PlayerName);
			SetColor(SaveManager.BodyColor);
			CmdCheckName(SaveManager.PlayerName);
			CmdCheckColor(SaveManager.BodyColor);
			RpcSetHat(SaveManager.LastHat);
			RpcSetSkin(SaveManager.LastSkin);
			RpcSetTimesImpostor(StatsManager.Instance.CrewmateStreak);
		}
		else
		{
			StartCoroutine(ClientInitialize());
		}
		if (isNew)
		{
			isNew = false;
			StartCoroutine(MyPhysics.CoSpawnPlayer(LobbyBehaviour.Instance));
		}
	}

	private IEnumerator ClientInitialize()
	{
		Visible = false;
		while (!GameData.Instance)
		{
			yield return null;
		}
		while (Data == null)
		{
			yield return null;
		}
		while (string.IsNullOrEmpty(Data.PlayerName))
		{
			yield return null;
		}
		SetName(Data.PlayerName);
		SetColor(Data.ColorId);
		SetHat(Data.HatId);
		SetSkin(Data.SkinId);
		Visible = true;
	}

	public override void OnDestroy()
	{
		AllPlayerControls.Remove(this);
		base.OnDestroy();
	}

	private void FixedUpdate()
	{
		if (!GameData.Instance)
		{
			return;
		}
		GameData.PlayerInfo data = Data;
		if (data == null)
		{
			return;
		}
		if (data.IsDead && (bool)LocalPlayer)
		{
			Visible = LocalPlayer.Data.IsDead;
		}
		if (!base.AmOwner)
		{
			return;
		}
		if ((bool)ShipStatus.Instance)
		{
			myLight.LightRadius = ShipStatus.Instance.CalculateLightRadius(data);
		}
		if ((data.IsImpostor || CE_RoleManager.GetRoleFromID(PlayerControl.LocalPlayer.Data.role).CanDo(CE_Specials.Kill)) && CanMove && !data.IsDead)
		{
			SetKillTimer(Mathf.Max(0f, killTimer - Time.fixedDeltaTime));
			PlayerControl target = FindClosestTarget();
			DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target);
		}
		else
		{
			DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
		}
		if (CanMove || inVent)
		{
			newItemsInRange.Clear();
			bool flag = (GameOptions.GhostsDoTasks || !data.IsDead) && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && CanMove;
			Vector2 truePosition = GetTruePosition();
			int num = Physics2D.OverlapCircleNonAlloc(truePosition, MaxReportDistance, hitBuffer, Constants.Usables);
			IUsable usable = null;
			float num2 = float.MaxValue;
			bool flag2 = false;
			for (int i = 0; i < num; i++)
			{
				Collider2D collider2D = hitBuffer[i];
				if (!cache.TryGetValue(collider2D, out var value))
				{
					IUsable usable2 = (cache[collider2D] = collider2D.GetComponent<IUsable>());
					value = usable2;
				}
				if (value != null && (flag || inVent))
				{
					bool canUse;
					bool couldUse;
					float num3 = value.CanUse(data, out canUse, out couldUse);
					if (canUse || couldUse)
					{
						newItemsInRange.Add(value);
					}
					if (canUse && num3 < num2)
					{
						num2 = num3;
						usable = value;
					}
				}
				if (flag && !data.IsDead && !flag2 && collider2D.tag == "DeadBody")
				{
					DeadBody component2 = collider2D.GetComponent<DeadBody>();
					if (!PhysicsHelpers.AnythingBetween(truePosition, component2.TruePosition, Constants.ShipAndObjectsMask, useTriggers: false))
					{
						flag2 = true;
					}
				}
			}
			for (int num4 = itemsInRange.Count - 1; num4 > -1; num4--)
			{
				IUsable item = itemsInRange[num4];
				int num5 = newItemsInRange.FindIndex((IUsable j) => j == item);
				if (num5 == -1)
				{
					item.SetOutline(on: false, mainTarget: false);
					itemsInRange.RemoveAt(num4);
				}
				else
				{
					newItemsInRange.RemoveAt(num5);
					item.SetOutline(on: true, usable == item);
				}
			}
			for (int k = 0; k < newItemsInRange.Count; k++)
			{
				IUsable usable3 = newItemsInRange[k];
				usable3.SetOutline(on: true, usable == usable3);
				itemsInRange.Add(usable3);
			}
			closest = usable;
			DestroyableSingleton<HudManager>.Instance.UseButton.SetTarget(usable);
			DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(flag2);
		}
		else
		{
			closest = null;
			DestroyableSingleton<HudManager>.Instance.UseButton.SetTarget(null);
			DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(isActive: false);
		}
	}

	public void UseClosest()
	{
		if (closest != null)
		{
			closest.Use();
		}
		closest = null;
		DestroyableSingleton<HudManager>.Instance.UseButton.SetTarget(null);
	}

	public void ReportClosest()
	{
		if (AmongUsClient.Instance.IsGameOver || LocalPlayer.Data.IsDead)
		{
			return;
		}
		Collider2D[] array = Physics2D.OverlapCircleAll(base.transform.position, MaxReportDistance, Constants.NotShipMask);
		foreach (Collider2D collider2D in array)
		{
			if (collider2D.tag != "DeadBody")
			{
				continue;
			}
			DeadBody component = collider2D.GetComponent<DeadBody>();
			if ((bool)component && !component.Reported)
			{
				component.OnClick();
				if (component.Reported)
				{
					break;
				}
			}
		}
	}

	public void PlayStepSound()
	{
		if (Constants.ShouldPlaySfx() && DestroyableSingleton<HudManager>.InstanceExists && LocalPlayer == this)
		{
			ShipRoom lastRoom = DestroyableSingleton<HudManager>.Instance.roomTracker.LastRoom;
			if ((bool)lastRoom && (bool)lastRoom.FootStepSounds)
			{
				AudioClip clip = lastRoom.FootStepSounds.Random();
				FootSteps.clip = clip;
				FootSteps.Play();
			}
		}
	}

	private void SetScanner(bool on, byte cnt)
	{
		if (!GameOptions.Visuals || cnt < scannerCount)
		{
			return;
		}
		scannerCount = cnt;
		for (int i = 0; i < ScannerAnims.Length; i++)
		{
			SpriteAnim spriteAnim = ScannerAnims[i];
			if (on && !Data.IsDead)
			{
				spriteAnim.gameObject.SetActive(value: true);
				spriteAnim.Play(null, 1f);
				ScannersImages[i].flipX = !myRend.flipX;
				continue;
			}
			if (spriteAnim.isActiveAndEnabled)
			{
				spriteAnim.Stop();
			}
			spriteAnim.gameObject.SetActive(value: false);
		}
	}

	public Vector2 GetTruePosition()
	{
		return (Vector2)base.transform.position + Collider.offset;
	}

	private PlayerControl FindClosestTarget()
	{
		PlayerControl result = null;
		float num = GameOptionsData.KillDistances[GameOptions.KillDistance];
		if (!ShipStatus.Instance)
		{
			return null;
		}
		Vector2 truePosition = GetTruePosition();
		List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
		for (int i = 0; i < allPlayers.Count; i++)
		{
			GameData.PlayerInfo playerInfo = allPlayers[i];
			if (playerInfo.Disconnected || playerInfo.PlayerId == PlayerId || playerInfo.IsDead || !ShouldKillImp(playerInfo) || playerInfo.Object.inVent)
			{
				continue;
			}
			PlayerControl @object = playerInfo.Object;
			if ((bool)@object)
			{
				Vector2 vector = @object.GetTruePosition() - truePosition;
				float magnitude = vector.magnitude;
				if (magnitude <= num && Physics2D.RaycastAll(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask).All((RaycastHit2D h) => h.collider.isTrigger))
				{
					result = @object;
					num = magnitude;
				}
			}
		}
		return result;
	}

	public void SetTasks(byte[] tasks)
	{
		StartCoroutine(CoSetTasks(tasks));
	}

	private IEnumerator CoSetTasks(byte[] tasks)
	{
		while (!ShipStatus.Instance)
		{
			yield return null;
		}
		myTasks.Clear();
		if (base.AmOwner)
		{
			DestroyableSingleton<HudManager>.Instance.TaskStuff.SetActive(value: true);
			StatsManager.Instance.GamesStarted++;
			if (Data.IsImpostor)
			{
				StatsManager.Instance.TimesImpostor++;
				StatsManager.Instance.CrewmateStreak = 0u;
				ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
				importantTextTask.transform.SetParent(base.transform, worldPositionStays: false);
				importantTextTask.Text = "Sabotage and kill everyone\r\n[FFFFFFFF]Fake Tasks:";
				myTasks.Add(importantTextTask);
			}
			else
			{
				StatsManager.Instance.TimesCrewmate++;
				StatsManager.Instance.CrewmateStreak++;
				if (!CE_RoleManager.GetRoleFromID(PlayerControl.LocalPlayer.Data.role).CanDo(CE_Specials.Kill))
				{
					DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(value: false);
				}
			}
			DestroyableSingleton<Telemetry>.Instance.StartGame(SaveManager.SendName, AmongUsClient.Instance.AmHost, GameData.Instance.PlayerCount, GameOptions.NumImpostors, AmongUsClient.Instance.GameMode, StatsManager.Instance.TimesImpostor, StatsManager.Instance.GamesStarted);
		}
		foreach (byte idx in tasks)
		{
			NormalPlayerTask normalPlayerTask = UnityEngine.Object.Instantiate(ShipStatus.Instance.GetTaskById(idx), base.transform);
			normalPlayerTask.Id = TaskIdCount++;
			normalPlayerTask.Owner = this;
			normalPlayerTask.Initialize();
			myTasks.Add(normalPlayerTask);
		}
	}

	public void AddSystemTask(SystemTypes system)
	{
		PlayerTask original;
		switch (system)
		{
		default:
			return;
		case SystemTypes.Reactor:
			original = ShipStatus.Instance.SpecialTasks[0];
			break;
		case SystemTypes.LifeSupp:
			original = ShipStatus.Instance.SpecialTasks[3];
			break;
		case SystemTypes.Electrical:
			original = ShipStatus.Instance.SpecialTasks[1];
			break;
		case SystemTypes.Comms:
			original = ShipStatus.Instance.SpecialTasks[2];
			break;
		}
		PlayerControl localPlayer = LocalPlayer;
		PlayerTask playerTask = UnityEngine.Object.Instantiate(original, localPlayer.transform);
		playerTask.Id = (byte)localPlayer.TaskIdCount++;
		playerTask.Initialize();
		localPlayer.myTasks.Add(playerTask);
	}

	public void RemoveTask(PlayerTask task)
	{
		task.OnRemove();
		myTasks.Remove(task);
		GameData.Instance.TutOnlyRemoveTask(PlayerId, task.Id);
		DestroyableSingleton<HudManager>.Instance.UseButton.SetTarget(null);
		UnityEngine.Object.Destroy(task.gameObject);
	}

	private void ClearTasks()
	{
		for (int i = 0; i < myTasks.Count; i++)
		{
			PlayerTask playerTask = myTasks[i];
			playerTask.OnRemove();
			UnityEngine.Object.Destroy(playerTask.gameObject);
		}
		myTasks.Clear();
	}

	public void RemoveInfected()
	{
		GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(PlayerId);
		if (playerById.IsImpostor)
		{
			playerById.Object.nameText.Color = Color.white;
			playerById.IsImpostor = false;
			myTasks.RemoveAt(0);
			DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(value: false);
		}
	}

	public void Die(DeathReason reason)
	{
		GameData.Instance.LastDeathReason = reason;
		Data.IsDead = true;
		base.gameObject.layer = LayerMask.NameToLayer("Ghost");
		nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
		if (base.AmOwner)
		{
			SaveManager.LastGameStart = DateTime.MinValue;
			DestroyableSingleton<HudManager>.Instance.Chat.SetVisible(visible: true);
		}
	}

	public void Revive()
	{
		Data.IsDead = false;
		base.gameObject.layer = LayerMask.NameToLayer("Players");
		MyPhysics.ResetAnim();
		nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 4);
		if (base.AmOwner)
		{
			DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(Data.IsImpostor || CE_RoleManager.GetRoleFromID(PlayerControl.LocalPlayer.Data.role).CanDo(CE_Specials.Kill));
			DestroyableSingleton<HudManager>.Instance.Chat.ForceClosed();
			DestroyableSingleton<HudManager>.Instance.Chat.SetVisible(visible: false);
		}
	}

	public void PlayAnimation(byte animType)
	{
		switch (animType)
		{
		case 6:
			ShipStatus.Instance.FireWeapon();
			break;
		case 1:
			ShipStatus.Instance.StartShields();
			break;
		case 9:
		case 10:
			ShipStatus.Instance.OpenHatch();
			break;
		}
	}

	public void CompleteTask(uint idx)
	{
		PlayerTask playerTask = myTasks.Find((PlayerTask p) => p.Id == idx);
		if ((bool)playerTask)
		{
			GameData.Instance.CompleteTask(this, idx);
			playerTask.Complete();
			DestroyableSingleton<Telemetry>.Instance.WriteCompleteTask(PlayerId, playerTask.TaskType);
		}
		else
		{
			Debug.LogWarning(PlayerId + ": Server didn't have task: " + idx);
		}
	}

	public void SetInfected(GameData.PlayerInfo[] infected)
	{
		killTimer = 20f;
		for (int i = 0; i < infected.Length; i++)
		{
			infected[i].IsImpostor = true;
		}
		DestroyableSingleton<HudManager>.Instance.MapButton.gameObject.SetActive(value: true);
		DestroyableSingleton<HudManager>.Instance.ReportButton.gameObject.SetActive(value: true);
		GameData.PlayerInfo data = LocalPlayer.Data;
		LocalPlayer.RemainingEmergencies = GameOptions.NumEmergencyMeetings;
		if (data.IsImpostor)
		{
			DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(value: true);
			if (GameOptions.Gamemode == 1)
			{
				for (int j = 0; j < infected.Length; j++)
				{
					infected[j].Object.nameText.Color = Palette.InfectedGreen;
				}
			}
			else
			{
				for (int k = 0; k < infected.Length; k++)
				{
					infected[k].Object.nameText.Color = Palette.ImpostorRed;
				}
			}
		}
		if (!DestroyableSingleton<TutorialManager>.InstanceExists)
		{
			List<PlayerControl> yourTeam = ((!data.IsImpostor) ? (from pcd in GameData.Instance.AllPlayers
				where !pcd.Disconnected
				select pcd.Object into pc
				orderby (!(pc == LocalPlayer)) ? 1 : 0
				select pc).ToList() : (from pcd in GameData.Instance.AllPlayers
				where !pcd.Disconnected
				where pcd.IsImpostor
				select pcd.Object into pc
				orderby (!(pc == LocalPlayer)) ? 1 : 0
				select pc).ToList());
			StopAllCoroutines();
			DestroyableSingleton<HudManager>.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoShowIntro(yourTeam));
		}
	}

	public void Exiled()
	{
		Die(DeathReason.Exile);
		if (!base.AmOwner)
		{
			return;
		}
		StatsManager.Instance.TimesEjected++;
		DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(value: false);
		if (!GameOptions.GhostsDoTasks)
		{
			ClearTasks();
			GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(PlayerId);
			ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
			importantTextTask.transform.SetParent(base.transform, worldPositionStays: false);
			if (playerById.IsImpostor)
			{
				importantTextTask.Text = "You are dead, but you can still sabotage.";
			}
			else
			{
				importantTextTask.Text = "You are dead, but finish your tasks anyway.";
			}
			myTasks.Add(importantTextTask);
		}
	}

	public void LateUpdate()
    {
		for (int i = 0; i < AllPlayerControls.Count; i++)
        {
			bool noHatBobbing = DestroyableSingleton<HatManager>.Instance.GetHatById(AllPlayerControls[i].Data.HatId).NoBobbing;
			if (noHatBobbing)
			{
				string name = AllPlayerControls[i].myRend.sprite.name;
				AllPlayerControls[i].HatRenderer.transform.localPosition = CE_WardrobeLoader.SetHatBobingPhysics(AllPlayerControls[i], name, AllPlayerControls[i].HatRenderer.transform.localPosition);
			}
		}
    }

	public void CheckName(string name)
	{
		List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
		if (allPlayers.Any((GameData.PlayerInfo i) => i.PlayerId != PlayerId && i.PlayerName.Equals(name, StringComparison.OrdinalIgnoreCase)))
		{
			for (int j = 1; j < 100; j++)
			{
				string text = name + " " + j;
				bool flag = false;
				for (int k = 0; k < allPlayers.Count; k++)
				{
					if (allPlayers[k].PlayerId != PlayerId && allPlayers[k].PlayerName.Equals(text, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					name = text;
					break;
				}
			}
		}
		RpcSetName(name);
		GameData.Instance.UpdateName(PlayerId, name);
	}

	public void SetName(string name)
	{
		if ((bool)GameData.Instance)
		{
			GameData.Instance.UpdateName(PlayerId, name);
		}
		base.gameObject.name = name;
		nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 4);
		nameText.Text = name;
	}

	public void CheckColor(byte bodyColor)
	{
		List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
		int num = 0;
		while (allPlayers.Any((GameData.PlayerInfo p) => !p.Disconnected && p.PlayerId != PlayerId && p.ColorId == bodyColor) && num++ < 100)
		{
			bodyColor = (byte)((bodyColor + 1) % Palette.PlayerColors.Length);
		}
		RpcSetColor(bodyColor);
	}

	public void SetHatAlpha(float a)
	{
		Color white = Color.white;
		white.a = a;
		HatRenderer.color = white;
	}

	public void SetColor(byte bodyColor)
	{
		if ((bool)GameData.Instance)
		{
			GameData.Instance.UpdateColor(PlayerId, bodyColor);
		}
		if ((object)myRend == null)
		{
			GetComponent<SpriteRenderer>();
		}
		SetPlayerMaterialColors(bodyColor, myRend);
	}

	public void SetSkin(uint skinId)
	{
		if ((bool)GameData.Instance)
		{
			GameData.Instance.UpdateSkin(PlayerId, skinId);
		}
		MyPhysics.SetSkin(skinId);
	}

	public void SetHat(uint hatId)
	{
		if ((bool)GameData.Instance)
		{
			GameData.Instance.UpdateHat(PlayerId, hatId);
		}
		SetHatImage(hatId, HatRenderer);
		nameText.transform.localPosition = new Vector3(0f, (hatId == 0) ? 0.7f : 1.05f, -0.5f);
	}

	public static void SetSkinImage(uint skinId, SpriteRenderer target)
	{
		if (DestroyableSingleton<HatManager>.InstanceExists)
		{
			SetSkinImage(DestroyableSingleton<HatManager>.Instance.GetSkinById(skinId), target);
		}
	}

	public static void SetSkinImage(SkinData skin, SpriteRenderer target)
	{
		target.sprite = skin.IdleFrame;
	}

	public static void SetHatImage(uint hatId, SpriteRenderer target)
	{
		if (DestroyableSingleton<HatManager>.InstanceExists)
		{
			SetHatImage(DestroyableSingleton<HatManager>.Instance.GetHatById(hatId), target);
		}
	}

	public static void SetHatImage(HatBehaviour hat, SpriteRenderer target)
	{
		if ((bool)target && (bool)hat)
		{
			target.sprite = hat.MainImage;
			Vector3 localPosition = target.transform.localPosition; 
			localPosition.z = (hat.InFront ? (-0.0001f) : 0.0001f);
			target.transform.localPosition = localPosition;
		}
		else
		{
			string str = ((!target) ? "null" : target.name);
			string str2 = ((!hat) ? "null" : hat.name);
			Debug.LogError("Player: " + str + "\tHat: " + str2);
		}
	}

	public void SendChat(string chatText)
	{
		if ((bool)DestroyableSingleton<HudManager>.Instance)
		{
			DestroyableSingleton<HudManager>.Instance.Chat.AddChat(this, chatText);
		}
	}

	private void ReportDeadBody(GameData.PlayerInfo target)
	{
		if (!AmongUsClient.Instance.IsGameOver && !MeetingHud.Instance && !Data.IsDead)
		{
			MeetingRoomManager.Instance.AssignSelf(this, target);
			if (AmongUsClient.Instance.AmHost && !ShipStatus.Instance.CheckTaskCompletion())
			{
				DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(this);
				RpcStartMeeting(target);
			}
		}
	}

	public IEnumerator CoStartMeeting(GameData.PlayerInfo target)
	{
		DestroyableSingleton<Telemetry>.Instance.WriteMeetingStarted(target == null);
		while (!MeetingHud.Instance)
		{
			yield return null;
		}
		MeetingRoomManager.Instance.RemoveSelf();
		DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i].gameObject);
		}
		for (int j = 0; j < AllPlayerControls.Count; j++)
		{
			PlayerControl playerControl = AllPlayerControls[j];
			if (!playerControl.GetComponent<DummyBehaviour>().enabled)
			{
				playerControl.MyPhysics.ExitAllVents();
				playerControl.NetTransform.SnapTo(ShipStatus.Instance.GetSpawnLocation(playerControl.PlayerId, GameData.Instance.PlayerCount));
			}
		}
		if (base.AmOwner)
		{
			if (target != null)
			{
				StatsManager.Instance.BodiesReported++;
			}
			else
			{
				StatsManager.Instance.EmergenciesCalled++;
			}
		}
		if ((bool)MapBehaviour.Instance)
		{
			MapBehaviour.Instance.Close();
		}
		if ((bool)Minigame.Instance)
		{
			Minigame.Instance.Close();
		}
		KillAnimation.SetMovement(this, canMove: true);
		MeetingHud.Instance.StartCoroutine(MeetingHud.Instance.CoIntro(this, target));
	}

	public void MurderPlayer(PlayerControl target)
	{
		if (!target || AmongUsClient.Instance.IsGameOver)
		{
			return;
		}
		GameData.PlayerInfo data = target.Data;
		if (data.IsDead)
		{
			return;
		}
		if (base.AmOwner)
		{
			StatsManager.Instance.ImpostorKills++;
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(LocalPlayer.KillSfx, loop: false, 0.8f);
			}
		}
		SetKillTimer(GameOptions.KillCooldown);
		DestroyableSingleton<Telemetry>.Instance.WriteMurder(PlayerId, target.PlayerId, target.transform.position);
		if (target.AmOwner)
		{
			StatsManager.Instance.TimesMurdered++;
			if ((bool)Minigame.Instance)
			{
				Minigame.Instance.Close();
				Minigame.Instance.Close();
			}
			if (GameOptions.Gamemode == 1)
			{
				ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
				importantTextTask.transform.SetParent(base.transform, worldPositionStays: false);
				importantTextTask.Text = "You are infected.";
				target.myTasks.Add(importantTextTask);
				List<GameData.PlayerInfo> list = new List<GameData.PlayerInfo>();
				foreach (GameData.PlayerInfo allPlayer in GameData.Instance.AllPlayers)
				{
					if (allPlayer.IsImpostor)
					{
						list.Add(allPlayer);
					}
				}
				list.Add(target.Data);
				foreach (PlayerControl allPlayerControl in AllPlayerControls)
				{
					_ = allPlayerControl;
					target.RpcSetInfectedNoIntro(list.ToArray());
				}
			}
			if (GameOptions.Gamemode != 1)
			{
				DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(value: false);
				base.gameObject.layer = 12;
				nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
				DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowOne(this, data);
				target.RpcSetScanner(value: false);
				if (!GameOptions.GhostsDoTasks)
				{
					target.ClearTasks();
					ImportantTextTask importantTextTask2 = new GameObject("_Player").AddComponent<ImportantTextTask>();
					importantTextTask2.transform.SetParent(base.transform, worldPositionStays: false);
					importantTextTask2.Text = "You're dead, enjoy the chaos";
					target.myTasks.Add(importantTextTask2);
				}
			}
		}
		MyPhysics.StartCoroutine(KillAnimations.Random().CoPerformKill(this, target));
	}

	public override bool Serialize(MessageWriter writer, bool initialState)
	{
		if (initialState)
		{
			writer.Write(isNew);
		}
		writer.Write(PlayerId);
		return true;
	}

	public override void Deserialize(MessageReader reader, bool initialState)
	{
		if (initialState)
		{
			isNew = reader.ReadBoolean();
		}
		PlayerId = reader.ReadByte();
	}

	public void SetPlayerMaterialColors(Renderer rend)
	{
		SetPlayerMaterialColors(GameData.Instance.GetPlayerById(PlayerId)?.ColorId ?? 0, rend);
	}

	public static void SetPlayerMaterialColors(int colorId, Renderer rend)
	{
		rend.material.SetColor("_BackColor", Palette.ShadowColors[colorId]);
		rend.material.SetColor("_BodyColor", Palette.PlayerColors[colorId]);
		switch (colorId)
		{
		case 21:
			rend.material.SetColor("_VisorColor", Palette.VisorColorRed);
			break;
		case 22:
			rend.material.SetColor("_VisorColor", Palette.VisorColorGreen);
			break;
		case 23:
			rend.material.SetColor("_VisorColor", Palette.VisorColorCarJemGenerations);
			break;
		default:
			rend.material.SetColor("_VisorColor", Palette.VisorColor);
			break;
		}
	}

	public void RpcSetScanner(bool value)
	{
		byte b = ++scannerCount;
		if (AmongUsClient.Instance.AmClient)
		{
			SetScanner(value, b);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 16);
		messageWriter.Write(value);
		messageWriter.Write(b);
		messageWriter.EndMessage();
	}

	public void RpcPlayAnimation(byte animType)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			PlayAnimation(animType);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 0, SendOption.None);
		messageWriter.Write(animType);
		messageWriter.EndMessage();
	}

	public void RpcCompleteTask(uint idx, byte id)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			CompleteTask(idx);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 1);
		messageWriter.WritePacked(idx);
		messageWriter.Write(id);
		messageWriter.EndMessage();
	}

	public void RpcSendUpdate(byte id)
    {
		if (AmongUsClient.Instance.AmClient && AmongUsClient.Instance.AmHost)
        {
			CE_LuaLoader.GetGamemodeResult("OnHostRecieveSimple", id); //no need to check if the gamemode we are in is lua, this packet can't be sent if it isn't
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 254);
		messageWriter.Write(AmongUsClient.Instance.GameId);
		messageWriter.Write(id);
		messageWriter.EndMessage();
	}

	public void RpcSyncSettings(GameOptionsData gameOptions)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			GameOptions = gameOptions;
			SaveManager.GameHostOptions = gameOptions;
			MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 2);
			messageWriter.WriteBytesAndSize(gameOptions.ToBytes());
			messageWriter.EndMessage();
		}
	}

	public void RpcSetInfected(GameData.PlayerInfo[] infected)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			SetInfected(infected);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 3);
		messageWriter.WritePacked(infected.Length);
		for (int i = 0; i < infected.Length; i++)
		{
			messageWriter.Write(infected[i].PlayerId);
		}
		messageWriter.EndMessage();
	}

	public void CmdCheckName(string name)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			CheckName(name);
			return;
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(NetId, 5, SendOption.Reliable, AmongUsClient.Instance.HostId);
		messageWriter.Write(name);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
	}

	public void RpcSetSkin(uint skinId)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			SetSkin(skinId);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 10);
		messageWriter.WritePacked(skinId);
		messageWriter.EndMessage();
	}

	public void RpcSetHat(uint hatId)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			SetHat(hatId);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 9);
		messageWriter.WritePacked(hatId);
		messageWriter.EndMessage();
	}

	public void RpcSetName(string name)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			SetName(name);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 6);
		messageWriter.Write(name);
		messageWriter.EndMessage();
	}

	public void CmdCheckColor(byte bodyColor)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			CheckColor(bodyColor);
			return;
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(NetId, 7, SendOption.Reliable, AmongUsClient.Instance.HostId);
		messageWriter.Write(bodyColor);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
	}

	public void RpcSetColor(byte bodyColor)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			SetColor(bodyColor);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 8);
		messageWriter.Write(bodyColor);
		messageWriter.EndMessage();
	}

	public void RpcSetTimesImpostor(float percImpostor)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.percImpostor = percImpostor;
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 14, SendOption.None);
		messageWriter.Write(percImpostor);
		messageWriter.EndMessage();
	}

	public bool RpcSendChat(string chatText)
	{
		if (string.IsNullOrWhiteSpace(chatText))
		{
			return false;
		}
		if (AmongUsClient.Instance.AmClient)
		{
			SendChat(chatText);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(NetId, 13, SendOption.Reliable);
		messageWriter.Write(chatText);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
		return true;
	}

	public void CmdReportDeadBody(GameData.PlayerInfo target)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			ReportDeadBody(target);
			return;
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 11);
		messageWriter.Write(target?.PlayerId ?? byte.MaxValue);
		messageWriter.EndMessage();
	}

	public void RpcStartMeeting(GameData.PlayerInfo info)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			StartCoroutine(CoStartMeeting(info));
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(NetId, 15, SendOption.Reliable);
		messageWriter.Write(info?.PlayerId ?? byte.MaxValue);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
	}

	public void RpcMurderPlayer(PlayerControl target)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			MurderPlayer(target);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(NetId, 12, SendOption.Reliable);
		messageWriter.WriteNetObject(target);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
	}

	public override void HandleRpc(byte callId, MessageReader reader)
	{
		switch (callId)
		{
		case 0:
			PlayAnimation(reader.ReadByte());
			break;
		case 1:
			CompleteTask(reader.ReadPackedUInt32());
			if (CE_LuaLoader.CurrentGMLua && AmongUsClient.Instance.AmHost)
			{
					GameData.PlayerInfo plf = GameData.Instance.GetPlayerById(reader.ReadByte());
					bool shouldcontinue = CE_LuaLoader.GetGamemodeResult("OnTaskCompletionHost", plf.Object.myTasks.Count, PlayerTask.HowManyTasksCompleted(plf.Object), new CE_PlayerInfoLua(plf)).Boolean; //just run it
			}
			break;
			case 254:
				int gameid = reader.ReadInt32();
				if (AmongUsClient.Instance.GameId != gameid || !AmongUsClient.Instance.AmHost)
				{
					break;
				}
				byte id = reader.ReadByte();
				CE_LuaLoader.GetGamemodeResult("OnHostRecieveSimple", id); //no need to check if the gamemode we are in is lua, this packet can't be sent if it isn't
				break;
			case 2:
			GameOptions = GameOptionsData.FromBytes(reader.ReadBytesAndSize());
			break;
		case 3:
			int num2 = reader.ReadPackedInt32();
			GameData.PlayerInfo[] array3 = new GameData.PlayerInfo[num2];
			for (int j = 0; j < num2; j++)
			{
				array3[j] = GameData.Instance.GetPlayerById(reader.ReadByte());
			}
			SetInfected(array3);
			break;
		case 4:
			Exiled();
			break;
		case 5:
			CheckName(reader.ReadString());
			break;
		case 6:
			SetName(reader.ReadString());
			break;
		case 7:
			CheckColor(reader.ReadByte());
			break;
		case 8:
			SetColor(reader.ReadByte());
			break;
		case 9:
			SetHat(reader.ReadPackedUInt32());
			break;
		case 10:
			SetSkin(reader.ReadPackedUInt32());
			break;
		case 11:
		{
			GameData.PlayerInfo playerById2 = GameData.Instance.GetPlayerById(reader.ReadByte());
			ReportDeadBody(playerById2);
			break;
		}
		case 12:
		{
			PlayerControl target = reader.ReadNetObject<PlayerControl>();
			MurderPlayer(target);
			break;
		}
		case 13:
			string chatmessage = reader.ReadString();
			SendChat(chatmessage);
			break;
		case 14:
			percImpostor = reader.ReadSingle();
			break;
		case 15:
		{
			GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(reader.ReadByte());
			StartCoroutine(CoStartMeeting(playerById));
			break;
		}
		case 16:
			SetScanner(reader.ReadBoolean(), reader.ReadByte());
			break;
		case 17:
		{
			int num3 = reader.ReadPackedInt32();
			GameData.PlayerInfo[] array4 = new GameData.PlayerInfo[num3];
			for (int k = 0; k < num3; k++)
			{
				array4[k] = GameData.Instance.GetPlayerById(reader.ReadByte());
			}
			SetInfectedNoIntro(array4);
			break;
		}
		case 18:
		{
			int num = reader.ReadPackedInt32();
			GameData.PlayerInfo[] array = new GameData.PlayerInfo[num];
			byte[] array2 = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = GameData.Instance.GetPlayerById(reader.ReadByte());
				array2[i] = reader.ReadByte();
			}
			SetRoles(array, array2);
			break;
		}
		}
	}

	static PlayerControl()
	{
		GameOptions = new GameOptionsData();
		AllPlayerControls = new List<PlayerControl>();
	}

	public void SetInfectedNoIntro(GameData.PlayerInfo[] infected)
	{
		killTimer = GameOptions.KillCooldown;
		for (int i = 0; i < infected.Length; i++)
		{
			infected[i].IsImpostor = true;
		}
		DestroyableSingleton<HudManager>.Instance.MapButton.gameObject.SetActive(value: true);
		DestroyableSingleton<HudManager>.Instance.ReportButton.gameObject.SetActive(value: true);
		if (!LocalPlayer.Data.IsImpostor)
		{
			return;
		}
		DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(value: true);
		if (GameOptions.Gamemode == 1)
		{
			for (int j = 0; j < infected.Length; j++)
			{
				infected[j].Object.nameText.Color = Palette.InfectedGreen;
			}
		}
		else
		{
			for (int k = 0; k < infected.Length; k++)
			{
				infected[k].Object.nameText.Color = Palette.ImpostorRed;
			}
		}
	}

	public void RpcSetInfectedNoIntro(GameData.PlayerInfo[] infected)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			SetInfectedNoIntro(infected);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 17);
		messageWriter.WritePacked(infected.Length);
		for (int i = 0; i < infected.Length; i++)
		{
			messageWriter.Write(infected[i].PlayerId);
		}
		messageWriter.EndMessage();
	}

	public void RpcSetRole(GameData.PlayerInfo[] persons, byte[] roles)
	{
		Debug.Log("Setting roles(IF THE IMPOSTOR SOMEHOW TRIGGERS THIS I WILL DIE)");
		if (AmongUsClient.Instance.AmClient)
		{
			SetRoles(persons, roles);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 18);
		messageWriter.WritePacked(persons.Length);
		for (int i = 0; i < persons.Length; i++)
		{
			messageWriter.Write(persons[i].PlayerId);
			messageWriter.Write((byte)roles[i]);
		}
		messageWriter.EndMessage();
	}

	public void SetRoles(GameData.PlayerInfo[] players, byte[] roles)
	{
		for (int i = 0; i < players.Length; i++)
		{
            players[i].role = roles[i];
			if (players[i].role != 0)
			{
				CE_Role playerrole = CE_RoleManager.GetRoleFromID(players[i].role);
				if (playerrole.CanSee(LocalPlayer.Data))
				{
					players[i].Object.nameText.Color = playerrole.RoleColor;
				}
			}
			else
            {
				if (!players[i].IsImpostor)
                {
					players[i].Object.nameText.Color = Palette.White;
				}
			}
		}
		CE_Role selfrole = CE_RoleManager.GetRoleFromID(LocalPlayer.Data.role);
		if (selfrole.CanDo(CE_Specials.Kill) || LocalPlayer.Data.IsImpostor)
		{
			DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(value: true);
		}
		else
        {
			DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(value: false);
		}
		if (LocalPlayer.Data.role != 0)
		{
			LocalPlayer.nameText.Color = selfrole.RoleColor;
		}
	}

	private bool ShouldKillImp(GameData.PlayerInfo playerinfo)
	{
		if (CE_LuaLoader.CurrentGMLua)
		{
			return CE_LuaLoader.GetGamemodeResult("CanKill", new CE_PlayerInfoLua(Data), new CE_PlayerInfoLua(playerinfo)).Boolean;
		}
		return !playerinfo.IsImpostor;
	}
}
