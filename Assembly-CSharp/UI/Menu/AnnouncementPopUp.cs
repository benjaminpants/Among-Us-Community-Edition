using System;
using System.Collections;
using System.Net;
using Hazel;
using Hazel.Udp;
using UnityEngine;

public class AnnouncementPopUp : MonoBehaviour
{
	private enum AnnounceState
	{
		Fetching,
		Failed,
		Success,
		Cached
	}

	private const uint AnnouncementVersion = 1u;

	private UdpClientConnection connection;

	private static AnnounceState AskedForUpdate;

	public TextRenderer AnnounceText;

	private Announcement announcementUpdate;

	private bool LocalMode = false;

	private static bool IsSuccess(AnnounceState state)
	{
		if (state != AnnounceState.Success)
		{
			return state == AnnounceState.Cached;
		}
		return true;
	}

	public IEnumerator Init()
	{
		return Init_Original();
	}

	public IEnumerator Init_CE()
	{
		return null;
	}

	public IEnumerator Init_Original()
    {
		if (AskedForUpdate == AnnounceState.Fetching)
		{
			yield return DestroyableSingleton<ServerManager>.Instance.WaitForServers();
			connection = new UdpClientConnection(new IPEndPoint(IPAddress.Parse(DestroyableSingleton<ServerManager>.Instance.OnlineNetAddress), 22024));
			connection.DataReceived += Connection_DataReceived;
			connection.Disconnected += Connection_Disconnected;
			Announcement lastAnnouncement = SaveManager.LastAnnouncement;
			try
			{
				MessageWriter messageWriter = MessageWriter.Get();
				messageWriter.WritePacked(1u);
				messageWriter.WritePacked(lastAnnouncement.Id);
				connection.ConnectAsync(messageWriter.ToByteArray(includeHeader: true));
				messageWriter.Recycle();
			}
			catch
			{
				AskedForUpdate = AnnounceState.Failed;
			}
		}
	}

	private void Connection_Disconnected(object sender, DisconnectedEventArgs e)
	{
		AskedForUpdate = AnnounceState.Failed;
		connection.Dispose();
		connection = null;
	}

	private void Connection_DataReceived(DataReceivedEventArgs e)
	{
		MessageReader message = e.Message;
		try
		{
			if (message.Length > 4)
			{
				announcementUpdate = default(Announcement);
				announcementUpdate.DateFetched = DateTime.UtcNow.DayOfYear;
				announcementUpdate.Id = message.ReadPackedUInt32();
				announcementUpdate.AnnounceText = message.ReadString();
				AskedForUpdate = AnnounceState.Success;
			}
			else
			{
				AskedForUpdate = AnnounceState.Cached;
			}
		}
		finally
		{
			message.Recycle();
		}
		try
		{
			connection.Dispose();
			connection = null;
		}
		catch
		{
		}
	}

	public IEnumerator Show()
	{
		return Show_Original();
	}

	public IEnumerator Show_Original()
    {
		float timer = 0f;
		while (AskedForUpdate == AnnounceState.Fetching && connection != null && timer < 5f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		if (!IsSuccess(AskedForUpdate))
		{
			Announcement lastAnnouncement = SaveManager.LastAnnouncement;
			if (lastAnnouncement.Id == 0)
			{
				AnnounceText.Text = "Couldn't get announcement.";
			}
			else
			{
				AnnounceText.Text = "Couldn't get announcement. Last Known:\r\n" + lastAnnouncement.AnnounceText;
			}
		}
		else if (announcementUpdate.Id != SaveManager.LastAnnouncement.Id)
		{
			if (AskedForUpdate != AnnounceState.Cached)
			{
				base.gameObject.SetActive(value: true);
			}
			if (announcementUpdate.Id == 0)
			{
				announcementUpdate = SaveManager.LastAnnouncement;
				announcementUpdate.DateFetched = DateTime.UtcNow.DayOfYear;
			}
			SaveManager.LastAnnouncement = announcementUpdate;
			AnnounceText.Text = announcementUpdate.AnnounceText;
		}
		while (base.gameObject.activeSelf)
		{
			yield return null;
		}
	}

	public IEnumerator Show_CE()
    {
		return null;
    }

	public void Close()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		if (connection != null)
		{
			connection.Dispose();
			connection = null;
		}
	}
}
