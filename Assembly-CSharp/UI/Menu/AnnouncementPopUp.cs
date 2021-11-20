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

	#region CE Variables

	private bool CE_LocalMode = true;

    #endregion

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
		return Init_CE();
	}
	public IEnumerator Show()
	{
		return Show_CE();
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

	#region Connection Methods
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
				Announcment_SetData(message);
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
	#endregion

	#region Announcment Methods
	private void Announcment_SetData(MessageReader message)
    {
		Announcement announcement = default(Announcement);
		var DateFetched = DateTime.UtcNow.DayOfYear;
		var Id = message.ReadPackedUInt32();
		var AnnounceText = message.ReadString();
		Announcment_SetData(announcement, DateFetched, Id, AnnounceText);
	}
	private void Announcment_SetData(Announcement announcement, int dateFetched, uint id, string announceText)
    {
		announcementUpdate = announcement;
		announcementUpdate.DateFetched = dateFetched;
		announcementUpdate.Id = id;
		announcementUpdate.AnnounceText = announceText;
	}

	#endregion

	#region Original Methods
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
	public IEnumerator Show_Original()
	{
		float timer = 0f;
		while (AskedForUpdate == AnnounceState.Fetching && connection != null && timer < 5f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		if (!IsSuccess(AskedForUpdate)) Original_SetAnnouncmentTextFail();
		else if (announcementUpdate.Id != SaveManager.LastAnnouncement.Id) Original_UpdateLastAnnouncementID();
		while (base.gameObject.activeSelf) yield return null;
	}
	public void Original_SetAnnouncmentTextFail()
    {
		Announcement lastAnnouncement = SaveManager.LastAnnouncement;
		if (lastAnnouncement.Id == 0) AnnounceText.Text = "Couldn't get announcement.";
		else AnnounceText.Text = "Couldn't get announcement. Last Known:\r\n" + lastAnnouncement.AnnounceText;
	}
	public void Original_UpdateLastAnnouncementID()
	{
		if (AskedForUpdate != AnnounceState.Cached) base.gameObject.SetActive(value: true);
		if (announcementUpdate.Id == 0)
		{
			announcementUpdate = SaveManager.LastAnnouncement;
			announcementUpdate.DateFetched = DateTime.UtcNow.DayOfYear;
		}
		SaveManager.LastAnnouncement = announcementUpdate;
		AnnounceText.Text = announcementUpdate.AnnounceText;
	}
	#endregion

	#region CE Methods
	public IEnumerator Init_CE()
	{
		if (CE_LocalMode) return CE_GetLocalAnnouncments();
		else return CE_GetNetAnnouncments();
	}
	public IEnumerator CE_GetLocalAnnouncments()
    {		
		Announcement announcement = default(Announcement);
		int DateFetched = DateTime.UtcNow.DayOfYear;
		uint Id = 1;
		string AnnounceText = "11/20/2021 Fixed bugs new hats and skins a brand new gamemode removed debug.lua and more!";
		Announcment_SetData(announcement, DateFetched, Id, AnnounceText);
		AskedForUpdate = AnnounceState.Success;
		yield return null;
	}
	public IEnumerator CE_GetNetAnnouncments()
    {
		//TODO: Change Functionality to Something Unique that isn't just the original code for Init();
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
	public IEnumerator Show_CE()
	{
		float timer = 0f;
		while (AskedForUpdate == AnnounceState.Fetching && connection != null && timer < 5f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		if (!IsSuccess(AskedForUpdate)) Original_SetAnnouncmentTextFail();
		else if (announcementUpdate.Id != SaveManager.LastAnnouncement.Id) Original_UpdateLastAnnouncementID();
		while (base.gameObject.activeSelf) yield return null;
	}
	#endregion
}
