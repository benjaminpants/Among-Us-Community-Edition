using System.Collections;
using InnerNet;
using PowerTools;
using UnityEngine;

public class JoinGameButton : MonoBehaviour, IConnectButton
{
	public AudioClip IntroMusic;

	public TextBox GameIdText;

	public TextRenderer gameNameText;

	public float timeRecieved;

	public SpriteRenderer FillScreen;

	public SpriteAnim connectIcon;

	public AnimationClip connectClip;

	public GameModes GameMode;

	public string netAddress;


	public void OnClick()
	{
		if (string.IsNullOrWhiteSpace(netAddress) || NameTextBehaviour.Instance.ShakeIfInvalid())
		{
			return;
		}
		if (false)
		{
			AmongUsClient.Instance.LastDisconnectReason = DisconnectReasons.IntentionalLeaving;
			DestroyableSingleton<DisconnectPopup>.Instance.Show();
		}
		else
		{
			if (!DestroyableSingleton<MatchMaker>.Instance.Connecting(this))
			{
				return;
			}
			AmongUsClient.Instance.GameMode = GameMode;
			if (GameMode == GameModes.OnlineGame)
			{
				AmongUsClient.Instance.SetEndpoint(DestroyableSingleton<ServerManager>.Instance.OnlineNetAddress, Constants.ServersPort);
				AmongUsClient.Instance.MainMenuScene = "MMOnline";
				int num = InnerNetClient.GameNameToInt(GameIdText.text);
				if (num == -1)
				{
					StartCoroutine(Effects.Shake(GameIdText.transform));
					DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
					return;
				}
				AmongUsClient.Instance.GameId = num;
			}
			else
			{
				AmongUsClient.Instance.SetEndpoint(netAddress, 22023);
				AmongUsClient.Instance.GameId = 32;
				AmongUsClient.Instance.GameMode = GameModes.LocalGame;
				AmongUsClient.Instance.MainMenuScene = "MatchMaking";
			}
			StartCoroutine(JoinGame());
		}
	}

	private IEnumerator JoinGame()
	{
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
		AmongUsClient.Instance.OnlineScene = "OnlineGame";
		AmongUsClient.Instance.Connect(MatchMakerModes.Client);
		yield return AmongUsClient.Instance.WaitForConnectionOrFail();
		if (AmongUsClient.Instance.mode != 0)
		{
			yield break;
		}
		if ((bool)FillScreen)
		{
			SoundManager.Instance.CrossFadeSound("MainBG", IntroMusic, 0.5f);
			for (float time2 = 0f; time2 < 0.25f; time2 += Time.deltaTime)
			{
				FillScreen.color = Color.Lerp(Color.black, Color.clear, time2 / 0.25f);
				yield return null;
			}
			FillScreen.color = Color.clear;
		}
		DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
	}

	public void SetGameName(string[] gameNameParts)
	{
		gameNameText.Text = gameNameParts[0] + " (" + gameNameParts[2] + "/20)";
	}

	public void StartIcon()
	{
		connectIcon.Play(connectClip);
	}

	public void StopIcon()
	{
		connectIcon.Stop();
		connectIcon.GetComponent<SpriteRenderer>().sprite = null;
	}
}
