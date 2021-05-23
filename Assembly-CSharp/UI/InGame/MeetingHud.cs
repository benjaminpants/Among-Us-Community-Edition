using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.CoreScripts;
using Hazel;
using InnerNet;
using UnityEngine;

public class MeetingHud : InnerNetObject, IDisconnectHandler
{
	public enum VoteStates
	{
		Discussion,
		NotVoted,
		Voted,
		Results,
		Proceeding
	}

	private enum RpcCalls
	{
		Close,
		VotingComplete,
		CastVote,
		ClearVote
	}

	private const float ResultsTime = 5f;

	private const float Depth = -100f;

	public static MeetingHud Instance;

	public Transform ButtonParent;

	public TextRenderer TitleText;

	public Vector3 VoteOrigin = new Vector3(-3.6f, 1.75f);

	public Vector3 VoteButtonOffsets = new Vector2(3.6f, -0.91f);

	private Vector3 CounterOrigin = new Vector2(0.5f, -0.13f);

	private Vector3 CounterOffsets = new Vector2(0.3f, 0f);

	public PlayerVoteArea SkipVoteButton;

	[HideInInspector]
	private PlayerVoteArea[] playerStates;

	public PlayerVoteArea PlayerButtonPrefab;

	public SpriteRenderer PlayerVotePrefab;

	public Sprite CrackedGlass;

	public SpriteRenderer Glass;

	public PassiveButton ProceedButton;

	public ExileController ExileCutscenePrefab;

	public AudioClip VoteSound;

	public AudioClip VoteLockinSound;

	private VoteStates state;

	public SpriteRenderer SkippedVoting;

	public SpriteRenderer HostIcon;

	public Sprite KillBackground;

	private GameData.PlayerInfo exiledPlayer;

	private bool wasTie;

	public TextRenderer TimerText;

	public float discussionTimer;

	private byte reporterId;

	private bool amDead;

	private float resultsStartedAt;

