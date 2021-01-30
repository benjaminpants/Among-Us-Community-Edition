using System.Collections;
using InnerNet;
using PowerTools;
using UnityEngine;

public class CreateGameOptions : MonoBehaviour, IConnectButton
{
	public AudioClip IntroMusic;

	public GameObject Content;

	public SpriteRenderer Foreground;

	public SpriteAnim connectIcon;

	public AnimationClip connectClip;

	public void Show()
	{
		if (!NameTextBehaviour.Instance.ShakeIfInvalid())
		{
				base.gameObject.SetActive(value: true);
				Content.SetActive(value: false);
				StartCoroutine(CoShow());
		}
	}

	private IEnumerator CoShow()
	{
		yield return Effects.ColorFade(Foreground, Color.clear, Color.black, 0.1f);
		Content.SetActive(value: true);
		yield return Effects.ColorFade(Foreground, Color.black, Color.clear, 0.1f);
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

	public void Hide()
	{
		StartCoroutine(CoHide());
	}

	private IEnumerator CoHide()
	{
		yield return Effects.ColorFade(Foreground, Color.clear, Color.black, 0.1f);
		Content.SetActive(value: false);
		yield return Effects.ColorFade(Foreground, Color.black, Color.clear, 0.1f);
		base.gameObject.SetActive(value: false);
	}

	public void Confirm()
	{
		if (DestroyableSingleton<MatchMaker>.Instance.Connecting(this))
		{
			StartCoroutine(CoStartGame());
		}
	}

	private IEnumerator CoStartGame()
	{
		SoundManager.Instance.CrossFadeSound("MainBG", null, 0.5f);
		yield return Effects.ColorFade(Foreground, Color.clear, Color.black, 0.2f);
		AmongUsClient.Instance.GameMode = GameModes.OnlineGame;
		AmongUsClient.Instance.SetEndpoint(DestroyableSingleton<ServerManager>.Instance.OnlineNetAddress, (ushort)DestroyableSingleton<ServerManager>.Instance.LastPort);
		AmongUsClient.Instance.MainMenuScene = "MMOnline";
		AmongUsClient.Instance.OnlineScene = "OnlineGame";
		AmongUsClient.Instance.Connect(MatchMakerModes.HostAndClient);
		yield return AmongUsClient.Instance.WaitForConnectionOrFail();
		DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
		if (AmongUsClient.Instance.mode == MatchMakerModes.None)
		{
			SoundManager.Instance.CrossFadeSound("MainBG", IntroMusic, 0.5f);
			yield return Effects.ColorFade(Foreground, Color.black, Color.clear, 0.2f);
		}
	}
}
