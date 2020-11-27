using System;
using InnerNet;

// Token: 0x02000076 RID: 118
public class DisconnectPopup : DestroyableSingleton<DisconnectPopup>
{
	// Token: 0x0600027B RID: 635 RVA: 0x000038EA File Offset: 0x00001AEA
	public void Start()
	{
		if (DestroyableSingleton<DisconnectPopup>.Instance == this)
		{
			this.Show();
		}
	}

	// Token: 0x0600027C RID: 636 RVA: 0x000038FF File Offset: 0x00001AFF
	public void Show()
	{
		base.gameObject.SetActive(true);
		this.DoShow();
	}

	// Token: 0x0600027D RID: 637 RVA: 0x00014050 File Offset: 0x00012250
	private void DoShow()
	{
		if (DestroyableSingleton<WaitForHostPopup>.InstanceExists)
		{
			DestroyableSingleton<WaitForHostPopup>.Instance.Hide();
		}
		if (!AmongUsClient.Instance)
		{
			base.gameObject.SetActive(false);
			return;
		}
		string text = InnerNetClient.IntToGameName(AmongUsClient.Instance.GameId);
		string str = (text != null) ? (" of " + text) : "";
		DisconnectReasons lastDisconnectReason = AmongUsClient.Instance.LastDisconnectReason;
		switch (lastDisconnectReason)
		{
		case DisconnectReasons.ExitGame:
		case DisconnectReasons.Destroy:
			base.gameObject.SetActive(false);
			break;
		case DisconnectReasons.GameFull:
			this.TextArea.Text = "The game you tried to join is full.\r\n\r\nCheck with the host to see if you can join next round.";
			return;
		case DisconnectReasons.GameStarted:
			this.TextArea.Text = "The game you tried to join already started.\r\n\r\nCheck with the host to see if you can join next round.";
			return;
		case DisconnectReasons.GameNotFound:
		case DisconnectReasons.IncorrectGame:
			this.TextArea.Text = "Could not find the game you're looking for.";
			return;
		case (DisconnectReasons)4:
		case (DisconnectReasons)9:
		case (DisconnectReasons)10:
		case (DisconnectReasons)11:
		case (DisconnectReasons)12:
		case (DisconnectReasons)13:
		case (DisconnectReasons)14:
		case (DisconnectReasons)15:
			break;
		case DisconnectReasons.IncorrectVersion:
			this.TextArea.Text = "You are running an older version of the game.\r\n\r\nPlease update to play with others.";
			return;
		case DisconnectReasons.Banned:
			this.TextArea.Text = "You were banned by the host" + str + ".\r\n\r\nYou cannot rejoin that game.";
			return;
		case DisconnectReasons.Kicked:
			this.TextArea.Text = "You were kicked by the host" + str + ".\r\n\r\nYou can rejoin if the game hasn't started, but try to understand why you were kicked.";
			return;
		case DisconnectReasons.Custom:
			this.TextArea.Text = (AmongUsClient.Instance.LastCustomDisconnect ?? "An unknown error disconnected you from the server.");
			return;
		case DisconnectReasons.Error:
			if (AmongUsClient.Instance.GameMode == GameModes.OnlineGame)
			{
				this.TextArea.Text = "You disconnected from the server.\r\nIf this happens often, check your network strength.\r\nThis may also be a server issue.";
				return;
			}
			this.TextArea.Text = "You disconnected from the host.\r\n\r\nIf this happens often, check your WiFi strength.";
			return;
		case DisconnectReasons.ServerRequest:
			this.TextArea.Text = "The server stopped this game. Possibly due to inactivity.";
			return;
		case DisconnectReasons.ServerFull:
			this.TextArea.Text = "The Among Us servers are overloaded.\r\n\r\nSorry! Please try again later!";
			return;
		default:
			if (lastDisconnectReason == DisconnectReasons.IntentionalLeaving)
			{
				this.TextArea.Text = string.Format("You may not join another game for another {0} minutes after intentionally disconnecting.", SaveManager.BanMinutesLeft);
				return;
			}
			if (lastDisconnectReason != DisconnectReasons.FocusLost)
			{
				return;
			}
			this.TextArea.Text = "You were disconnected because Among Us was suspended by another app.";
			return;
		}
	}

	// Token: 0x0600027E RID: 638 RVA: 0x00003913 File Offset: 0x00001B13
	public void ShowCustom(string message)
	{
		base.gameObject.SetActive(true);
		this.TextArea.Text = message;
	}

	// Token: 0x04000279 RID: 633
	public TextRenderer TextArea;
}
