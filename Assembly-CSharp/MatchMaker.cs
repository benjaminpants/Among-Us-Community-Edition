using System;
using InnerNet;
using UnityEngine;

// Token: 0x02000150 RID: 336
public class MatchMaker : DestroyableSingleton<MatchMaker>
{
	// Token: 0x060006FA RID: 1786 RVA: 0x00028C38 File Offset: 0x00026E38
	public void Start()
	{
		if (this.GameIdText && AmongUsClient.Instance)
		{
			this.GameIdText.SetText(InnerNetClient.IntToGameName(AmongUsClient.Instance.GameId) ?? "", "");
		}
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x00028C88 File Offset: 0x00026E88
	public bool Connecting(MonoBehaviour button)
	{
		if (!this.Connecter)
		{
			this.Connecter = button;
			((IConnectButton)this.Connecter).StartIcon();
			return true;
		}
		base.StartCoroutine(Effects.Shake(this.Connecter.transform, 0.75f, 0.25f));
		return false;
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x00006509 File Offset: 0x00004709
	public void NotConnecting()
	{
		if (this.Connecter)
		{
			((IConnectButton)this.Connecter).StopIcon();
			this.Connecter = null;
		}
	}

	// Token: 0x040006C2 RID: 1730
	public TextBox NameText;

	// Token: 0x040006C3 RID: 1731
	public TextBox GameIdText;

	// Token: 0x040006C4 RID: 1732
	private MonoBehaviour Connecter;
}