	public List<byte> ExtraVotes;

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		DestroyableSingleton<HudManager>.Instance.Chat.gameObject.SetActive(value: true);
		DestroyableSingleton<HudManager>.Instance.Chat.SetPosition(this);
		DestroyableSingleton<HudManager>.Instance.StopOxyFlash();
		DestroyableSingleton<HudManager>.Instance.StopReactorFlash();
		SkipVoteButton.TargetPlayerId = -1;
		SkipVoteButton.Parent = this;
		Camera.main.GetComponent<FollowerCamera>().Locked = true;
		if (PlayerControl.LocalPlayer.Data.IsDead)
		{
			SetForegroundForDead();
		}
		AmongUsClient.Instance.DisconnectHandlers.AddUnique(this);
	}

	private void SetForegroundForDead()
	{
		amDead = true;
		SkipVoteButton.gameObject.SetActive(value: false);
		Glass.sprite = CrackedGlass;
		Glass.color = Color.white;
	}

	public void Update()
	{
		discussionTimer += Time.deltaTime;
		UpdateButtons();
		switch (state)
		{
		case VoteStates.Discussion:
		{
			if (discussionTimer < (float)PlayerControl.GameOptions.DiscussionTime)
			{
				float f = (float)PlayerControl.GameOptions.DiscussionTime - discussionTimer;
				TimerText.Text = $"Voting Begins In: {Mathf.CeilToInt(f)}s";
				for (int i = 0; i < playerStates.Length; i++)
				{
					playerStates[i].SetDisabled();
				}
				SkipVoteButton.SetDisabled();
				break;
			}
			state = VoteStates.NotVoted;
			bool active = PlayerControl.GameOptions.VotingTime > 0;
			TimerText.gameObject.SetActive(active);
			for (int j = 0; j < playerStates.Length; j++)
			{
				playerStates[j].SetEnabled();
			}
			SkipVoteButton.SetEnabled();
			break;
		}
		case VoteStates.NotVoted:
		case VoteStates.Voted:
			if (PlayerControl.GameOptions.VotingTime > 0)
			{
				float num3 = discussionTimer - (float)PlayerControl.GameOptions.DiscussionTime;
				float f2 = Mathf.Max(0f, (float)PlayerControl.GameOptions.VotingTime - num3);
				TimerText.Text = $"Voting Ends In: {Mathf.CeilToInt(f2)}s";
				if (AmongUsClient.Instance.AmHost && num3 >= (float)PlayerControl.GameOptions.VotingTime)
				{
					ForceSkipAll();
				}
			}
			break;
		case VoteStates.Results:
			if (AmongUsClient.Instance.GameMode == GameModes.OnlineGame)
			{
				float num = discussionTimer - resultsStartedAt;
				float num2 = Mathf.Max(0f, 10f - num);
				TimerText.Text = $"Proceeding In: {Mathf.CeilToInt(num2)}s";
				if (AmongUsClient.Instance.AmHost && num2 <= 0f)
				{
					HandleProceed();
				}
			}
			break;
		}
	}

	public IEnumerator CoIntro(PlayerControl reporter, GameData.PlayerInfo targetPlayer)
	{
		if (DestroyableSingleton<HudManager>.InstanceExists)
		{
			DestroyableSingleton<HudManager>.Instance.Chat.ForceClosed();
			base.transform.SetParent(DestroyableSingleton<HudManager>.Instance.transform);
			base.transform.localPosition = new Vector3(0f, -10f, -100f);
			DestroyableSingleton<HudManager>.Instance.SetHudActive(isActive: false);
		}
		OverlayKillAnimation killAnimPrefab = ((targetPlayer == null) ? DestroyableSingleton<HudManager>.Instance.KillOverlay.EmergencyOverlay : DestroyableSingleton<HudManager>.Instance.KillOverlay.ReportOverlay);
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
		TitleText.Text = CE_LanguageManager.GetGMLanguage(CE_LuaLoader.CurrentGM.internalname).GetText("Meeting_WhoIsImpostor");
		if (!PlayerControl.LocalPlayer.Data.IsDead)
		{
			yield return DestroyableSingleton<HudManager>.Instance.ShowEmblem(shhh: false);
		}
		for (int i = 0; i < playerStates.Length; i++)
		{
			StartCoroutine(playerStates[i].CoAnimateOverlay());
		}
	}

	public void OnEnable()
	{
		if (playerStates != null)
		{
			for (int i = 0; i < playerStates.Length; i++)
			{
				PlayerVoteArea obj = playerStates[i];
				int num = i / 5;
				int num2 = i % 5;
				obj.transform.SetParent(base.transform);
				obj.transform.localPosition = VoteOrigin + new Vector3(VoteButtonOffsets.x * (float)num, VoteButtonOffsets.y * (float)num2, -0.1f);
			}
		}
	}

	private IEnumerator CoStartCutscene()
	{
		yield return DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.clear, Color.black, 1f);
		ExileController exileController = Object.Instantiate(ExileCutscenePrefab);
		exileController.transform.SetParent(DestroyableSingleton<HudManager>.Instance.transform, worldPositionStays: false);
		exileController.transform.localPosition = new Vector3(0f, 0f, -60f);
		exileController.Begin(exiledPlayer, wasTie);
		DespawnOnDestroy = false;
		Object.Destroy(base.gameObject);
	}

	public void ServerStart(byte reporter)
	{
		reporterId = reporter;
		PopulateButtons(reporter);
	}

	public void Close()
	{
		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		DestroyableSingleton<HudManager>.Instance.Chat.SetPosition(null);
		DestroyableSingleton<HudManager>.Instance.Chat.SetVisible(data.IsDead);
		StartCoroutine(CoStartCutscene());
	}

	private void VotingComplete(byte[] states, GameData.PlayerInfo exiled, bool tie)
	{
		if (state != VoteStates.Results)
		{
			state = VoteStates.Results;
			resultsStartedAt = discussionTimer;
			exiledPlayer = exiled;
			wasTie = tie;
			SkipVoteButton.gameObject.SetActive(value: false);
			SkippedVoting.gameObject.SetActive(value: true);
			AmongUsClient.Instance.DisconnectHandlers.Remove(this);
			PopulateResults(states);
			SetupProceedButton();
		}
	}

	public bool Select(int suspectStateIdx)
	{
		if (discussionTimer < (float)PlayerControl.GameOptions.DiscussionTime)
		{
			return false;
		}
		if (PlayerControl.LocalPlayer.Data.IsDead)
		{
			return false;
		}
		SoundManager.Instance.PlaySound(VoteSound, loop: false).volume = 0.8f;
		for (int i = 0; i < playerStates.Length; i++)
		{
			PlayerVoteArea playerVoteArea = playerStates[i];
			if (suspectStateIdx != playerVoteArea.TargetPlayerId)
			{
				playerVoteArea.ClearButtons();
			}
		}
		if (suspectStateIdx != -1)
		{
			SkipVoteButton.ClearButtons();
		}
		return true;
	}

	public void Confirm(sbyte suspectStateIdx)
	{
		if (!PlayerControl.LocalPlayer.Data.IsDead)
		{
			for (int i = 0; i < playerStates.Length; i++)
			{
				playerStates[i].ClearButtons();
				playerStates[i].voteComplete = true;
			}
			SkipVoteButton.ClearButtons();
			SkipVoteButton.voteComplete = true;
			SkipVoteButton.gameObject.SetActive(value: false);
			VoteStates voteStates = state;
			if (voteStates != VoteStates.NotVoted)
			{
				_ = 3;
				return;
			}
			state = VoteStates.Voted;
			SoundManager.Instance.PlaySound(VoteLockinSound, loop: false);
			CmdCastVote(PlayerControl.LocalPlayer.PlayerId, suspectStateIdx);
		}
	}

	public void HandleDisconnect(PlayerControl pc, DisconnectReasons reason)
	{
		if (!AmongUsClient.Instance.AmHost)
		{
			return;
		}
		int num = playerStates.IndexOf((PlayerVoteArea pv) => pv.TargetPlayerId == pc.PlayerId);
		PlayerVoteArea obj = playerStates[num];
		obj.isDead = true;
		obj.Overlay.gameObject.SetActive(value: true);
		for (int i = 0; i < playerStates.Length; i++)
		{
			PlayerVoteArea playerVoteArea = playerStates[i];
			if (playerVoteArea.isDead || !playerVoteArea.didVote || playerVoteArea.votedFor != pc.PlayerId)
			{
				continue;
			}
			playerVoteArea.UnsetVote();
			SetDirtyBit((uint)(1 << i));
			GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById((byte)playerVoteArea.TargetPlayerId);
			if (playerById != null)
			{
				int clientIdFromCharacter = AmongUsClient.Instance.GetClientIdFromCharacter(playerById.Object);
				if (clientIdFromCharacter != -1)
				{
					RpcClearVote(clientIdFromCharacter);
				}
			}
		}
		SetDirtyBit((uint)(1 << num));
		CheckForEndVoting();
		if (state == VoteStates.Results)
		{
			SetupProceedButton();
		}
	}

	public void HandleDisconnect()
	{
	}

	private void ForceSkipAll()
	{
		for (int i = 0; i < playerStates.Length; i++)
		{
			PlayerVoteArea playerVoteArea = playerStates[i];
			if (!playerVoteArea.didVote)
			{
				playerVoteArea.didVote = true;
				playerVoteArea.votedFor = -2;
				SetDirtyBit((uint)(1 << i));
			}
		}
		CheckForEndVoting();
	}

	public void CastVote(byte srcPlayerId, sbyte suspectPlayerId)
	{
		int num = playerStates.IndexOf((PlayerVoteArea pv) => pv.TargetPlayerId == srcPlayerId);
		PlayerVoteArea playerVoteArea = playerStates[num];
		if (CE_LuaLoader.CurrentGMLua)
		{
			suspectPlayerId = (sbyte)CE_LuaLoader.GetGamemodeResult("OnVote", new CE_PlayerInfoLua(GameData.Instance.GetPlayerById(srcPlayerId)), suspectPlayerId, suspectPlayerId == -1 || suspectPlayerId == -2).Number;
		}
		if (!playerVoteArea.isDead && !playerVoteArea.didVote)
		{
			playerVoteArea.SetVote(suspectPlayerId);
			SetDirtyBit((uint)(1 << num));
			CheckForEndVoting();
		}
	}

	public void ClearVote()
	{
		for (int i = 0; i < playerStates.Length; i++)
		{
			playerStates[i].voteComplete = false;
		}
		SkipVoteButton.voteComplete = false;
		SkipVoteButton.gameObject.SetActive(value: true);
		state = VoteStates.NotVoted;
	}

	private void CheckForEndVoting()
	{
		if (playerStates.All((PlayerVoteArea ps) => ps.isDead || ps.didVote))
		{
			byte[] self = CalculateVotes();
			bool tie;
			int maxIdx = self.IndexOfMax((byte p) => p, out tie) - 1;
			GameData.PlayerInfo exiled = GameData.Instance.AllPlayers.FirstOrDefault((GameData.PlayerInfo v) => v.PlayerId == maxIdx);
			byte[] states = playerStates.Select((PlayerVoteArea ps) => ps.GetState()).ToArray();
			RpcVotingComplete(states, exiled, tie);
		}
	}

	private byte[] CalculateVotes()
	{
		byte[] array = new byte[21];
        for (int i = 0; i < playerStates.Length; i++)
        {
            PlayerVoteArea playerVoteArea = playerStates[i];
            if (playerVoteArea.didVote)
            {
                int num = playerVoteArea.votedFor + 1;
                if (num >= 0 && num < array.Length)
                {
                    int num2 = num;
                    array[num2]++;
                }
            }
        }
		/*for (int i = 0; i < ExtraVotes.Count; i++)
		{
			int num = ExtraVotes[i] + 1;
			if (num >= 0 && num < array.Length)
			{
				int num2 = num;
				array[num2]++;
			}
		}*/
		return array;
	}

	public override bool Serialize(MessageWriter writer, bool initialState)
	{
		if (playerStates == null)
		{
			return false;
		}
		if (initialState)
		{
			for (int i = 0; i < playerStates.Length; i++)
			{
				playerStates[i].Serialize(writer);
			}
		}
		else
		{
			writer.WritePacked(DirtyBits);
			for (int j = 0; j < playerStates.Length; j++)
			{
				if ((DirtyBits & (uint)(1 << j)) != 0)
				{
					playerStates[j].Serialize(writer);
				}
			}
		}
		DirtyBits = 0u;
		return true;
	}

	public override void Deserialize(MessageReader reader, bool initialState)
	{
		if (initialState)
		{
			Instance = this;
			PopulateButtons(0);
			for (int i = 0; i < playerStates.Length; i++)
			{
				PlayerVoteArea playerVoteArea = playerStates[i];
				playerVoteArea.Deserialize(reader);
				if (playerVoteArea.didReport)
				{
					reporterId = (byte)playerVoteArea.TargetPlayerId;
				}
			}
			return;
		}
		uint num = reader.ReadPackedUInt32();
		for (int j = 0; j < playerStates.Length; j++)
		{
			if ((num & (uint)(1 << j)) != 0)
			{
				playerStates[j].Deserialize(reader);
			}
		}
	}

	public void HandleProceed()
	{
		if (AmongUsClient.Instance.AmHost)
		{
			if (state == VoteStates.Results)
			{
				state = VoteStates.Proceeding;
				RpcClose();
			}
		}
		else
		{
			StartCoroutine(Effects.Shake(HostIcon.transform));
		}
	}

	private void SetupProceedButton()
	{
		if (AmongUsClient.Instance.GameMode != GameModes.OnlineGame)
		{
			TimerText.gameObject.SetActive(value: false);
			ProceedButton.gameObject.SetActive(value: true);
			HostIcon.gameObject.SetActive(value: true);
			GameData.PlayerInfo host = GameData.Instance.GetHost();
			if (host != null)
			{
				PlayerControl.SetPlayerMaterialColors(host.ColorId, HostIcon);
			}
			else
			{
				HostIcon.enabled = false;
			}
		}
	}


	private void PopulateResults(byte[] states)
	{
		DestroyableSingleton<Telemetry>.Instance.WriteMeetingEnded(states, discussionTimer);
		TitleText.Text = "Voting Results";
		int num = 0;
		for (int i = 0; i < playerStates.Length; i++)
		{
			PlayerVoteArea playerVoteArea = playerStates[i];
			playerVoteArea.ClearForResults();
			int num2 = 0;
			for (int j = 0; j < playerStates.Length; j++)
			{
				if (states[j].HasAnyBit((byte)128))
				{
					continue;
				}
				GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById((byte)playerStates[j].TargetPlayerId);
				int num3 = (states[j] & 0x1F) - 1;
				if (num3 == playerVoteArea.TargetPlayerId)
				{
					SpriteRenderer spriteRenderer = Object.Instantiate(PlayerVotePrefab);
					if (PlayerControl.GameOptions.AnonVotes)
					{
						PlayerControl.SetPlayerMaterialColors((uint)Palette.PLColors.FindIndex(c => c.Name == "Black"), spriteRenderer);
					}
					else
					{
						PlayerControl.SetPlayerMaterialColors(playerById.ColorId, spriteRenderer);
					}
					spriteRenderer.transform.SetParent(playerVoteArea.transform);
					spriteRenderer.transform.localPosition = CounterOrigin + new Vector3(CounterOffsets.x / 2f * (float)num2, 0f, 0f);
					spriteRenderer.transform.localScale = Vector3.zero;
					StartCoroutine(Effects.BloopHalf((float)num2 * 0.5f, spriteRenderer.transform));
					num2++;
				}
				else if (i == 0 && num3 == -1)
				{
					SpriteRenderer spriteRenderer2 = Object.Instantiate(PlayerVotePrefab);
					if (PlayerControl.GameOptions.AnonVotes)
					{
						PlayerControl.SetPlayerMaterialColors((uint)Palette.PLColors.FindIndex(c => c.Name == "Black"), spriteRenderer2);
					}
					else
					{
						PlayerControl.SetPlayerMaterialColors(playerById.ColorId, spriteRenderer2);
					}
					spriteRenderer2.transform.SetParent(SkippedVoting.transform);
					spriteRenderer2.transform.localPosition = CounterOrigin + new Vector3((CounterOffsets.x / 2) * (float)num, 0f, 0f);
					spriteRenderer2.transform.localScale = Vector3.zero;
					StartCoroutine(Effects.BloopEntireHalf(num / 2f, spriteRenderer2.transform));
					num++;
				}
			}
		}
	}


	private void CE_UpdateButtons()
	{
		for (int i = 0; i < playerStates.Length; i++)
		{
			PlayerVoteArea playerVoteArea = playerStates[i];
			UpdateIcons(playerVoteArea);
			playerVoteArea.Update();
		}
	}

	private void UpdateButtons()
	{
		if (PlayerControl.LocalPlayer.Data.IsDead && !amDead)
		{
			SetForegroundForDead();
		}
		CE_UpdateButtons();
		if (!AmongUsClient.Instance.AmHost)
		{
			return;
		}
		for (int i = 0; i < playerStates.Length; i++)
		{			
			GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
			PlayerVoteArea playerVoteArea = playerStates[i];
			bool flag = playerInfo.Disconnected || playerInfo.IsDead;
			if (flag != playerVoteArea.isDead)
			{
				playerVoteArea.SetDead(playerInfo.PlayerId == PlayerControl.LocalPlayer.PlayerId, reporterId == playerInfo.PlayerId, flag);
				SetDirtyBit((uint)(1 << i));
			}
		}
	}

	private void PopulateButtons(byte reporter)
	{
		playerStates = new PlayerVoteArea[GameData.Instance.PlayerCount];
		for (int i = 0; i < playerStates.Length; i++)
		{
			GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
			PlayerVoteArea obj = (playerStates[i] = CreateButton(playerInfo));
			obj.Parent = this;
			obj.TargetPlayerId = (sbyte)playerInfo.PlayerId;
			obj.SetDead(playerInfo.PlayerId == PlayerControl.LocalPlayer.PlayerId, reporter == playerInfo.PlayerId, playerInfo.Disconnected || playerInfo.IsDead);
		}
		SortButtons();
	}

	private void SortButtons()
	{
		PlayerVoteArea[] array = (from p in playerStates
			orderby p.isDead ? 50 : 0, p.TargetPlayerId
			select p).ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			int num = i % 2;
			int num2 = i / 2;
			array[i].transform.localPosition = VoteOrigin + new Vector3(VoteButtonOffsets.x * (float)num, (VoteButtonOffsets.y / 2) * (float)num2 + 0.20f, -1f);
		}
	}

	private PlayerVoteArea CreateButton(GameData.PlayerInfo playerInfo)
	{
		PlayerVoteArea playerVoteArea = Object.Instantiate(PlayerButtonPrefab, ButtonParent.transform);
		PlayerControl.SetPlayerMaterialColors(playerInfo.ColorId, playerVoteArea.PlayerIcon);
		playerVoteArea.PlayerIcon.transform.localScale = new Vector3(0.5f, 1f, 1f);
		playerVoteArea.NameText.Text = playerInfo.PlayerName;
		playerVoteArea.NameText.transform.localScale = new Vector3(0.5f, 1.0f, 1.0f);
		bool flag = (((PlayerControl.LocalPlayer.Data.IsImpostor || CE_RoleManager.GetRoleFromID(PlayerControl.LocalPlayer.Data.role).CanSeeImps) && playerInfo.IsImpostor) || (PlayerControl.LocalPlayer.Data.IsDead && playerInfo.IsImpostor) && PlayerControl.GameOptions.GhostsSeeRoles);
		CE_Role playerrole = CE_RoleManager.GetRoleFromID(playerInfo.role);
		playerVoteArea.NameText.Color = ((flag && (PlayerControl.GameOptions.CanSeeOtherImps || playerInfo == PlayerControl.LocalPlayer.Data)) ? Palette.ImpostorRed : Color.white);
		if ((playerrole.CanSee(PlayerControl.LocalPlayer.Data) || playerInfo == PlayerControl.LocalPlayer.Data) && playerInfo.role != 0 && !flag)
		{
			playerVoteArea.NameText.Color = playerrole.RoleColor;
		}
		//prioritize showing the impostor over their role
		playerVoteArea.transform.localScale = new Vector3(1f, 0.5f, 1f);
		return playerVoteArea;
	}

	public void RpcClose()
	{
		if (AmongUsClient.Instance.AmClient)
		{
			Close();
		}
		AmongUsClient.Instance.SendRpc(NetId, 0);
	}

	public void CmdCastVote(byte playerId, sbyte suspectIdx)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			CastVote(playerId, suspectIdx);
			return;
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(NetId, 2, SendOption.Reliable, AmongUsClient.Instance.HostId);
		messageWriter.Write(playerId);
		messageWriter.Write(suspectIdx);
		AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
	}

	private void RpcVotingComplete(byte[] states, GameData.PlayerInfo exiled, bool tie)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			VotingComplete(states, exiled, tie);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 1);
		messageWriter.WriteBytesAndSize(states);
		messageWriter.Write(exiled?.PlayerId ?? byte.MaxValue);
		messageWriter.Write(tie);
		messageWriter.EndMessage();
	}

	private void RpcClearVote(int clientId)
	{
		if (AmongUsClient.Instance.ClientId == clientId)
		{
			ClearVote();
			return;
		}
		MessageWriter msg = AmongUsClient.Instance.StartRpcImmediately(NetId, 3, SendOption.Reliable, clientId);
		AmongUsClient.Instance.FinishRpcImmediately(msg);
	}

	public override void HandleRpc(byte callId, MessageReader reader)
	{
		switch (callId)
		{
		case 0:
			Close();
			break;
		case 2:
		{
			byte srcPlayerId = reader.ReadByte();
			sbyte suspectPlayerId = reader.ReadSByte();
			CastVote(srcPlayerId, suspectPlayerId);
			break;
		}
		case 1:
		{
			byte[] states = reader.ReadBytesAndSize();
			GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(reader.ReadByte());
			bool tie = reader.ReadBoolean();
			VotingComplete(states, playerById, tie);
			break;
		}
		case 3:
			ClearVote();
			break;
		}
	}

	private void UpdateIcons(PlayerVoteArea playerVoteArea)
	{
		if (SaveManager.UseLegacyVoteIcons)
		{
			playerVoteArea.PlayerIcon.transform.localScale = new Vector3(0.5f, 1f, 1f);
		}
		else
		{
			playerVoteArea.PlayerIcon.transform.localScale = new Vector3(0.35f, 0.6f, 1f);
		}
	}
}
