using InnerNet;

public class DisconnectPopup : DestroyableSingleton<DisconnectPopup>
{
	public TextRenderer TextArea;

	public void Start()
	{
		if (DestroyableSingleton<DisconnectPopup>.Instance == this)
		{
			Show();
		}
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
		DoShow();
	}

	private void DoShow()
	{
		if (DestroyableSingleton<WaitForHostPopup>.InstanceExists)
		{
			DestroyableSingleton<WaitForHostPopup>.Instance.Hide();
		}
		if (!AmongUsClient.Instance)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		string text = InnerNetClient.IntToGameName(AmongUsClient.Instance.GameId);
		string str = ((text != null) ? (" of " + text) : "");
		switch (AmongUsClient.Instance.LastDisconnectReason)
		{
		case DisconnectReasons.ServerFull:
			TextArea.Text = "The Among Us servers are overloaded.\r\n\r\nSorry! Please try again later!";
			break;
		case DisconnectReasons.IntentionalLeaving:
			TextArea.Text = $"You may not join another game for another {SaveManager.BanMinutesLeft} minutes after intentionally disconnecting.";
			break;
		case DisconnectReasons.FocusLost:
			TextArea.Text = "You were disconnected because Among Us was suspended by another app.";
			break;
		case DisconnectReasons.Banned:
			TextArea.Text = "You were banned by the host" + str + ".\r\n\r\nYou cannot rejoin that game.";
			break;
		case DisconnectReasons.Kicked:
			TextArea.Text = "You were kicked by the host" + str + ".\r\n\r\nYou can rejoin if the game hasn't started, but try to understand why you were kicked.";
			break;
		case DisconnectReasons.GameFull:
			TextArea.Text = "The game you tried to join is full.\r\n\r\nCheck with the host to see if you can join next round.";
			break;
		case DisconnectReasons.GameStarted:
			TextArea.Text = "The game you tried to join already started.\r\n\r\nCheck with the host to see if you can join next round.";
			break;
		case DisconnectReasons.GameNotFound:
		case DisconnectReasons.IncorrectGame:
			TextArea.Text = "Could not find the game you're looking for.";
			break;
		case DisconnectReasons.ServerRequest:
			TextArea.Text = "The server stopped this game. Possibly due to inactivity.";
			break;
		case DisconnectReasons.Error:
			if (AmongUsClient.Instance.GameMode == GameModes.OnlineGame)
			{
				TextArea.Text = "You disconnected from the server.\r\nIf this happens often, check your network strength.\r\nThis may also be a server issue.";
			}
			else
			{
				TextArea.Text = "You disconnected from the host.\r\n\r\nIf this happens often, check your WiFi strength.";
			}
			break;
		case DisconnectReasons.Custom:
			TextArea.Text = AmongUsClient.Instance.LastCustomDisconnect ?? "An unknown error disconnected you from the server.";
			break;
		case DisconnectReasons.IncorrectVersion:
			TextArea.Text = "You are running an older version of the game.\r\n\r\nPlease update to play with others.";
			break;
		case DisconnectReasons.ExitGame:
		case DisconnectReasons.Destroy:
			base.gameObject.SetActive(value: false);
			break;
		}
	}

	public void ShowCustom(string message)
	{
		base.gameObject.SetActive(value: true);
		TextArea.Text = message;
	}
}
