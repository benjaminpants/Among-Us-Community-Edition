using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.CoreScripts;
using Hazel;
using InnerNet;
using PowerTools;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class PlayerControl : InnerNetObject
{
	// Token: 0x17000133 RID: 307
	// (get) Token: 0x060007E5 RID: 2021 RVA: 0x0002C868 File Offset: 0x0002AA68
	public bool CanMove
	{
		get
		{
			return this.moveable && !Minigame.Instance && (!DestroyableSingleton<HudManager>.InstanceExists || (!DestroyableSingleton<HudManager>.Instance.Chat.IsOpen && !DestroyableSingleton<HudManager>.Instance.KillOverlay.IsOpen && !DestroyableSingleton<HudManager>.Instance.GameMenu.IsOpen)) && (!MapBehaviour.Instance || !MapBehaviour.Instance.IsOpenStopped) && !MeetingHud.Instance && !IntroCutscene.Instance;
		}
	}

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x060007E6 RID: 2022 RVA: 0x00006DC6 File Offset: 0x00004FC6
	public GameData.PlayerInfo Data
	{
		get
		{
			if (this._cachedData == null)
			{
				if (!GameData.Instance)
				{
					return null;
				}
				this._cachedData = GameData.Instance.GetPlayerById(this.PlayerId);
			}
			return this._cachedData;
		}
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0002C8F8 File Offset: 0x0002AAF8
	public void SetKillTimer(float time)
	{
		this.killTimer = time;
		if (PlayerControl.GameOptions.KillCooldown > 0f)
		{
			DestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(this.killTimer, PlayerControl.GameOptions.KillCooldown);
			return;
		}
		DestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(0f, PlayerControl.GameOptions.KillCooldown);
	}

	// Token: 0x17000135 RID: 309
	// (set) Token: 0x060007E8 RID: 2024 RVA: 0x00006DFA File Offset: 0x00004FFA
	public bool Visible
	{
		set
		{
			this.myRend.enabled = value;
			this.MyPhysics.Skin.Visible = value;
			this.HatRenderer.enabled = value;
			this.nameText.gameObject.SetActive(value);
		}
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x00006E36 File Offset: 0x00005036
	private void Awake()
	{
		this.myRend = base.GetComponent<SpriteRenderer>();
		this.MyPhysics = base.GetComponent<PlayerPhysics>();
		this.NetTransform = base.GetComponent<CustomNetworkTransform>();
		this.Collider = base.GetComponent<Collider2D>();
		PlayerControl.AllPlayerControls.Add(this);
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x0002C95C File Offset: 0x0002AB5C
	private void Start()
	{
		this.RemainingEmergencies = PlayerControl.GameOptions.NumEmergencyMeetings;
		if (base.AmOwner)
		{
			this.myLight = UnityEngine.Object.Instantiate<LightSource>(this.LightPrefab);
			this.myLight.transform.SetParent(base.transform);
			this.myLight.transform.localPosition = this.Collider.offset;
			PlayerControl.LocalPlayer = this;
			Camera.main.GetComponent<FollowerCamera>().SetTarget(this);
			this.SetName(SaveManager.PlayerName);
			this.SetColor(SaveManager.BodyColor);
			this.CmdCheckName(SaveManager.PlayerName);
			this.CmdCheckColor(SaveManager.BodyColor);
			this.RpcSetHat(SaveManager.LastHat);
			this.RpcSetSkin(SaveManager.LastSkin);
			this.RpcSetTimesImpostor(StatsManager.Instance.CrewmateStreak);
		}
		else
		{
			base.StartCoroutine(this.ClientInitialize());
		}
		if (this.isNew)
		{
			this.isNew = false;
			base.StartCoroutine(this.MyPhysics.CoSpawnPlayer(LobbyBehaviour.Instance));
		}
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x00006E73 File Offset: 0x00005073
	private IEnumerator ClientInitialize()
	{
		this.Visible = false;
		while (!GameData.Instance)
		{
			yield return null;
		}
		while (this.Data == null)
		{
			yield return null;
		}
		while (string.IsNullOrEmpty(this.Data.PlayerName))
		{
			yield return null;
		}
		this.SetName(this.Data.PlayerName);
		this.SetColor(this.Data.ColorId);
		this.SetHat(this.Data.HatId);
		this.SetSkin(this.Data.SkinId);
		this.Visible = true;
		yield break;
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x00006E82 File Offset: 0x00005082
	public override void OnDestroy()
	{
		PlayerControl.AllPlayerControls.Remove(this);
		base.OnDestroy();
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x0002CA6C File Offset: 0x0002AC6C
	private void FixedUpdate()
	{
		if (!GameData.Instance)
		{
			return;
		}
		GameData.PlayerInfo data = this.Data;
		if (data == null)
		{
			return;
		}
		if (data.IsDead && PlayerControl.LocalPlayer)
		{
			this.Visible = PlayerControl.LocalPlayer.Data.IsDead;
		}
		if (base.AmOwner)
		{
			if (ShipStatus.Instance)
			{
				this.myLight.LightRadius = ShipStatus.Instance.CalculateLightRadius(data);
			}
			if ((data.IsImpostor || data.role == GameData.PlayerInfo.Role.Sheriff) && this.CanMove && !data.IsDead)
			{
				this.SetKillTimer(Mathf.Max(0f, this.killTimer - Time.fixedDeltaTime));
				PlayerControl target = this.FindClosestTarget();
				DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target);
			}
			else
			{
				DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
			}
			if (this.CanMove || this.inVent)
			{
				this.newItemsInRange.Clear();
				bool flag = (PlayerControl.GameOptions.GhostsDoTasks || !data.IsDead) && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && this.CanMove;
				Vector2 truePosition = this.GetTruePosition();
				int num = Physics2D.OverlapCircleNonAlloc(truePosition, this.MaxReportDistance, this.hitBuffer, Constants.Usables);
				IUsable usable = null;
				float num2 = float.MaxValue;
				bool flag2 = false;
				for (int i = 0; i < num; i++)
				{
					Collider2D collider2D = this.hitBuffer[i];
					IUsable usable2;
					if (!this.cache.TryGetValue(collider2D, out usable2))
					{
						usable2 = (this.cache[collider2D] = collider2D.GetComponent<IUsable>());
					}
					if (usable2 != null && (flag || this.inVent))
					{
						bool flag3;
						bool flag4;
						float num3 = usable2.CanUse(data, out flag3, out flag4);
						if (flag3 || flag4)
						{
							this.newItemsInRange.Add(usable2);
						}
						if (flag3 && num3 < num2)
						{
							num2 = num3;
							usable = usable2;
						}
					}
					if (flag && !data.IsDead && !flag2 && collider2D.tag == "DeadBody")
					{
						DeadBody component = collider2D.GetComponent<DeadBody>();
						if (!PhysicsHelpers.AnythingBetween(truePosition, component.TruePosition, Constants.ShipAndObjectsMask, false))
						{
							flag2 = true;
						}
					}
				}
				for (int l = this.itemsInRange.Count - 1; l > -1; l--)
				{
					IUsable item = this.itemsInRange[l];
					int num4 = this.newItemsInRange.FindIndex((IUsable j) => j == item);
					if (num4 == -1)
					{
						item.SetOutline(false, false);
						this.itemsInRange.RemoveAt(l);
					}
					else
					{
						this.newItemsInRange.RemoveAt(num4);
						item.SetOutline(true, usable == item);
					}
				}
				for (int k = 0; k < this.newItemsInRange.Count; k++)
				{
					IUsable usable3 = this.newItemsInRange[k];
					usable3.SetOutline(true, usable == usable3);
					this.itemsInRange.Add(usable3);
				}
				this.closest = usable;
				DestroyableSingleton<HudManager>.Instance.UseButton.SetTarget(usable);
				DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(flag2);
				return;
			}
			this.closest = null;
			DestroyableSingleton<HudManager>.Instance.UseButton.SetTarget(null);
			DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(false);
		}
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x00006E96 File Offset: 0x00005096
	public void UseClosest()
	{
		if (this.closest != null)
		{
			this.closest.Use();
		}
		this.closest = null;
		DestroyableSingleton<HudManager>.Instance.UseButton.SetTarget(null);
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x0002CDD8 File Offset: 0x0002AFD8
	public void ReportClosest()
	{
		if (AmongUsClient.Instance.IsGameOver)
		{
			return;
		}
		if (PlayerControl.LocalPlayer.Data.IsDead)
		{
			return;
		}
		foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(base.transform.position, this.MaxReportDistance, Constants.NotShipMask))
		{
			if (!(collider2D.tag != "DeadBody"))
			{
				DeadBody component = collider2D.GetComponent<DeadBody>();
				if (component && !component.Reported)
				{
					component.OnClick();
					if (component.Reported)
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0002CE70 File Offset: 0x0002B070
	public void PlayStepSound()
	{
		if (!Constants.ShouldPlaySfx())
		{
			return;
		}
		if (DestroyableSingleton<HudManager>.InstanceExists && PlayerControl.LocalPlayer == this)
		{
			ShipRoom lastRoom = DestroyableSingleton<HudManager>.Instance.roomTracker.LastRoom;
			if (lastRoom && lastRoom.FootStepSounds)
			{
				AudioClip clip = lastRoom.FootStepSounds.Random();
				this.FootSteps.clip = clip;
				this.FootSteps.Play();
			}
		}
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x0002CEE4 File Offset: 0x0002B0E4
	private void SetScanner(bool on, byte cnt)
	{
		if (!PlayerControl.GameOptions.Visuals)
		{
			return;
		}
		if (cnt < this.scannerCount)
		{
			return;
		}
		this.scannerCount = cnt;
		for (int i = 0; i < this.ScannerAnims.Length; i++)
		{
			SpriteAnim spriteAnim = this.ScannerAnims[i];
			if (on && !this.Data.IsDead)
			{
				spriteAnim.gameObject.SetActive(true);
				spriteAnim.Play(null, 1f);
				this.ScannersImages[i].flipX = !this.myRend.flipX;
			}
			else
			{
				if (spriteAnim.isActiveAndEnabled)
				{
					spriteAnim.Stop();
				}
				spriteAnim.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x00006EC2 File Offset: 0x000050C2
	public Vector2 GetTruePosition()
	{
		return (Vector2)base.transform.position + this.Collider.offset;
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x0002CF8C File Offset: 0x0002B18C
	private PlayerControl FindClosestTarget()
	{
		PlayerControl result = null;
		float num = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
		if (!ShipStatus.Instance)
		{
			return null;
		}
		Vector2 truePosition = this.GetTruePosition();
		List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
		for (int i = 0; i < allPlayers.Count; i++)
		{
			GameData.PlayerInfo playerInfo = allPlayers[i];
			if (!playerInfo.Disconnected && playerInfo.PlayerId != this.PlayerId && !playerInfo.IsDead && this.ShouldKillImp(playerInfo) && !playerInfo.Object.inVent)
			{
				PlayerControl @object = playerInfo.Object;
				if (@object)
				{
					Vector2 vector = @object.GetTruePosition() - truePosition;
					float magnitude = vector.magnitude;
					if (magnitude <= num)
					{
						if (Physics2D.RaycastAll(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask).All((RaycastHit2D h) => h.collider.isTrigger))
						{
							result = @object;
							num = magnitude;
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x00006EE4 File Offset: 0x000050E4
	public void SetTasks(byte[] tasks)
	{
		base.StartCoroutine(this.CoSetTasks(tasks));
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x00006EF4 File Offset: 0x000050F4
	private IEnumerator CoSetTasks(byte[] tasks)
	{
		while (!ShipStatus.Instance)
		{
			yield return null;
		}
		this.myTasks.Clear();
		if (base.AmOwner)
		{
			DestroyableSingleton<HudManager>.Instance.TaskStuff.SetActive(true);
			StatsManager instance = StatsManager.Instance;
			uint num = instance.GamesStarted;
			instance.GamesStarted = num + 1U;
			if (this.Data.IsImpostor)
			{
				StatsManager instance2 = StatsManager.Instance;
				num = instance2.TimesImpostor;
				instance2.TimesImpostor = num + 1U;
				StatsManager.Instance.CrewmateStreak = 0U;
				ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
				importantTextTask.transform.SetParent(base.transform, false);
				importantTextTask.Text = "Sabotage and kill everyone\r\n[FFFFFFFF]Fake Tasks:";
				this.myTasks.Add(importantTextTask);
			}
			else
			{
				StatsManager instance3 = StatsManager.Instance;
				num = instance3.TimesCrewmate;
				instance3.TimesCrewmate = num + 1U;
				StatsManager instance4 = StatsManager.Instance;
				num = instance4.CrewmateStreak;
				instance4.CrewmateStreak = num + 1U;
				if (this.Data.role != GameData.PlayerInfo.Role.Sheriff)
				{
					DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
				}
			}
			DestroyableSingleton<Telemetry>.Instance.StartGame(SaveManager.SendName, AmongUsClient.Instance.AmHost, GameData.Instance.PlayerCount, PlayerControl.GameOptions.NumImpostors, AmongUsClient.Instance.GameMode, StatsManager.Instance.TimesImpostor, StatsManager.Instance.GamesStarted);
		}
		foreach (byte idx in tasks)
		{
			NormalPlayerTask normalPlayerTask = UnityEngine.Object.Instantiate<NormalPlayerTask>(ShipStatus.Instance.GetTaskById(idx), base.transform);
			PlayerTask playerTask = normalPlayerTask;
			uint taskIdCount = this.TaskIdCount;
			this.TaskIdCount = taskIdCount + 1U;
			playerTask.Id = taskIdCount;
			normalPlayerTask.Owner = this;
			normalPlayerTask.Initialize();
			this.myTasks.Add(normalPlayerTask);
		}
		yield break;
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0002D0A4 File Offset: 0x0002B2A4
	public void AddSystemTask(SystemTypes system)
	{
		PlayerTask original;
		if (system <= SystemTypes.Electrical)
		{
			if (system != SystemTypes.Reactor)
			{
				if (system != SystemTypes.Electrical)
				{
					return;
				}
				original = ShipStatus.Instance.SpecialTasks[1];
			}
			else
			{
				original = ShipStatus.Instance.SpecialTasks[0];
			}
		}
		else if (system != SystemTypes.LifeSupp)
		{
			if (system != SystemTypes.Comms)
			{
				return;
			}
			original = ShipStatus.Instance.SpecialTasks[2];
		}
		else
		{
			original = ShipStatus.Instance.SpecialTasks[3];
		}
		PlayerControl localPlayer = PlayerControl.LocalPlayer;
		PlayerTask playerTask = UnityEngine.Object.Instantiate<PlayerTask>(original, localPlayer.transform);
		PlayerTask playerTask2 = playerTask;
		PlayerControl playerControl = localPlayer;
		uint taskIdCount = playerControl.TaskIdCount;
		playerControl.TaskIdCount = taskIdCount + 1U;
		playerTask2.Id = (uint)((byte)taskIdCount);
		playerTask.Initialize();
		localPlayer.myTasks.Add(playerTask);
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0002D140 File Offset: 0x0002B340
	public void RemoveTask(PlayerTask task)
	{
		task.OnRemove();
		this.myTasks.Remove(task);
		GameData.Instance.TutOnlyRemoveTask(this.PlayerId, task.Id);
		DestroyableSingleton<HudManager>.Instance.UseButton.SetTarget(null);
		UnityEngine.Object.Destroy(task.gameObject);
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0002D194 File Offset: 0x0002B394
	private void ClearTasks()
	{
		for (int i = 0; i < this.myTasks.Count; i++)
		{
			PlayerTask playerTask = this.myTasks[i];
			playerTask.OnRemove();
			UnityEngine.Object.Destroy(playerTask.gameObject);
		}
		this.myTasks.Clear();
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0002D1E0 File Offset: 0x0002B3E0
	public void RemoveInfected()
	{
		GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(this.PlayerId);
		if (playerById.IsImpostor)
		{
			playerById.Object.nameText.Color = Color.white;
			playerById.IsImpostor = false;
			this.myTasks.RemoveAt(0);
			DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
		}
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0002D244 File Offset: 0x0002B444
	public void Die(DeathReason reason)
	{
		GameData.Instance.LastDeathReason = reason;
		this.Data.IsDead = true;
		base.gameObject.layer = LayerMask.NameToLayer("Ghost");
		this.nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
		if (base.AmOwner)
		{
			SaveManager.LastGameStart = DateTime.MinValue;
			DestroyableSingleton<HudManager>.Instance.Chat.SetVisible(true);
		}
		if (PlayerControl.LocalPlayer.Data.role == GameData.PlayerInfo.Role.Sheriff)
		{
			this.nameText.Color = Palette.White;
			List<GameData.PlayerInfo> list = (from pcd in GameData.Instance.AllPlayers
			where !pcd.Disconnected
			select pcd into pc
			where !pc.IsDead
			select pc into pci
			where !pci.IsImpostor
			select pci into pcs
			where pcs != PlayerControl.LocalPlayer.Data
			select pcs).ToList<GameData.PlayerInfo>();
			list.Shuffle<GameData.PlayerInfo>();
			GameData.PlayerInfo.Role[] roles = new GameData.PlayerInfo.Role[]
			{
				GameData.PlayerInfo.Role.None,
				GameData.PlayerInfo.Role.Sheriff
			};
			PlayerControl.LocalPlayer.RpcSetRole(new GameData.PlayerInfo[]
			{
				PlayerControl.LocalPlayer.Data,
				list.Take(1).ToArray<GameData.PlayerInfo>()[0]
			}, roles);
		}
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0002D3C0 File Offset: 0x0002B5C0
	public void Revive()
	{
		this.Data.IsDead = false;
		base.gameObject.layer = LayerMask.NameToLayer("Players");
		this.MyPhysics.ResetAnim(true);
		this.nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 4);
		if (base.AmOwner)
		{
			DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(this.Data.IsImpostor);
			DestroyableSingleton<HudManager>.Instance.Chat.ForceClosed();
			DestroyableSingleton<HudManager>.Instance.Chat.SetVisible(false);
		}
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x0002D45C File Offset: 0x0002B65C
	public void PlayAnimation(byte animType)
	{
		if (animType == 1)
		{
			ShipStatus.Instance.StartShields();
			return;
		}
		if (animType == 6)
		{
			ShipStatus.Instance.FireWeapon();
			return;
		}
		if (animType - 9 > 1)
		{
			return;
		}
		ShipStatus.Instance.OpenHatch();
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x0002D49C File Offset: 0x0002B69C
	public void CompleteTask(uint idx)
	{
		PlayerTask playerTask = this.myTasks.Find((PlayerTask p) => p.Id == idx);
		if (playerTask)
		{
			GameData.Instance.CompleteTask(this, idx);
			playerTask.Complete();
			DestroyableSingleton<Telemetry>.Instance.WriteCompleteTask(this.PlayerId, playerTask.TaskType);
			return;
		}
		Debug.LogWarning(this.PlayerId + ": Server didn't have task: " + idx);
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0002D52C File Offset: 0x0002B72C
	public void SetInfected(GameData.PlayerInfo[] infected)
	{
		this.killTimer = 20f;
		for (int i = 0; i < infected.Length; i++)
		{
			infected[i].IsImpostor = true;
		}
		DestroyableSingleton<HudManager>.Instance.MapButton.gameObject.SetActive(true);
		DestroyableSingleton<HudManager>.Instance.ReportButton.gameObject.SetActive(true);
		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		PlayerControl.LocalPlayer.RemainingEmergencies = PlayerControl.GameOptions.NumEmergencyMeetings;
		if (data.IsImpostor)
		{
			DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(true);
			if (PlayerControl.GameOptions.Gamemode == 1)
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
			List<PlayerControl> yourTeam;
			if (data.IsImpostor)
			{
				yourTeam = (from pcd in GameData.Instance.AllPlayers
				where !pcd.Disconnected
				where pcd.IsImpostor
				select pcd.Object).OrderBy(delegate(PlayerControl pc)
				{
					if (!(pc == PlayerControl.LocalPlayer))
					{
						return 1;
					}
					return 0;
				}).ToList<PlayerControl>();
			}
			else
			{
				yourTeam = (from pcd in GameData.Instance.AllPlayers
				where !pcd.Disconnected
				select pcd.Object).OrderBy(delegate(PlayerControl pc)
				{
					if (!(pc == PlayerControl.LocalPlayer))
					{
						return 1;
					}
					return 0;
				}).ToList<PlayerControl>();
			}
			base.StopAllCoroutines();
			DestroyableSingleton<HudManager>.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoShowIntro(yourTeam));
		}
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0002D76C File Offset: 0x0002B96C
	public void Exiled()
	{
		this.Die(DeathReason.Exile);
		if (base.AmOwner)
		{
			StatsManager instance = StatsManager.Instance;
			uint timesEjected = instance.TimesEjected;
			instance.TimesEjected = timesEjected + 1U;
			DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
			if (!PlayerControl.GameOptions.GhostsDoTasks)
			{
				this.ClearTasks();
				GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(this.PlayerId);
				ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
				importantTextTask.transform.SetParent(base.transform, false);
				if (playerById.IsImpostor)
				{
					importantTextTask.Text = "You are dead, but you can still sabotage.";
				}
				else
				{
					importantTextTask.Text = "You are dead, but finish your tasks anyway.";
				}
				this.myTasks.Add(importantTextTask);
			}
		}
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x0002D824 File Offset: 0x0002BA24
	public void CheckName(string name)
	{
		List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
		bool flag = allPlayers.Any((GameData.PlayerInfo i) => i.PlayerId != this.PlayerId && i.PlayerName.Equals(name, StringComparison.OrdinalIgnoreCase));
		if (flag)
		{
			for (int k = 1; k < 100; k++)
			{
				string text = name + " " + k;
				flag = false;
				for (int j = 0; j < allPlayers.Count; j++)
				{
					if (allPlayers[j].PlayerId != this.PlayerId && allPlayers[j].PlayerName.Equals(text, StringComparison.OrdinalIgnoreCase))
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
		this.RpcSetName(name);
		GameData.Instance.UpdateName(this.PlayerId, name);
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0002D904 File Offset: 0x0002BB04
	public void SetName(string name)
	{
		if (GameData.Instance)
		{
			GameData.Instance.UpdateName(this.PlayerId, name);
		}
		base.gameObject.name = name;
		this.nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 4);
		this.nameText.Text = name;
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x0002D964 File Offset: 0x0002BB64
	public void CheckColor(byte bodyColor)
	{
		List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
		int num = 0;
		while (allPlayers.Any((GameData.PlayerInfo p) => !p.Disconnected && p.PlayerId != this.PlayerId && p.ColorId == bodyColor) && num++ < 100)
		{
			bodyColor = (byte)((int)(bodyColor + 1) % Palette.PlayerColors.Length);
		}
		this.RpcSetColor(bodyColor);
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x0002D9D4 File Offset: 0x0002BBD4
	public void SetHatAlpha(float a)
	{
		Color white = Color.white;
		white.a = a;
		this.HatRenderer.color = white;
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x00006F0A File Offset: 0x0000510A
	public void SetColor(byte bodyColor)
	{
		if (GameData.Instance)
		{
			GameData.Instance.UpdateColor(this.PlayerId, bodyColor);
		}
		if (this.myRend == null)
		{
			base.GetComponent<SpriteRenderer>();
		}
		PlayerControl.SetPlayerMaterialColors((int)bodyColor, this.myRend);
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x00006F44 File Offset: 0x00005144
	public void SetSkin(uint skinId)
	{
		if (GameData.Instance)
		{
			GameData.Instance.UpdateSkin(this.PlayerId, skinId);
		}
		this.MyPhysics.SetSkin(skinId);
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x0002D9FC File Offset: 0x0002BBFC
	public void SetHat(uint hatId)
	{
		if (GameData.Instance)
		{
			GameData.Instance.UpdateHat(this.PlayerId, hatId);
		}
		PlayerControl.SetHatImage(hatId, this.HatRenderer);
		this.nameText.transform.localPosition = new Vector3(0f, (hatId == 0U) ? 0.7f : 1.05f, -0.5f);
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x00006F6F File Offset: 0x0000516F
	public static void SetSkinImage(uint skinId, SpriteRenderer target)
	{
		if (!DestroyableSingleton<HatManager>.InstanceExists)
		{
			return;
		}
		PlayerControl.SetSkinImage(DestroyableSingleton<HatManager>.Instance.GetSkinById(skinId), target);
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x00006F8A File Offset: 0x0000518A
	public static void SetSkinImage(SkinData skin, SpriteRenderer target)
	{
		target.sprite = skin.IdleFrame;
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x00006F98 File Offset: 0x00005198
	public static void SetHatImage(uint hatId, SpriteRenderer target)
	{
		if (!DestroyableSingleton<HatManager>.InstanceExists)
		{
			return;
		}
		PlayerControl.SetHatImage(DestroyableSingleton<HatManager>.Instance.GetHatById(hatId), target);
	}

	// Token: 0x0600080A RID: 2058 RVA: 0x0002DA60 File Offset: 0x0002BC60
	public static void SetHatImage(HatBehaviour hat, SpriteRenderer target)
	{
		if (target && hat)
		{
			target.sprite = hat.MainImage;
			Vector3 localPosition = target.transform.localPosition;
			localPosition.z = (hat.InFront ? -0.0001f : 0.0001f);
			target.transform.localPosition = localPosition;
			return;
		}
		string str = (!target) ? "null" : target.name;
		string str2 = (!hat) ? "null" : hat.name;
		Debug.LogError("Player: " + str + "\tHat: " + str2);
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x00006FB3 File Offset: 0x000051B3
	public void SendChat(string chatText)
	{
		if (DestroyableSingleton<HudManager>.Instance)
		{
			DestroyableSingleton<HudManager>.Instance.Chat.AddChat(this, chatText);
		}
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0002DB00 File Offset: 0x0002BD00
	private void ReportDeadBody(GameData.PlayerInfo target)
	{
		if (AmongUsClient.Instance.IsGameOver)
		{
			return;
		}
		if (MeetingHud.Instance)
		{
			return;
		}
		if (this.Data.IsDead)
		{
			return;
		}
		MeetingRoomManager.Instance.AssignSelf(this, target);
		if (!AmongUsClient.Instance.AmHost)
		{
			return;
		}
		if (ShipStatus.Instance.CheckTaskCompletion())
		{
			return;
		}
		DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(this);
		this.RpcStartMeeting(target);
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x00006FD2 File Offset: 0x000051D2
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
		for (int j = 0; j < PlayerControl.AllPlayerControls.Count; j++)
		{
			PlayerControl playerControl = PlayerControl.AllPlayerControls[j];
			if (!playerControl.GetComponent<DummyBehaviour>().enabled)
			{
				playerControl.MyPhysics.ExitAllVents();
				playerControl.NetTransform.SnapTo(ShipStatus.Instance.GetSpawnLocation((int)playerControl.PlayerId, GameData.Instance.PlayerCount));
			}
		}
		if (base.AmOwner)
		{
			if (target != null)
			{
				StatsManager instance = StatsManager.Instance;
				uint num = instance.BodiesReported;
				instance.BodiesReported = num + 1U;
			}
			else
			{
				StatsManager instance2 = StatsManager.Instance;
				uint num = instance2.EmergenciesCalled;
				instance2.EmergenciesCalled = num + 1U;
			}
		}
		if (MapBehaviour.Instance)
		{
			MapBehaviour.Instance.Close();
		}
		if (Minigame.Instance)
		{
			Minigame.Instance.Close();
		}
		KillAnimation.SetMovement(this, true);
		MeetingHud.Instance.StartCoroutine(MeetingHud.Instance.CoIntro(this, target));
		yield break;
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x0002DB70 File Offset: 0x0002BD70
	public void MurderPlayer(PlayerControl target)
	{
		if (!target)
		{
			return;
		}
		if (AmongUsClient.Instance.IsGameOver)
		{
			return;
		}
		GameData.PlayerInfo data = target.Data;
		if (!data.IsDead)
		{
			if (base.AmOwner)
			{
				StatsManager instance = StatsManager.Instance;
				uint impostorKills = instance.ImpostorKills;
				instance.ImpostorKills = impostorKills + 1U;
				if (Constants.ShouldPlaySfx())
				{
					SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);
				}
			}
			this.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
			DestroyableSingleton<Telemetry>.Instance.WriteMurder(this.PlayerId, target.PlayerId, target.transform.position);
			if (target.AmOwner)
			{
				StatsManager instance2 = StatsManager.Instance;
				uint timesMurdered = instance2.TimesMurdered;
				instance2.TimesMurdered = timesMurdered + 1U;
				if (Minigame.Instance)
				{
					Minigame.Instance.Close();
					Minigame.Instance.Close();
				}
				if (PlayerControl.GameOptions.Gamemode == 1)
				{
					ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
					importantTextTask.transform.SetParent(base.transform, false);
					importantTextTask.Text = "You are infected.";
					target.myTasks.Add(importantTextTask);
					List<GameData.PlayerInfo> list = new List<GameData.PlayerInfo>();
					foreach (GameData.PlayerInfo playerInfo in GameData.Instance.AllPlayers)
					{
						if (playerInfo.IsImpostor)
						{
							list.Add(playerInfo);
						}
					}
					list.Add(target.Data);
					foreach (PlayerControl playerControl in PlayerControl.AllPlayerControls)
					{
						target.RpcSetInfectedNoIntro(list.ToArray());
					}
				}
				if (PlayerControl.GameOptions.Gamemode != 1)
				{
					DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
					base.gameObject.layer = 12;
					this.nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
					DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowOne(this, data);
					target.RpcSetScanner(false);
					if (!PlayerControl.GameOptions.GhostsDoTasks)
					{
						target.ClearTasks();
						ImportantTextTask importantTextTask2 = new GameObject("_Player").AddComponent<ImportantTextTask>();
						importantTextTask2.transform.SetParent(base.transform, false);
						importantTextTask2.Text = "You're dead, enjoy the chaos";
						target.myTasks.Add(importantTextTask2);
					}
				}
			}
			this.MyPhysics.StartCoroutine(this.KillAnimations.Random<KillAnimation>().CoPerformKill(this, target));
		}
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x00006FE8 File Offset: 0x000051E8
	public override bool Serialize(MessageWriter writer, bool initialState)
	{
		if (initialState)
		{
			writer.Write(this.isNew);
		}
		writer.Write(this.PlayerId);
		return true;
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x00007006 File Offset: 0x00005206
	public override void Deserialize(MessageReader reader, bool initialState)
	{
		if (initialState)
		{
			this.isNew = reader.ReadBoolean();
		}
		this.PlayerId = reader.ReadByte();
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x00007023 File Offset: 0x00005223
	public void SetPlayerMaterialColors(Renderer rend)
	{
		GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(this.PlayerId);
		PlayerControl.SetPlayerMaterialColors((int)((playerById != null) ? playerById.ColorId : 0), rend);
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x0002DE24 File Offset: 0x0002C024
	public static void SetPlayerMaterialColors(int colorId, Renderer rend)
	{
		rend.material.SetColor("_BackColor", Palette.ShadowColors[colorId]);
		rend.material.SetColor("_BodyColor", Palette.PlayerColors[colorId]);
		if (colorId == 21)
		{
			rend.material.SetColor("_VisorColor", Palette.VisorColorRed);
			return;
		}
		if (colorId == 22)
		{
			rend.material.SetColor("_VisorColor", Palette.VisorColorGreen);
			return;
		}
		rend.material.SetColor("_VisorColor", Palette.VisorColor);
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x0002DECC File Offset: 0x0002C0CC
	public void RpcSetScanner(bool value)
	{
		byte b = (byte)(this.scannerCount + 1);
		this.scannerCount = b;
		byte b2 = b;
		if (AmongUsClient.Instance.AmClient)
		{
			this.SetScanner(value, b2);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 16, SendOption.Reliable);
		messageWriter.Write(value);
		messageWriter.Write(b2);
		messageWriter.EndMessage();
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x00007047 File Offset: 0x00005247
	public void RpcPlayAnimation(byte animType)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.PlayAnimation(animType);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 0, SendOption.None);
		messageWriter.Write(animType);
		messageWriter.EndMessage();
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x0000707A File Offset: 0x0000527A
	public void RpcCompleteTask(uint idx)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.CompleteTask(idx);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 1, SendOption.Reliable);
		messageWriter.WritePacked(idx);
		messageWriter.EndMessage();
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x000070AD File Offset: 0x000052AD
	public void RpcSyncSettings(GameOptionsData gameOptions)
	{
		if (!AmongUsClient.Instance.AmHost)
		{
			return;
		}
		PlayerControl.GameOptions = gameOptions;
		SaveManager.GameHostOptions = gameOptions;
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 2, SendOption.Reliable);
		messageWriter.WriteBytesAndSize(gameOptions.ToBytes());
		messageWriter.EndMessage();
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x0002DF28 File Offset: 0x0002C128
	public void RpcSetInfected(GameData.PlayerInfo[] infected)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.SetInfected(infected);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 3, SendOption.Reliable);
		messageWriter.WritePacked(infected.Length);
		for (int i = 0; i < infected.Length; i++)
		{
			messageWriter.Write(infected[i].PlayerId);
		}
		messageWriter.EndMessage();
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x0002DF88 File Offset: 0x0002C188
	public void CmdCheckName(string name)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			this.CheckName(name);
			return;
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(this.NetId, 5, SendOption.Reliable, AmongUsClient.Instance.HostId);
		messageWriter.Write(name);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x000070EB File Offset: 0x000052EB
	public void RpcSetSkin(uint skinId)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.SetSkin(skinId);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 10, SendOption.Reliable);
		messageWriter.WritePacked(skinId);
		messageWriter.EndMessage();
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x0000711F File Offset: 0x0000531F
	public void RpcSetHat(uint hatId)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.SetHat(hatId);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 9, SendOption.Reliable);
		messageWriter.WritePacked(hatId);
		messageWriter.EndMessage();
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x00007153 File Offset: 0x00005353
	public void RpcSetName(string name)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.SetName(name);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 6, SendOption.Reliable);
		messageWriter.Write(name);
		messageWriter.EndMessage();
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0002DFD8 File Offset: 0x0002C1D8
	public void CmdCheckColor(byte bodyColor)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			this.CheckColor(bodyColor);
			return;
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(this.NetId, 7, SendOption.Reliable, AmongUsClient.Instance.HostId);
		messageWriter.Write(bodyColor);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x00007186 File Offset: 0x00005386
	public void RpcSetColor(byte bodyColor)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.SetColor(bodyColor);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 8, SendOption.Reliable);
		messageWriter.Write(bodyColor);
		messageWriter.EndMessage();
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x000071B9 File Offset: 0x000053B9
	public void RpcSetTimesImpostor(float percImpostor)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.percImpostor = percImpostor;
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 14, SendOption.None);
		messageWriter.Write(percImpostor);
		messageWriter.EndMessage();
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x0002E028 File Offset: 0x0002C228
	public bool RpcSendChat(string chatText)
	{
		if (string.IsNullOrWhiteSpace(chatText))
		{
			return false;
		}
		chatText = BlockedWords.CensorWords(chatText);
		if (AmongUsClient.Instance.AmClient)
		{
			this.SendChat(chatText);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(this.NetId, 13, SendOption.Reliable, -1);
		messageWriter.Write(chatText);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
		return true;
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x0002E084 File Offset: 0x0002C284
	public void CmdReportDeadBody(GameData.PlayerInfo target)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			this.ReportDeadBody(target);
			return;
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 11, SendOption.Reliable);
		messageWriter.Write((target != null) ? target.PlayerId : byte.MaxValue);
		messageWriter.EndMessage();
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x0002E0D4 File Offset: 0x0002C2D4
	public void RpcStartMeeting(GameData.PlayerInfo info)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			base.StartCoroutine(this.CoStartMeeting(info));
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(this.NetId, 15, SendOption.Reliable, -1);
		messageWriter.Write((info != null) ? info.PlayerId : byte.MaxValue);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x0002E134 File Offset: 0x0002C334
	public void RpcMurderPlayer(PlayerControl target)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.MurderPlayer(target);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(this.NetId, 12, SendOption.Reliable, -1);
		messageWriter.WriteNetObject(target);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x0002E17C File Offset: 0x0002C37C
	public override void HandleRpc(byte callId, MessageReader reader)
	{
		switch (callId)
		{
		case 0:
			this.PlayAnimation(reader.ReadByte());
			return;
		case 1:
			this.CompleteTask(reader.ReadPackedUInt32());
			return;
		case 2:
			PlayerControl.GameOptions = GameOptionsData.FromBytes(reader.ReadBytesAndSize());
			return;
		case 3:
		{
			int num = reader.ReadPackedInt32();
			GameData.PlayerInfo[] array = new GameData.PlayerInfo[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = GameData.Instance.GetPlayerById(reader.ReadByte());
			}
			this.SetInfected(array);
			return;
		}
		case 4:
			this.Exiled();
			return;
		case 5:
			this.CheckName(reader.ReadString());
			return;
		case 6:
			this.SetName(reader.ReadString());
			return;
		case 7:
			this.CheckColor(reader.ReadByte());
			return;
		case 8:
			this.SetColor(reader.ReadByte());
			return;
		case 9:
			this.SetHat(reader.ReadPackedUInt32());
			return;
		case 10:
			this.SetSkin(reader.ReadPackedUInt32());
			return;
		case 11:
		{
			GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(reader.ReadByte());
			this.ReportDeadBody(playerById);
			return;
		}
		case 12:
		{
			PlayerControl target = reader.ReadNetObject<PlayerControl>();
			this.MurderPlayer(target);
			return;
		}
		case 13:
			this.SendChat(reader.ReadString());
			return;
		case 14:
			this.percImpostor = reader.ReadSingle();
			return;
		case 15:
		{
			GameData.PlayerInfo playerById2 = GameData.Instance.GetPlayerById(reader.ReadByte());
			base.StartCoroutine(this.CoStartMeeting(playerById2));
			return;
		}
		case 16:
			this.SetScanner(reader.ReadBoolean(), reader.ReadByte());
			return;
		case 17:
		{
			int num2 = reader.ReadPackedInt32();
			GameData.PlayerInfo[] array2 = new GameData.PlayerInfo[num2];
			for (int j = 0; j < num2; j++)
			{
				array2[j] = GameData.Instance.GetPlayerById(reader.ReadByte());
			}
			this.SetInfectedNoIntro(array2);
			return;
		}
		case 18:
		{
			int num3 = reader.ReadPackedInt32();
			GameData.PlayerInfo[] array3 = new GameData.PlayerInfo[num3];
			GameData.PlayerInfo.Role[] array4 = new GameData.PlayerInfo.Role[num3];
			for (int k = 0; k < num3; k++)
			{
				array3[k] = GameData.Instance.GetPlayerById(reader.ReadByte());
				array4[k] = (GameData.PlayerInfo.Role)reader.ReadByte();
			}
			this.SetRoles(array3, array4);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x0002E418 File Offset: 0x0002C618
	public void SetInfectedNoIntro(GameData.PlayerInfo[] infected)
	{
		this.killTimer = PlayerControl.GameOptions.KillCooldown;
		for (int i = 0; i < infected.Length; i++)
		{
			infected[i].IsImpostor = true;
		}
		DestroyableSingleton<HudManager>.Instance.MapButton.gameObject.SetActive(true);
		DestroyableSingleton<HudManager>.Instance.ReportButton.gameObject.SetActive(true);
		if (PlayerControl.LocalPlayer.Data.IsImpostor)
		{
			DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(true);
			if (PlayerControl.GameOptions.Gamemode == 1)
			{
				for (int j = 0; j < infected.Length; j++)
				{
					infected[j].Object.nameText.Color = Palette.InfectedGreen;
				}
				return;
			}
			for (int k = 0; k < infected.Length; k++)
			{
				infected[k].Object.nameText.Color = Palette.ImpostorRed;
			}
		}
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x0002E4F4 File Offset: 0x0002C6F4
	public void RpcSetInfectedNoIntro(GameData.PlayerInfo[] infected)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.SetInfectedNoIntro(infected);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 17, SendOption.Reliable);
		messageWriter.WritePacked(infected.Length);
		for (int i = 0; i < infected.Length; i++)
		{
			messageWriter.Write(infected[i].PlayerId);
		}
		messageWriter.EndMessage();
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x0002E554 File Offset: 0x0002C754
	public void RpcSetRole(GameData.PlayerInfo[] persons, GameData.PlayerInfo.Role[] roles)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.SetRoles(persons, roles);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 18, SendOption.Reliable);
		messageWriter.WritePacked(persons.Length);
		for (int i = 0; i < persons.Length; i++)
		{
			messageWriter.Write(persons[i].PlayerId);
			messageWriter.Write((byte)roles[i]);
		}
		messageWriter.EndMessage();
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x0002E5C0 File Offset: 0x0002C7C0
	public void SetRoles(GameData.PlayerInfo[] players, GameData.PlayerInfo.Role[] roles)
	{
		for (int i = 0; i < players.Length; i++)
		{
			players[i].role = roles[i];
		}
		Debug.Log("got role " + Enum.GetName(typeof(GameData.PlayerInfo.Role), PlayerControl.LocalPlayer.Data.role));
		if (PlayerControl.LocalPlayer.Data.role == GameData.PlayerInfo.Role.Sheriff)
		{
			PlayerControl.LocalPlayer.nameText.Color = Palette.SheriffYellow;
			DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x00007203 File Offset: 0x00005403
	private bool ShouldKillImp(GameData.PlayerInfo playerinfo)
	{
		return this.Data.role == GameData.PlayerInfo.Role.Sheriff || !playerinfo.IsImpostor;
	}

	// Token: 0x040007CD RID: 1997
	public byte PlayerId = byte.MaxValue;

	// Token: 0x040007CE RID: 1998
	public float MaxReportDistance = 5f;

	// Token: 0x040007CF RID: 1999
	public bool moveable = true;

	// Token: 0x040007D0 RID: 2000
	public bool inVent;

	// Token: 0x040007D1 RID: 2001
	public static PlayerControl LocalPlayer;

	// Token: 0x040007D2 RID: 2002
	private GameData.PlayerInfo _cachedData;

	// Token: 0x040007D3 RID: 2003
	public AudioSource FootSteps;

	// Token: 0x040007D4 RID: 2004
	public AudioClip KillSfx;

	// Token: 0x040007D5 RID: 2005
	public KillAnimation[] KillAnimations;

	// Token: 0x040007D6 RID: 2006
	[SerializeField]
	private float killTimer;

	// Token: 0x040007D7 RID: 2007
	public int RemainingEmergencies;

	// Token: 0x040007D8 RID: 2008
	public TextRenderer nameText;

	// Token: 0x040007D9 RID: 2009
	public LightSource LightPrefab;

	// Token: 0x040007DA RID: 2010
	private LightSource myLight;

	// Token: 0x040007DB RID: 2011
	[HideInInspector]
	public Collider2D Collider;

	// Token: 0x040007DC RID: 2012
	[HideInInspector]
	public PlayerPhysics MyPhysics;

	// Token: 0x040007DD RID: 2013
	[HideInInspector]
	public CustomNetworkTransform NetTransform;

	// Token: 0x040007DE RID: 2014
	public SpriteRenderer HatRenderer;

	// Token: 0x040007DF RID: 2015
	private SpriteRenderer myRend;

	// Token: 0x040007E0 RID: 2016
	private Collider2D[] hitBuffer = new Collider2D[20];

	// Token: 0x040007E1 RID: 2017
	public static GameOptionsData GameOptions = new GameOptionsData();

	// Token: 0x040007E2 RID: 2018
	public List<PlayerTask> myTasks = new List<PlayerTask>();

	// Token: 0x040007E3 RID: 2019
	[NonSerialized]
	public uint TaskIdCount;

	// Token: 0x040007E4 RID: 2020
	public SpriteAnim[] ScannerAnims;

	// Token: 0x040007E5 RID: 2021
	public SpriteRenderer[] ScannersImages;

	// Token: 0x040007E6 RID: 2022
	public AudioClip[] VentMoveSounds;

	// Token: 0x040007E7 RID: 2023
	public AudioClip VentEnterSound;

	// Token: 0x040007E8 RID: 2024
	private IUsable closest;

	// Token: 0x040007E9 RID: 2025
	private bool isNew = true;

	// Token: 0x040007EA RID: 2026
	public float percImpostor;

	// Token: 0x040007EB RID: 2027
	public static List<PlayerControl> AllPlayerControls = new List<PlayerControl>();

	// Token: 0x040007EC RID: 2028
	private Dictionary<Collider2D, IUsable> cache = new Dictionary<Collider2D, IUsable>(PlayerControl.ColliderComparer.Instance);

	// Token: 0x040007ED RID: 2029
	private List<IUsable> itemsInRange = new List<IUsable>();

	// Token: 0x040007EE RID: 2030
	private List<IUsable> newItemsInRange = new List<IUsable>();

	// Token: 0x040007EF RID: 2031
	private byte scannerCount;

	// Token: 0x02000183 RID: 387
	public class ColliderComparer : IEqualityComparer<Collider2D>
	{
		// Token: 0x0600082B RID: 2091 RVA: 0x0000721E File Offset: 0x0000541E
		public bool Equals(Collider2D x, Collider2D y)
		{
			return x == y;
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x00007227 File Offset: 0x00005427
		public int GetHashCode(Collider2D obj)
		{
			return obj.GetInstanceID();
		}

		// Token: 0x040007F0 RID: 2032
		public static readonly PlayerControl.ColliderComparer Instance = new PlayerControl.ColliderComparer();
	}

	// Token: 0x02000184 RID: 388
	public class UsableComparer : IEqualityComparer<IUsable>
	{
		// Token: 0x0600082F RID: 2095 RVA: 0x0000723B File Offset: 0x0000543B
		public bool Equals(IUsable x, IUsable y)
		{
			return x == y;
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x00007241 File Offset: 0x00005441
		public int GetHashCode(IUsable obj)
		{
			return obj.GetHashCode();
		}

		// Token: 0x040007F1 RID: 2033
		public static readonly PlayerControl.UsableComparer Instance = new PlayerControl.UsableComparer();
	}

	// Token: 0x02000185 RID: 389
	public enum RpcCalls : byte
	{
		// Token: 0x040007F3 RID: 2035
		PlayAnimation,
		// Token: 0x040007F4 RID: 2036
		CompleteTask,
		// Token: 0x040007F5 RID: 2037
		SyncSettings,
		// Token: 0x040007F6 RID: 2038
		SetInfected,
		// Token: 0x040007F7 RID: 2039
		Exiled,
		// Token: 0x040007F8 RID: 2040
		CheckName,
		// Token: 0x040007F9 RID: 2041
		SetName,
		// Token: 0x040007FA RID: 2042
		CheckColor,
		// Token: 0x040007FB RID: 2043
		SetColor,
		// Token: 0x040007FC RID: 2044
		SetHat,
		// Token: 0x040007FD RID: 2045
		SetSkin,
		// Token: 0x040007FE RID: 2046
		ReportDeadBody,
		// Token: 0x040007FF RID: 2047
		MurderPlayer,
		// Token: 0x04000800 RID: 2048
		SendChat,
		// Token: 0x04000801 RID: 2049
		TimesImpostor,
		// Token: 0x04000802 RID: 2050
		StartMeeting,
		// Token: 0x04000803 RID: 2051
		SetScanner
	}
}
