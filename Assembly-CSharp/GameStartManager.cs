using System;
using InnerNet;
using UnityEngine;

// Token: 0x02000098 RID: 152
public class GameStartManager : DestroyableSingleton<GameStartManager>, IDisconnectHandler
{
	// Token: 0x0600032F RID: 815 RVA: 0x000171DC File Offset: 0x000153DC
	public void Start()
	{
		if (DestroyableSingleton<TutorialManager>.InstanceExists)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		string text = InnerNetClient.IntToGameName(AmongUsClient.Instance.GameId);
		if (text != null)
		{
			this.GameRoomName.Text = "Room\r\n" + text;
		}
		else
		{
			this.StartButton.transform.localPosition = new Vector3(0f, -0.2f, 0f);
			this.PlayerCounter.transform.localPosition = new Vector3(0f, -0.8f, 0f);
		}
		AmongUsClient.Instance.DisconnectHandlers.AddUnique(this);
		if (!AmongUsClient.Instance.AmHost)
		{
			this.StartButton.gameObject.SetActive(false);
		}
		else
		{
			LobbyBehaviour.Instance = UnityEngine.Object.Instantiate<LobbyBehaviour>(this.LobbyPrefab);
			AmongUsClient.Instance.Spawn(LobbyBehaviour.Instance, -2, SpawnFlags.None);
		}
		this.MakePublicButton.gameObject.SetActive(AmongUsClient.Instance.GameMode == GameModes.OnlineGame);
	}

	// Token: 0x06000330 RID: 816 RVA: 0x0000415C File Offset: 0x0000235C
	public void MakePublic()
	{
		if (AmongUsClient.Instance.AmHost)
		{
			AmongUsClient.Instance.ChangeGamePublic(!AmongUsClient.Instance.IsGamePublic);
		}
	}

	// Token: 0x06000331 RID: 817 RVA: 0x000172E0 File Offset: 0x000154E0
	public void Update()
	{
		if (!GameData.Instance)
		{
			return;
		}
		this.MakePublicButton.sprite = (AmongUsClient.Instance.IsGamePublic ? this.PublicGameImage : this.PrivateGameImage);
		if (GameData.Instance.PlayerCount != this.LastPlayerCount)
		{
			this.LastPlayerCount = GameData.Instance.PlayerCount;
			string arg = "[FF0000FF]";
			if (this.LastPlayerCount > this.MinPlayers)
			{
				arg = "[00FF00FF]";
			}
			if (this.LastPlayerCount == this.MinPlayers)
			{
				arg = "[FFFF00FF]";
			}
			this.PlayerCounter.Text = string.Format("{0}{1}/{2}", arg, this.LastPlayerCount, PlayerControl.GameOptions.MaxPlayers);
			this.StartButton.color = ((this.LastPlayerCount >= this.MinPlayers) ? Palette.EnabledColor : Palette.DisabledColor);
			if (DestroyableSingleton<DiscordManager>.InstanceExists)
			{
				if (AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode == GameModes.OnlineGame)
				{
					DestroyableSingleton<DiscordManager>.Instance.SetInLobbyHost(this.LastPlayerCount, AmongUsClient.Instance.GameId);
					return;
				}
				DestroyableSingleton<DiscordManager>.Instance.SetInLobbyClient();
			}
		}
	}

	// Token: 0x06000332 RID: 818 RVA: 0x0001740C File Offset: 0x0001560C
	public void BeginGame()
	{
		if (SaveManager.ShowMinPlayerWarning && GameData.Instance.PlayerCount == this.MinPlayers)
		{
			this.GameSizePopup.SetActive(true);
			return;
		}
		if (GameData.Instance.PlayerCount < this.MinPlayers)
		{
			base.StartCoroutine(Effects.Shake(this.PlayerCounter.transform, 0.75f, 0.25f));
			return;
		}
		this.ReallyBegin(false);
	}

	// Token: 0x06000333 RID: 819 RVA: 0x0001747C File Offset: 0x0001567C
	public void ReallyBegin(bool neverShow)
	{
		if (this.starting)
		{
			return;
		}
		this.starting = true;
		if (neverShow)
		{
			SaveManager.ShowMinPlayerWarning = false;
		}
		AmongUsClient.Instance.StartGame();
		AmongUsClient.Instance.DisconnectHandlers.Remove(this);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x06000334 RID: 820 RVA: 0x00004181 File Offset: 0x00002381
	public void HandleDisconnect(PlayerControl pc, DisconnectReasons reason)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			this.LastPlayerCount = -1;
			if (this.StartButton)
			{
				this.StartButton.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06000335 RID: 821 RVA: 0x000041B4 File Offset: 0x000023B4
	public void HandleDisconnect()
	{
		this.HandleDisconnect(null, DisconnectReasons.ExitGame);
	}

	// Token: 0x04000327 RID: 807
	public int MinPlayers = 4;

	// Token: 0x04000328 RID: 808
	public TextRenderer PlayerCounter;

	// Token: 0x04000329 RID: 809
	private int LastPlayerCount = -1;

	// Token: 0x0400032A RID: 810
	public GameObject GameSizePopup;

	// Token: 0x0400032B RID: 811
	public TextRenderer GameRoomName;

	// Token: 0x0400032C RID: 812
	public PlayerControl PlayerPrefab;

	// Token: 0x0400032D RID: 813
	public LobbyBehaviour LobbyPrefab;

	// Token: 0x0400032E RID: 814
	public SpriteRenderer StartButton;

	// Token: 0x0400032F RID: 815
	public SpriteRenderer MakePublicButton;

	// Token: 0x04000330 RID: 816
	public Sprite PublicGameImage;

	// Token: 0x04000331 RID: 817
	public Sprite PrivateGameImage;

	// Token: 0x04000332 RID: 818
	public bool starting;
}
