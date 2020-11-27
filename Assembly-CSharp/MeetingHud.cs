using System;
using System.Collections;
using System.Linq;
using Assets.CoreScripts;
using Hazel;
using InnerNet;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class MeetingHud : InnerNetObject, IDisconnectHandler
{
	// Token: 0x060004D2 RID: 1234 RVA: 0x00004FFF File Offset: 0x000031FF
	private void Awake()
	{
		if (!MeetingHud.Instance)
		{
			MeetingHud.Instance = this;
			return;
		}
		if (MeetingHud.Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x00020AEC File Offset: 0x0001ECEC
	private void Start()
	{
		DestroyableSingleton<HudManager>.Instance.Chat.gameObject.SetActive(true);
		DestroyableSingleton<HudManager>.Instance.Chat.SetPosition(this);
		DestroyableSingleton<HudManager>.Instance.StopOxyFlash();
		DestroyableSingleton<HudManager>.Instance.StopReactorFlash();
		this.SkipVoteButton.TargetPlayerId = -1;
		this.SkipVoteButton.Parent = this;
		Camera.main.GetComponent<FollowerCamera>().Locked = true;
		if (PlayerControl.LocalPlayer.Data.IsDead)
		{
			this.SetForegroundForDead();
		}
		AmongUsClient.Instance.DisconnectHandlers.AddUnique(this);
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x0000502C File Offset: 0x0000322C
	private void SetForegroundForDead()
	{
		this.amDead = true;
		this.SkipVoteButton.gameObject.SetActive(false);
		this.Glass.sprite = this.CrackedGlass;
		this.Glass.color = Color.white;
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x00020B84 File Offset: 0x0001ED84
	public void Update()
	{
		this.discussionTimer += Time.deltaTime;
		this.UpdateButtons();
		switch (this.state)
		{
		case MeetingHud.VoteStates.Discussion:
		{
			if (this.discussionTimer < (float)PlayerControl.GameOptions.DiscussionTime)
			{
				float f = (float)PlayerControl.GameOptions.DiscussionTime - this.discussionTimer;
				this.TimerText.Text = string.Format("Voting Begins In: {0}s", Mathf.CeilToInt(f));
				for (int i = 0; i < this.playerStates.Length; i++)
				{
					this.playerStates[i].SetDisabled();
				}
				this.SkipVoteButton.SetDisabled();
				return;
			}
			this.state = MeetingHud.VoteStates.NotVoted;
			bool active = PlayerControl.GameOptions.VotingTime > 0;
			this.TimerText.gameObject.SetActive(active);
			for (int j = 0; j < this.playerStates.Length; j++)
			{
				this.playerStates[j].SetEnabled();
			}
			this.SkipVoteButton.SetEnabled();
			return;
		}
		case MeetingHud.VoteStates.NotVoted:
		case MeetingHud.VoteStates.Voted:
			if (PlayerControl.GameOptions.VotingTime > 0)
			{
				float num = this.discussionTimer - (float)PlayerControl.GameOptions.DiscussionTime;
				float f2 = Mathf.Max(0f, (float)PlayerControl.GameOptions.VotingTime - num);
				this.TimerText.Text = string.Format("Voting Ends In: {0}s", Mathf.CeilToInt(f2));
				if (AmongUsClient.Instance.AmHost && num >= (float)PlayerControl.GameOptions.VotingTime)
				{
					this.ForceSkipAll();
					return;
				}
			}
			break;
		case MeetingHud.VoteStates.Results:
			if (AmongUsClient.Instance.GameMode == GameModes.OnlineGame)
			{
				float num2 = this.discussionTimer - this.resultsStartedAt;
				float num3 = Mathf.Max(0f, 5f - num2);
				this.TimerText.Text = string.Format("Proceeding In: {0}s", Mathf.CeilToInt(num3));
				if (AmongUsClient.Instance.AmHost && num3 <= 0f)
				{
					this.HandleProceed();
				}
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x00005067 File Offset: 0x00003267
	public IEnumerator CoIntro(PlayerControl reporter, GameData.PlayerInfo targetPlayer)
	{
		if (DestroyableSingleton<HudManager>.InstanceExists)
		{
			DestroyableSingleton<HudManager>.Instance.Chat.ForceClosed();
			base.transform.SetParent(DestroyableSingleton<HudManager>.Instance.transform);
			base.transform.localPosition = new Vector3(0f, -10f, -100f);
			DestroyableSingleton<HudManager>.Instance.SetHudActive(false);
		}
		OverlayKillAnimation killAnimPrefab = (targetPlayer == null) ? DestroyableSingleton<HudManager>.Instance.KillOverlay.EmergencyOverlay : DestroyableSingleton<HudManager>.Instance.KillOverlay.ReportOverlay;
		DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowOne(killAnimPrefab, reporter, targetPlayer);
		yield return DestroyableSingleton<HudManager>.Instance.KillOverlay.WaitForFinish();
		Vector3 temp = new Vector3(0f, 0f, -50f);
		for (float timer = 0f; timer < 0.25f; timer += Time.deltaTime)
		{
			float t = timer / 0.25f;
			temp.y = Mathf.SmoothStep(-10f, 0f, t);
			base.transform.localPosition = temp;
			yield return null;
		}
		temp.y = 0f;
		base.transform.localPosition = temp;
		this.TitleText.Text = "Who Is The Impostor?";
		if (!PlayerControl.LocalPlayer.Data.IsDead)
		{
			yield return DestroyableSingleton<HudManager>.Instance.ShowEmblem(false);
		}
		for (int i = 0; i < this.playerStates.Length; i++)
		{
			base.StartCoroutine(this.playerStates[i].CoAnimateOverlay());
		}
		yield break;
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x00020D80 File Offset: 0x0001EF80
	public void OnEnable()
	{
		if (this.playerStates != null)
		{
			for (int i = 0; i < this.playerStates.Length; i++)
			{
				PlayerVoteArea playerVoteArea = this.playerStates[i];
				int num = i / 5;
				int num2 = i % 5;
				playerVoteArea.transform.SetParent(base.transform);
				playerVoteArea.transform.localPosition = this.VoteOrigin + new Vector3(this.VoteButtonOffsets.x * (float)num, this.VoteButtonOffsets.y * (float)num2, -0.1f);
			}
		}
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x00005084 File Offset: 0x00003284
	private IEnumerator CoStartCutscene()
	{
		yield return DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.clear, Color.black, 1f);
		ExileController exileController = UnityEngine.Object.Instantiate<ExileController>(this.ExileCutscenePrefab);
		exileController.transform.SetParent(DestroyableSingleton<HudManager>.Instance.transform, false);
		exileController.transform.localPosition = new Vector3(0f, 0f, -60f);
		exileController.Begin(this.exiledPlayer, this.wasTie);
		this.DespawnOnDestroy = false;
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x00005093 File Offset: 0x00003293
	public void ServerStart(byte reporter)
	{
		this.reporterId = reporter;
		this.PopulateButtons(reporter);
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x00020E04 File Offset: 0x0001F004
	public void Close()
	{
		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		DestroyableSingleton<HudManager>.Instance.Chat.SetPosition(null);
		DestroyableSingleton<HudManager>.Instance.Chat.SetVisible(data.IsDead);
		base.StartCoroutine(this.CoStartCutscene());
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x00020E50 File Offset: 0x0001F050
	private void VotingComplete(byte[] states, GameData.PlayerInfo exiled, bool tie)
	{
		if (this.state == MeetingHud.VoteStates.Results)
		{
			return;
		}
		this.state = MeetingHud.VoteStates.Results;
		this.resultsStartedAt = this.discussionTimer;
		this.exiledPlayer = exiled;
		this.wasTie = tie;
		this.SkipVoteButton.gameObject.SetActive(false);
		this.SkippedVoting.gameObject.SetActive(true);
		AmongUsClient.Instance.DisconnectHandlers.Remove(this);
		this.PopulateResults(states);
		this.SetupProceedButton();
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x00020EC8 File Offset: 0x0001F0C8
	public bool Select(int suspectStateIdx)
	{
		if (this.discussionTimer < (float)PlayerControl.GameOptions.DiscussionTime)
		{
			return false;
		}
		if (PlayerControl.LocalPlayer.Data.IsDead)
		{
			return false;
		}
		SoundManager.Instance.PlaySound(this.VoteSound, false, 1f).volume = 0.8f;
		for (int i = 0; i < this.playerStates.Length; i++)
		{
			PlayerVoteArea playerVoteArea = this.playerStates[i];
			if (suspectStateIdx != (int)playerVoteArea.TargetPlayerId)
			{
				playerVoteArea.ClearButtons();
			}
		}
		if (suspectStateIdx != -1)
		{
			this.SkipVoteButton.ClearButtons();
		}
		return true;
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x00020F58 File Offset: 0x0001F158
	public void Confirm(sbyte suspectStateIdx)
	{
		if (PlayerControl.LocalPlayer.Data.IsDead)
		{
			return;
		}
		for (int i = 0; i < this.playerStates.Length; i++)
		{
			this.playerStates[i].ClearButtons();
			this.playerStates[i].voteComplete = true;
		}
		this.SkipVoteButton.ClearButtons();
		this.SkipVoteButton.voteComplete = true;
		this.SkipVoteButton.gameObject.SetActive(false);
		MeetingHud.VoteStates voteStates = this.state;
		if (voteStates != MeetingHud.VoteStates.NotVoted)
		{
			return;
		}
		this.state = MeetingHud.VoteStates.Voted;
		SoundManager.Instance.PlaySound(this.VoteLockinSound, false, 1f);
		this.CmdCastVote(PlayerControl.LocalPlayer.PlayerId, suspectStateIdx);
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x0002100C File Offset: 0x0001F20C
	public void HandleDisconnect(PlayerControl pc, DisconnectReasons reason)
	{
		if (!AmongUsClient.Instance.AmHost)
		{
			return;
		}
		int num = this.playerStates.IndexOf((PlayerVoteArea pv) => pv.TargetPlayerId == (sbyte)pc.PlayerId);
		PlayerVoteArea playerVoteArea = this.playerStates[num];
		playerVoteArea.isDead = true;
		playerVoteArea.Overlay.gameObject.SetActive(true);
		for (int i = 0; i < this.playerStates.Length; i++)
		{
			PlayerVoteArea playerVoteArea2 = this.playerStates[i];
			if (!playerVoteArea2.isDead && playerVoteArea2.didVote && playerVoteArea2.votedFor == (sbyte)pc.PlayerId)
			{
				playerVoteArea2.UnsetVote();
				base.SetDirtyBit(1U << i);
				GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById((byte)playerVoteArea2.TargetPlayerId);
				if (playerById != null)
				{
					int clientIdFromCharacter = AmongUsClient.Instance.GetClientIdFromCharacter(playerById.Object);
					if (clientIdFromCharacter != -1)
					{
						this.RpcClearVote(clientIdFromCharacter);
					}
				}
			}
		}
		base.SetDirtyBit(1U << num);
		this.CheckForEndVoting();
		if (this.state == MeetingHud.VoteStates.Results)
		{
			this.SetupProceedButton();
		}
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x00002265 File Offset: 0x00000465
	public void HandleDisconnect()
	{
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x00021118 File Offset: 0x0001F318
	private void ForceSkipAll()
	{
		for (int i = 0; i < this.playerStates.Length; i++)
		{
			PlayerVoteArea playerVoteArea = this.playerStates[i];
			if (!playerVoteArea.didVote)
			{
				playerVoteArea.didVote = true;
				playerVoteArea.votedFor = -2;
				base.SetDirtyBit(1U << i);
			}
		}
		this.CheckForEndVoting();
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x0002116C File Offset: 0x0001F36C
	public void CastVote(byte srcPlayerId, sbyte suspectPlayerId)
	{
		int num = this.playerStates.IndexOf((PlayerVoteArea pv) => pv.TargetPlayerId == (sbyte)srcPlayerId);
		PlayerVoteArea playerVoteArea = this.playerStates[num];
		if (!playerVoteArea.isDead && !playerVoteArea.didVote)
		{
			playerVoteArea.SetVote(suspectPlayerId);
			base.SetDirtyBit(1U << num);
			this.CheckForEndVoting();
		}
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x000211D0 File Offset: 0x0001F3D0
	public void ClearVote()
	{
		for (int i = 0; i < this.playerStates.Length; i++)
		{
			this.playerStates[i].voteComplete = false;
		}
		this.SkipVoteButton.voteComplete = false;
		this.SkipVoteButton.gameObject.SetActive(true);
		this.state = MeetingHud.VoteStates.NotVoted;
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x00021224 File Offset: 0x0001F424
	private void CheckForEndVoting()
	{
		if (this.playerStates.All((PlayerVoteArea ps) => ps.isDead || ps.didVote))
		{
			byte[] self = this.CalculateVotes();
			bool tie;
			int maxIdx = self.IndexOfMax((byte p) => (int)p, out tie) - 1;
			GameData.PlayerInfo exiled = GameData.Instance.AllPlayers.FirstOrDefault((GameData.PlayerInfo v) => (int)v.PlayerId == maxIdx);
			byte[] states = (from ps in this.playerStates
			select ps.GetState()).ToArray<byte>();
			this.RpcVotingComplete(states, exiled, tie);
		}
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x000212F4 File Offset: 0x0001F4F4
	private byte[] CalculateVotes()
	{
		byte[] array = new byte[21];
		for (int i = 0; i < this.playerStates.Length; i++)
		{
			PlayerVoteArea playerVoteArea = this.playerStates[i];
			if (playerVoteArea.didVote)
			{
				int num = (int)(playerVoteArea.votedFor + 1);
				if (num >= 0 && num < array.Length)
				{
					byte[] array2 = array;
					int num2 = num;
					int num3 = num2;
					array2[num3] += 1;
				}
			}
		}
		return array;
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x00021354 File Offset: 0x0001F554
	public override bool Serialize(MessageWriter writer, bool initialState)
	{
		if (this.playerStates == null)
		{
			return false;
		}
		if (initialState)
		{
			for (int i = 0; i < this.playerStates.Length; i++)
			{
				this.playerStates[i].Serialize(writer);
			}
		}
		else
		{
			writer.WritePacked(this.DirtyBits);
			for (int j = 0; j < this.playerStates.Length; j++)
			{
				if ((this.DirtyBits & 1U << j) != 0U)
				{
					this.playerStates[j].Serialize(writer);
				}
			}
		}
		this.DirtyBits = 0U;
		return true;
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x000213D8 File Offset: 0x0001F5D8
	public override void Deserialize(MessageReader reader, bool initialState)
	{
		if (initialState)
		{
			MeetingHud.Instance = this;
			this.PopulateButtons(0);
			for (int i = 0; i < this.playerStates.Length; i++)
			{
				PlayerVoteArea playerVoteArea = this.playerStates[i];
				playerVoteArea.Deserialize(reader);
				if (playerVoteArea.didReport)
				{
					this.reporterId = (byte)playerVoteArea.TargetPlayerId;
				}
			}
			return;
		}
		uint num = reader.ReadPackedUInt32();
		for (int j = 0; j < this.playerStates.Length; j++)
		{
			if ((num & 1U << j) != 0U)
			{
				this.playerStates[j].Deserialize(reader);
			}
		}
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x00021460 File Offset: 0x0001F660
	public void HandleProceed()
	{
		if (!AmongUsClient.Instance.AmHost)
		{
			base.StartCoroutine(Effects.Shake(this.HostIcon.transform, 0.75f, 0.25f));
			return;
		}
		if (this.state != MeetingHud.VoteStates.Results)
		{
			return;
		}
		this.state = MeetingHud.VoteStates.Proceeding;
		this.RpcClose();
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x000214B4 File Offset: 0x0001F6B4
	private void SetupProceedButton()
	{
		if (AmongUsClient.Instance.GameMode != GameModes.OnlineGame)
		{
			this.TimerText.gameObject.SetActive(false);
			this.ProceedButton.gameObject.SetActive(true);
			this.HostIcon.gameObject.SetActive(true);
			GameData.PlayerInfo host = GameData.Instance.GetHost();
			if (host != null)
			{
				PlayerControl.SetPlayerMaterialColors((int)host.ColorId, this.HostIcon);
				return;
			}
			this.HostIcon.enabled = false;
		}
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x00021530 File Offset: 0x0001F730
	private void PopulateResults(byte[] states)
	{
		DestroyableSingleton<Telemetry>.Instance.WriteMeetingEnded(states, this.discussionTimer);
		this.TitleText.Text = "Voting Results";
		int num = 0;
		for (int i = 0; i < this.playerStates.Length; i++)
		{
			PlayerVoteArea playerVoteArea = this.playerStates[i];
			playerVoteArea.ClearForResults();
			int num2 = 0;
			for (int j = 0; j < this.playerStates.Length; j++)
			{
				if (!states[j].HasAnyBit(128))
				{
					GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById((byte)this.playerStates[j].TargetPlayerId);
					int num3 = (int)((states[j] & 31) - 1);
					if (num3 == (int)playerVoteArea.TargetPlayerId)
					{
						SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate<SpriteRenderer>(this.PlayerVotePrefab);
						if (PlayerControl.GameOptions.AnonVotes)
						{
							PlayerControl.SetPlayerMaterialColors(6, spriteRenderer);
						}
						else
						{
							PlayerControl.SetPlayerMaterialColors((int)playerById.ColorId, spriteRenderer);
						}
						spriteRenderer.transform.SetParent(playerVoteArea.transform);
						spriteRenderer.transform.localPosition = this.CounterOrigin + new Vector3(this.CounterOffsets.x / 2f * (float)num2, 0f, 0f);
						spriteRenderer.transform.localScale = Vector3.zero;
						base.StartCoroutine(Effects.BloopHalf((float)num2 * 0.5f, spriteRenderer.transform, 0.5f));
						num2++;
					}
					else if (i == 0 && num3 == -1)
					{
						SpriteRenderer spriteRenderer2 = UnityEngine.Object.Instantiate<SpriteRenderer>(this.PlayerVotePrefab);
						if (PlayerControl.GameOptions.AnonVotes)
						{
							PlayerControl.SetPlayerMaterialColors(6, spriteRenderer2);
						}
						else
						{
							PlayerControl.SetPlayerMaterialColors((int)playerById.ColorId, spriteRenderer2);
						}
						spriteRenderer2.transform.SetParent(this.SkippedVoting.transform);
						spriteRenderer2.transform.localPosition = this.CounterOrigin + new Vector3(this.CounterOffsets.x / 2f * (float)num, 0f, 0f);
						spriteRenderer2.transform.localScale = Vector3.zero;
						base.StartCoroutine(Effects.BloopHalf((float)num * 0.5f, spriteRenderer2.transform, 0.5f));
						num++;
					}
				}
			}
		}
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x0002176C File Offset: 0x0001F96C
	private void UpdateButtons()
	{
		if (PlayerControl.LocalPlayer.Data.IsDead && !this.amDead)
		{
			this.SetForegroundForDead();
		}
		if (AmongUsClient.Instance.AmHost)
		{
			for (int i = 0; i < this.playerStates.Length; i++)
			{
				GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
				PlayerVoteArea playerVoteArea = this.playerStates[i];
				bool flag = playerInfo.Disconnected || playerInfo.IsDead;
				if (flag != playerVoteArea.isDead)
				{
					playerVoteArea.SetDead(playerInfo.PlayerId == PlayerControl.LocalPlayer.PlayerId, this.reporterId == playerInfo.PlayerId, flag);
					base.SetDirtyBit(1U << i);
				}
			}
		}
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x00021820 File Offset: 0x0001FA20
	private void PopulateButtons(byte reporter)
	{
		this.playerStates = new PlayerVoteArea[GameData.Instance.PlayerCount];
		for (int i = 0; i < this.playerStates.Length; i++)
		{
			GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
			PlayerVoteArea playerVoteArea = this.playerStates[i] = this.CreateButton(playerInfo);
			playerVoteArea.Parent = this;
			playerVoteArea.TargetPlayerId = (sbyte)playerInfo.PlayerId;
			playerVoteArea.SetDead(playerInfo.PlayerId == PlayerControl.LocalPlayer.PlayerId, reporter == playerInfo.PlayerId, playerInfo.Disconnected || playerInfo.IsDead);
		}
		this.SortButtons();
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x000218C4 File Offset: 0x0001FAC4
	private void SortButtons()
	{
		PlayerVoteArea[] array = this.playerStates.OrderBy(delegate(PlayerVoteArea p)
		{
			if (!p.isDead)
			{
				return 0;
			}
			return 50;
		}).ThenBy((PlayerVoteArea p) => p.TargetPlayerId).ToArray<PlayerVoteArea>();
		for (int i = 0; i < array.Length; i++)
		{
			int num = i % 2;
			int num2 = i / 2;
			array[i].transform.localPosition = this.VoteOrigin + new Vector3(this.VoteButtonOffsets.x * (float)num, this.VoteButtonOffsets.y / 2f * (float)num2, -1f);
		}
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x00021980 File Offset: 0x0001FB80
	private PlayerVoteArea CreateButton(GameData.PlayerInfo playerInfo)
	{
		PlayerVoteArea playerVoteArea = UnityEngine.Object.Instantiate<PlayerVoteArea>(this.PlayerButtonPrefab, this.ButtonParent.transform);
		PlayerControl.SetPlayerMaterialColors((int)playerInfo.ColorId, playerVoteArea.PlayerIcon);
		playerVoteArea.PlayerIcon.transform.localScale = new Vector3(0.5f, 1f, 1f);
		playerVoteArea.NameText.Text = playerInfo.PlayerName;
		playerVoteArea.NameText.transform.localScale = new Vector3(0.6f, 1.1f, 1.1f);
		bool flag = PlayerControl.LocalPlayer.Data.IsImpostor && playerInfo.IsImpostor;
		if (PlayerControl.GameOptions.Gamemode == 1)
		{
			playerVoteArea.NameText.Color = (flag ? Palette.InfectedGreen : Color.white);
		}
		else
		{
			playerVoteArea.NameText.Color = (flag ? Palette.ImpostorRed : Color.white);
		}
		playerVoteArea.transform.localScale = new Vector3(1f, 0.5f, 1f);
		return playerVoteArea;
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x000050A3 File Offset: 0x000032A3
	public void RpcClose()
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.Close();
		}
		AmongUsClient.Instance.SendRpc(this.NetId, 0, SendOption.Reliable);
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x00021A8C File Offset: 0x0001FC8C
	public void CmdCastVote(byte playerId, sbyte suspectIdx)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			this.CastVote(playerId, suspectIdx);
			return;
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(this.NetId, 2, SendOption.Reliable, AmongUsClient.Instance.HostId);
		messageWriter.Write(playerId);
		messageWriter.Write(suspectIdx);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x00021AE4 File Offset: 0x0001FCE4
	private void RpcVotingComplete(byte[] states, GameData.PlayerInfo exiled, bool tie)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			this.VotingComplete(states, exiled, tie);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 1, SendOption.Reliable);
		messageWriter.WriteBytesAndSize(states);
		messageWriter.Write((exiled != null) ? exiled.PlayerId : byte.MaxValue);
		messageWriter.Write(tie);
		messageWriter.EndMessage();
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x00021B44 File Offset: 0x0001FD44
	private void RpcClearVote(int clientId)
	{
		if (AmongUsClient.Instance.ClientId == clientId)
		{
			this.ClearVote();
			return;
		}
		MessageWriter msg = AmongUsClient.Instance.StartRpcImmediately(this.NetId, 3, SendOption.Reliable, clientId);
		AmongUsClient.Instance.FinishRpcImmediately(msg);
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x00021B84 File Offset: 0x0001FD84
	public override void HandleRpc(byte callId, MessageReader reader)
	{
		switch (callId)
		{
		case 0:
			this.Close();
			return;
		case 1:
		{
			byte[] states = reader.ReadBytesAndSize();
			GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(reader.ReadByte());
			bool tie = reader.ReadBoolean();
			this.VotingComplete(states, playerById, tie);
			return;
		}
		case 2:
		{
			byte srcPlayerId = reader.ReadByte();
			sbyte suspectPlayerId = reader.ReadSByte();
			this.CastVote(srcPlayerId, suspectPlayerId);
			return;
		}
		case 3:
			this.ClearVote();
			return;
		default:
			return;
		}
	}

	// Token: 0x040004A1 RID: 1185
	private const float ResultsTime = 5f;

	// Token: 0x040004A2 RID: 1186
	private const float Depth = -100f;

	// Token: 0x040004A3 RID: 1187
	public static MeetingHud Instance;

	// Token: 0x040004A4 RID: 1188
	public Transform ButtonParent;

	// Token: 0x040004A5 RID: 1189
	public TextRenderer TitleText;

	// Token: 0x040004A6 RID: 1190
	public Vector3 VoteOrigin = new Vector3(-3.6f, 1.75f);

	// Token: 0x040004A7 RID: 1191
	public Vector3 VoteButtonOffsets = new Vector2(3.6f, -0.91f);

	// Token: 0x040004A8 RID: 1192
	private Vector3 CounterOrigin = new Vector2(0.5f, -0.13f);

	// Token: 0x040004A9 RID: 1193
	private Vector3 CounterOffsets = new Vector2(0.3f, 0f);

	// Token: 0x040004AA RID: 1194
	public PlayerVoteArea SkipVoteButton;

	// Token: 0x040004AB RID: 1195
	[HideInInspector]
	private PlayerVoteArea[] playerStates;

	// Token: 0x040004AC RID: 1196
	public PlayerVoteArea PlayerButtonPrefab;

	// Token: 0x040004AD RID: 1197
	public SpriteRenderer PlayerVotePrefab;

	// Token: 0x040004AE RID: 1198
	public Sprite CrackedGlass;

	// Token: 0x040004AF RID: 1199
	public SpriteRenderer Glass;

	// Token: 0x040004B0 RID: 1200
	public PassiveButton ProceedButton;

	// Token: 0x040004B1 RID: 1201
	public ExileController ExileCutscenePrefab;

	// Token: 0x040004B2 RID: 1202
	public AudioClip VoteSound;

	// Token: 0x040004B3 RID: 1203
	public AudioClip VoteLockinSound;

	// Token: 0x040004B4 RID: 1204
	private MeetingHud.VoteStates state;

	// Token: 0x040004B5 RID: 1205
	public SpriteRenderer SkippedVoting;

	// Token: 0x040004B6 RID: 1206
	public SpriteRenderer HostIcon;

	// Token: 0x040004B7 RID: 1207
	public Sprite KillBackground;

	// Token: 0x040004B8 RID: 1208
	private GameData.PlayerInfo exiledPlayer;

	// Token: 0x040004B9 RID: 1209
	private bool wasTie;

	// Token: 0x040004BA RID: 1210
	public TextRenderer TimerText;

	// Token: 0x040004BB RID: 1211
	public float discussionTimer;

	// Token: 0x040004BC RID: 1212
	private byte reporterId;

	// Token: 0x040004BD RID: 1213
	private bool amDead;

	// Token: 0x040004BE RID: 1214
	private float resultsStartedAt;

	// Token: 0x020000E6 RID: 230
	public enum VoteStates
	{
		// Token: 0x040004C0 RID: 1216
		Discussion,
		// Token: 0x040004C1 RID: 1217
		NotVoted,
		// Token: 0x040004C2 RID: 1218
		Voted,
		// Token: 0x040004C3 RID: 1219
		Results,
		// Token: 0x040004C4 RID: 1220
		Proceeding
	}

	// Token: 0x020000E7 RID: 231
	private enum RpcCalls
	{
		// Token: 0x040004C6 RID: 1222
		Close,
		// Token: 0x040004C7 RID: 1223
		VotingComplete,
		// Token: 0x040004C8 RID: 1224
		CastVote,
		// Token: 0x040004C9 RID: 1225
		ClearVote
	}
}
