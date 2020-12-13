using InnerNet;
using UnityEngine;

public class MatchMaker : DestroyableSingleton<MatchMaker>
{
	public TextBox NameText;

	public TextBox GameIdText;

	private MonoBehaviour Connecter;

	public void Start()
	{
		if ((bool)GameIdText && (bool)AmongUsClient.Instance)
		{
			GameIdText.SetText(InnerNetClient.IntToGameName(AmongUsClient.Instance.GameId) ?? "");
		}
	}

	public bool Connecting(MonoBehaviour button)
	{
		if (!Connecter)
		{
			Connecter = button;
			((IConnectButton)Connecter).StartIcon();
			return true;
		}
		StartCoroutine(Effects.Shake(Connecter.transform));
		return false;
	}

	public void NotConnecting()
	{
		if ((bool)Connecter)
		{
			((IConnectButton)Connecter).StopIcon();
			Connecter = null;
		}
	}
}
