using System;
using System.Collections;
using InnerNet;
using PowerTools;
using UnityEngine;

// Token: 0x02000140 RID: 320
public class JoinGameButton : MonoBehaviour, IConnectButton
{
	// Token: 0x060006BA RID: 1722 RVA: 0x00027CAC File Offset: 0x00025EAC
	public void OnClick()
	{
		if (string.IsNullOrWhiteSpace(this.netAddress))
		{
			return;
		}
		if (NameTextBehaviour.Instance.ShakeIfInvalid())
		{
			return;
		}
		if (SaveManager.AmBanned)
		{
			AmongUsClient.Instance.LastDisconnectReason = DisconnectReasons.IntentionalLeaving;
			DestroyableSingleton<DisconnectPopup>.Instance.Show();
			return;
		}
		if (!DestroyableSingleton<MatchMaker>.Instance.Connecting(this))
		{
			return;
		}
		AmongUsClient.Instance.GameMode = this.GameMode;
		if (this.GameMode == GameModes.OnlineGame)
		{
			AmongUsClient.Instance.SetEndpoint(DestroyableSingleton<ServerManager>.Instance.OnlineNetAddress, 25565);
			AmongUsClient.Instance.MainMenuScene = "MMOnline";
			int num = InnerNetClient.GameNameToInt(this.GameIdText.text);
			if (num == -1)
			{
				base.StartCoroutine(Effects.Shake(this.GameIdText.transform, 0.75f, 0.25f));
				DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
				return;
			}
			AmongUsClient.Instance.GameId = num;
		}
		else
		{
			AmongUsClient.Instance.SetEndpoint(this.netAddress, 22023);
			AmongUsClient.Instance.GameId = 32;
			AmongUsClient.Instance.GameMode = GameModes.LocalGame;
			AmongUsClient.Instance.MainMenuScene = "MatchMaking";
		}
		base.StartCoroutine(this.JoinGame());
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x00006333 File Offset: 0x00004533
	private IEnumerator JoinGame()
	{
		if (this.FillScreen)
		{
			SoundManager.Instance.CrossFadeSound("MainBG", null, 0.5f, 1.5f);
			this.FillScreen.gameObject.SetActive(true);
			for (float time = 0f; time < 0.25f; time += Time.deltaTime)
			{
				this.FillScreen.color = Color.Lerp(Color.clear, Color.black, time / 0.25f);
				yield return null;
			}
			this.FillScreen.color = Color.black;
		}
		AmongUsClient.Instance.OnlineScene = "OnlineGame";
		AmongUsClient.Instance.Connect(MatchMakerModes.Client);
		yield return AmongUsClient.Instance.WaitForConnectionOrFail();
		if (AmongUsClient.Instance.mode == MatchMakerModes.None)
		{
			if (this.FillScreen)
			{
				SoundManager.Instance.CrossFadeSound("MainBG", this.IntroMusic, 0.5f, 1.5f);
				for (float time = 0f; time < 0.25f; time += Time.deltaTime)
				{
					this.FillScreen.color = Color.Lerp(Color.black, Color.clear, time / 0.25f);
					yield return null;
				}
				this.FillScreen.color = Color.clear;
			}
			DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
		}
		yield break;
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x00006342 File Offset: 0x00004542
	public void SetGameName(string[] gameNameParts)
	{
		this.gameNameText.Text = gameNameParts[0] + " (" + gameNameParts[2] + "/20)";
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x00006364 File Offset: 0x00004564
	public void StartIcon()
	{
		this.connectIcon.Play(this.connectClip, 1f);
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x0000637C File Offset: 0x0000457C
	public void StopIcon()
	{
		this.connectIcon.Stop();
		this.connectIcon.GetComponent<SpriteRenderer>().sprite = null;
	}

	// Token: 0x0400067A RID: 1658
	public AudioClip IntroMusic;

	// Token: 0x0400067B RID: 1659
	public TextBox GameIdText;

	// Token: 0x0400067C RID: 1660
	public TextRenderer gameNameText;

	// Token: 0x0400067D RID: 1661
	public float timeRecieved;

	// Token: 0x0400067E RID: 1662
	public SpriteRenderer FillScreen;

	// Token: 0x0400067F RID: 1663
	public SpriteAnim connectIcon;

	// Token: 0x04000680 RID: 1664
	public AnimationClip connectClip;

	// Token: 0x04000681 RID: 1665
	public GameModes GameMode;

	// Token: 0x04000682 RID: 1666
	public string netAddress;
}
