using System;
using System.Collections;
using System.Net;
using Hazel;
using Hazel.Udp;
using UnityEngine;

// Token: 0x02000129 RID: 297
public class AnnouncementPopUp : MonoBehaviour
{
	// Token: 0x06000634 RID: 1588 RVA: 0x00005E83 File Offset: 0x00004083
	private static bool IsSuccess(AnnouncementPopUp.AnnounceState state)
	{
		return state == AnnouncementPopUp.AnnounceState.Success || state == AnnouncementPopUp.AnnounceState.Cached;
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x00005E8F File Offset: 0x0000408F
	public IEnumerator Init()
	{
		if (AnnouncementPopUp.AskedForUpdate != AnnouncementPopUp.AnnounceState.Fetching)
		{
			yield break;
		}
		yield return DestroyableSingleton<ServerManager>.Instance.WaitForServers();
		this.connection = new UdpClientConnection(new IPEndPoint(IPAddress.Parse(DestroyableSingleton<ServerManager>.Instance.OnlineNetAddress), 22024), IPMode.IPv4);
		this.connection.DataReceived += this.Connection_DataReceived;
		this.connection.Disconnected += this.Connection_Disconnected;
		Announcement lastAnnouncement = SaveManager.LastAnnouncement;
		try
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.None);
			messageWriter.WritePacked(1U);
			messageWriter.WritePacked(lastAnnouncement.Id);
			this.connection.ConnectAsync(messageWriter.ToByteArray(true), 5000);
			messageWriter.Recycle();
			yield break;
		}
		catch
		{
			AnnouncementPopUp.AskedForUpdate = AnnouncementPopUp.AnnounceState.Failed;
			yield break;
		}
		yield break;
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x00005E9E File Offset: 0x0000409E
	private void Connection_Disconnected(object sender, DisconnectedEventArgs e)
	{
		AnnouncementPopUp.AskedForUpdate = AnnouncementPopUp.AnnounceState.Failed;
		this.connection.Dispose();
		this.connection = null;
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x00025F60 File Offset: 0x00024160
	private void Connection_DataReceived(DataReceivedEventArgs e)
	{
		MessageReader message = e.Message;
		try
		{
			if (message.Length > 4)
			{
				this.announcementUpdate = default(Announcement);
				this.announcementUpdate.DateFetched = DateTime.UtcNow.DayOfYear;
				this.announcementUpdate.Id = message.ReadPackedUInt32();
				this.announcementUpdate.AnnounceText = message.ReadString();
				AnnouncementPopUp.AskedForUpdate = AnnouncementPopUp.AnnounceState.Success;
			}
			else
			{
				AnnouncementPopUp.AskedForUpdate = AnnouncementPopUp.AnnounceState.Cached;
			}
		}
		finally
		{
			message.Recycle();
		}
		try
		{
			this.connection.Dispose();
			this.connection = null;
		}
		catch
		{
		}
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x00005EB8 File Offset: 0x000040B8
	public IEnumerator Show()
	{
		float timer = 0f;
		while (AnnouncementPopUp.AskedForUpdate == AnnouncementPopUp.AnnounceState.Fetching && this.connection != null && timer < 5f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		if (!AnnouncementPopUp.IsSuccess(AnnouncementPopUp.AskedForUpdate))
		{
			Announcement lastAnnouncement = SaveManager.LastAnnouncement;
			if (lastAnnouncement.Id == 0U)
			{
				this.AnnounceText.Text = "Couldn't get announcement.";
			}
			else
			{
				this.AnnounceText.Text = "Couldn't get announcement. Last Known:\r\n" + lastAnnouncement.AnnounceText;
			}
		}
		else if (this.announcementUpdate.Id != SaveManager.LastAnnouncement.Id)
		{
			if (AnnouncementPopUp.AskedForUpdate != AnnouncementPopUp.AnnounceState.Cached)
			{
				base.gameObject.SetActive(true);
			}
			if (this.announcementUpdate.Id == 0U)
			{
				this.announcementUpdate = SaveManager.LastAnnouncement;
				this.announcementUpdate.DateFetched = DateTime.UtcNow.DayOfYear;
			}
			SaveManager.LastAnnouncement = this.announcementUpdate;
			this.AnnounceText.Text = this.announcementUpdate.AnnounceText;
		}
		while (base.gameObject.activeSelf)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x00005EC7 File Offset: 0x000040C7
	public void Close()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x00005ED5 File Offset: 0x000040D5
	private void OnDestroy()
	{
		if (this.connection != null)
		{
			this.connection.Dispose();
			this.connection = null;
		}
	}

	// Token: 0x04000621 RID: 1569
	private const uint AnnouncementVersion = 1U;

	// Token: 0x04000622 RID: 1570
	private UdpClientConnection connection;

	// Token: 0x04000623 RID: 1571
	private static AnnouncementPopUp.AnnounceState AskedForUpdate;

	// Token: 0x04000624 RID: 1572
	public TextRenderer AnnounceText;

	// Token: 0x04000625 RID: 1573
	private Announcement announcementUpdate;

	// Token: 0x0200012A RID: 298
	private enum AnnounceState
	{
		// Token: 0x04000627 RID: 1575
		Fetching,
		// Token: 0x04000628 RID: 1576
		Failed,
		// Token: 0x04000629 RID: 1577
		Success,
		// Token: 0x0400062A RID: 1578
		Cached
	}
}
