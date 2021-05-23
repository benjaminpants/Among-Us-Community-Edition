using System;
using System.Collections;
using InnerNet;
using PowerTools;
using UnityEngine;

public class HostGameButton : MonoBehaviour, IConnectButton
{
	public AudioClip IntroMusic;

	public string targetScene;

	public SpriteRenderer FillScreen;

	public SpriteAnim connectIcon;

	public AnimationClip connectClip;

	public GameModes GameMode;

	public void Start()
	{
		if (DestroyableSingleton<MatchMaker>.InstanceExists)
		{
			DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
		}
	}

	public void OnClick()
	{
		if (GameMode == GameModes.FreePlay)
		{
			if (!NameTextBehaviour.IsValidName(SaveManager.PlayerName))
			{
				SaveManager.PlayerName = "Player";
			}
		}
		else
		{
			if (NameTextBehaviour.Instance.ShakeIfInvalid())
			{
				return;
			}
			if (false)
			{
				AmongUsClient.Instance.LastDisconnectReason = DisconnectReasons.IntentionalLeaving;
				DestroyableSingleton<DisconnectPopup>.Instance.Show();
				return;
			}
			if (!DestroyableSingleton<MatchMaker>.Instance.Connecting(this))
			{
				return;
			}
		}
		StartCoroutine(CoStartGame());
	}

	public void StartIcon()
	{
		if ((bool)connectIcon)
		{
			connectIcon.Play(connectClip);
		}
	}

	public void StopIcon()
	{
		if ((bool)connectIcon)
		{
			connectIcon.Stop();
			connectIcon.GetComponent<SpriteRenderer>().sprite = null;
		}
	}

	private IEnumerator CoStartGame()
	{
		try
		{
			SoundManager.Instance.StopAllSound();
			AmongUsClient.Instance.GameMode = GameMode;
			switch (GameMode)
			{
			case GameModes.LocalGame:
				DestroyableSingleton<InnerNetServer>.Instance.StartAsServer();
				AmongUsClient.Instance.SetEndpoint("127.0.0.1", (ushort)ServerManager.LanGamePort);
				AmongUsClient.Instance.MainMenuScene = "MatchMaking";
				break;
			case GameModes.OnlineGame:
				AmongUsClient.Instance.SetEndpoint(DestroyableSingleton<ServerManager>.Instance.OnlineNetAddress, (ushort)DestroyableSingleton<ServerManager>.Instance.LastPort);
				AmongUsClient.Instance.MainMenuScene = "MMOnline";
				break;
			case GameModes.FreePlay:
				DestroyableSingleton<InnerNetServer>.Instance.StartAsServer();
				AmongUsClient.Instance.SetEndpoint("127.0.0.1", 22023);
				AmongUsClient.Instance.MainMenuScene = "MainMenu";
				break;
			}
		}
		catch (Exception ex)
		{
			DestroyableSingleton<DisconnectPopup>.Instance.ShowCustom(ex.Message);
			DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
			yield break;
		}
		yield return new WaitForSeconds(0.1f);
		if ((bool)FillScreen)
		{
			SoundManager.Instance.CrossFadeSound("MainBG", null, 0.5f);
			FillScreen.gameObject.SetActive(value: true);
			for (float time2 = 0f; time2 < 0.25f; time2 += Time.deltaTime)
			{
				FillScreen.color = Color.Lerp(Color.clear, Color.black, time2 / 0.25f);
				yield return null;
			}
			FillScreen.color = Color.black;
		}
		AmongUsClient.Instance.OnlineScene = targetScene;
		AmongUsClient.Instance.Connect(MatchMakerModes.HostAndClient);
		yield return AmongUsClient.Instance.WaitForConnectionOrFail();
		DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
		if (AmongUsClient.Instance.mode == MatchMakerModes.None && (bool)FillScreen)
		{
			SoundManager.Instance.CrossFadeSound("MainBG", IntroMusic, 0.5f);
			for (float time2 = 0f; time2 < 0.25f; time2 += Time.deltaTime)
			{
				FillScreen.color = Color.Lerp(Color.black, Color.clear, time2 / 0.25f);
				yield return null;
			}
			FillScreen.color = Color.clear;
		}
	}
}
