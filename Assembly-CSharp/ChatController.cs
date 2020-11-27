using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000BC RID: 188
public class ChatController : MonoBehaviour
{
	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x060003EC RID: 1004 RVA: 0x000048E4 File Offset: 0x00002AE4
	public bool IsOpen
	{
		get
		{
			return this.Content.activeInHierarchy;
		}
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x00019390 File Offset: 0x00017590
	public void Toggle()
	{
		if (this.animating)
		{
			return;
		}
		if (this.IsOpen)
		{
			base.StartCoroutine(this.CoClose());
			return;
		}
		this.Content.SetActive(true);
		PlayerControl.LocalPlayer.NetTransform.Halt();
		base.StartCoroutine(this.CoOpen());
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x000048F1 File Offset: 0x00002AF1
	public void SetVisible(bool visible)
	{
		Debug.Log("Chat is hidden: " + visible.ToString());
		DestroyableSingleton<HudManager>.Instance.Chat.ForceClosed();
		base.gameObject.SetActive(visible);
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x00004924 File Offset: 0x00002B24
	public void ForceClosed()
	{
		base.StopAllCoroutines();
		this.Content.SetActive(false);
		this.animating = false;
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x0000493F File Offset: 0x00002B3F
	public IEnumerator CoOpen()
	{
		this.animating = true;
		Vector3 scale = Vector3.one;
		this.BanButton.Hide();
		this.BanButton.ShowButton(true);
		float timer = 0f;
		while (timer < 0.15f)
		{
			timer += Time.deltaTime;
			float num = Mathf.SmoothStep(0f, 1f, timer / 0.15f);
			scale.y = (scale.x = Mathf.Lerp(0.1f, 1f, num));
			this.Content.transform.localScale = scale;
			this.Content.transform.localPosition = Vector3.Lerp(this.SourcePos, this.TargetPos, num);
			this.BanButton.transform.localPosition = new Vector3(0f, -num * 0.75f, -20f);
			yield return null;
		}
		this.BanButton.SetButtonActive(true);
		this.ChatNotifyDot.enabled = false;
		this.animating = false;
		this.GiveFocus();
		yield break;
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x0000494E File Offset: 0x00002B4E
	public IEnumerator CoClose()
	{
		this.animating = true;
		this.BanButton.Hide();
		this.BanButton.SetButtonActive(false);
		Vector3 scale = Vector3.one;
		for (float timer = 0f; timer < 0.15f; timer += Time.deltaTime)
		{
			float num = 1f - Mathf.SmoothStep(0f, 1f, timer / 0.15f);
			scale.y = (scale.x = Mathf.Lerp(0.1f, 1f, num));
			this.Content.transform.localScale = scale;
			this.Content.transform.localPosition = Vector3.Lerp(this.SourcePos, this.TargetPos, num);
			this.BanButton.transform.localPosition = new Vector3(0f, -num * 0.75f, -20f);
			yield return null;
		}
		this.BanButton.ShowButton(false);
		this.Content.SetActive(false);
		this.animating = false;
		yield break;
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x000193E4 File Offset: 0x000175E4
	public void SetPosition(MeetingHud meeting)
	{
		if (meeting)
		{
			base.transform.SetParent(meeting.transform);
			base.transform.localPosition = new Vector3(3f, 2.2f, -10f);
			return;
		}
		base.transform.SetParent(DestroyableSingleton<HudManager>.Instance.transform);
		base.GetComponent<AspectPosition>().AdjustPosition();
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x0001944C File Offset: 0x0001764C
	public void UpdateCharCount()
	{
		Vector2 size = this.TextBubble.size;
		size.y = Math.Max(0.62f, this.TextArea.TextHeight + 0.2f);
		this.TextBubble.size = size;
		Vector3 localPosition = this.TextBubble.transform.localPosition;
		localPosition.y = (0.62f - size.y) / 2f;
		this.TextBubble.transform.localPosition = localPosition;
		Vector3 localPosition2 = this.TypingArea.localPosition;
		localPosition2.y = -2.08f - localPosition.y * 2f;
		this.TypingArea.localPosition = localPosition2;
		int length = this.TextArea.text.Length;
		this.CharCount.Text = length + "/100";
		if (length < 75)
		{
			this.CharCount.Color = Color.black;
			return;
		}
		if (length < 100)
		{
			this.CharCount.Color = new Color(1f, 1f, 0f, 1f);
			return;
		}
		this.CharCount.Color = Color.red;
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x0001957C File Offset: 0x0001777C
	private void Update()
	{
		this.TimeSinceLastMessage += Time.deltaTime;
		if (this.SendRateMessage.isActiveAndEnabled)
		{
			float num = 3f - this.TimeSinceLastMessage;
			if (num < 0f)
			{
				this.SendRateMessage.gameObject.SetActive(false);
				return;
			}
			this.SendRateMessage.Text = string.Format("Too fast. Wait {0} seconds", Mathf.CeilToInt(num));
		}
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x000195F0 File Offset: 0x000177F0
	public void SendChat()
	{
		float num = 3f - this.TimeSinceLastMessage;
		if (num > 0f)
		{
			this.SendRateMessage.gameObject.SetActive(true);
			this.SendRateMessage.Text = string.Format("Too fast. Wait {0} seconds", Mathf.CeilToInt(num));
			return;
		}
		if (!PlayerControl.LocalPlayer.RpcSendChat(this.TextArea.text))
		{
			return;
		}
		this.TimeSinceLastMessage = 0f;
		this.TextArea.Clear();
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x00019674 File Offset: 0x00017874
	public void AddChat(PlayerControl sourcePlayer, string chatText)
	{
		if (!sourcePlayer)
		{
			return;
		}
		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		GameData.PlayerInfo data2 = sourcePlayer.Data;
		if (data2 == null || data == null || (data2.IsDead && !data.IsDead))
		{
			return;
		}
		if (this.bubbles.Count == this.MaxChat)
		{
			ChatBubble chatBubble = this.bubbles[0];
			this.bubbles.RemoveAt(0);
			chatBubble.OwnerPool.Reclaim(chatBubble);
		}
		ChatBubble chatBubble2 = this.chatBubPool.Get<ChatBubble>();
		try
		{
			chatBubble2.transform.SetParent(this.scroller.Inner);
			chatBubble2.transform.localScale = Vector3.one;
			bool flag = sourcePlayer == PlayerControl.LocalPlayer;
			if (flag)
			{
				chatBubble2.SetRight();
			}
			else
			{
				chatBubble2.SetLeft();
			}
			PlayerControl.SetPlayerMaterialColors((int)data2.ColorId, chatBubble2.ChatFace);
			chatBubble2.SetName(data2.PlayerName, data2.IsDead, data.IsImpostor && data2.IsImpostor);
			chatBubble2.TextArea.Text = chatText;
			chatBubble2.TextArea.RefreshMesh();
			chatBubble2.Background.size = new Vector2(5.52f, 0.2f + chatBubble2.NameText.Height + chatBubble2.TextArea.Height);
			Vector3 localPosition = chatBubble2.Background.transform.localPosition;
			localPosition.y = chatBubble2.NameText.transform.localPosition.y - chatBubble2.Background.size.y / 2f + 0.05f;
			chatBubble2.Background.transform.localPosition = localPosition;
			this.bubbles.Add(chatBubble2);
			float num = 0f;
			for (int i = this.bubbles.Count - 1; i >= 0; i--)
			{
				ChatBubble chatBubble3 = this.bubbles[i];
				num += chatBubble3.Background.size.y;
				localPosition = chatBubble3.transform.localPosition;
				localPosition.y = -1.85f + num;
				chatBubble3.transform.localPosition = localPosition;
				num += 0.1f;
			}
			this.scroller.YBounds.min = Mathf.Min(0f, -num + this.scroller.HitBox.bounds.size.y);
			if (!this.IsOpen && this.notificationRoutine == null)
			{
				this.notificationRoutine = DestroyableSingleton<HudManager>.Instance.StartCoroutine(this.BounceDot());
			}
			if (!flag)
			{
				SoundManager.Instance.PlaySound(this.MessageSound, false, 1f).pitch = 0.5f + (float)sourcePlayer.PlayerId / 10f;
			}
		}
		catch
		{
			this.chatBubPool.Reclaim(chatBubble2);
			this.bubbles.Remove(chatBubble2);
		}
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x0000495D File Offset: 0x00002B5D
	private IEnumerator BounceDot()
	{
		this.ChatNotifyDot.enabled = true;
		yield return Effects.Bounce(this.ChatNotifyDot.transform, 0.3f, 0.15f);
		this.notificationRoutine = null;
		yield break;
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x0000496C File Offset: 0x00002B6C
	public void GiveFocus()
	{
		this.TextArea.GiveFocus();
	}

	// Token: 0x040003DF RID: 991
	public ObjectPoolBehavior chatBubPool;

	// Token: 0x040003E0 RID: 992
	public Transform TypingArea;

	// Token: 0x040003E1 RID: 993
	public SpriteRenderer TextBubble;

	// Token: 0x040003E2 RID: 994
	public TextBox TextArea;

	// Token: 0x040003E3 RID: 995
	public TextRenderer CharCount;

	// Token: 0x040003E4 RID: 996
	public int MaxChat = 15;

	// Token: 0x040003E5 RID: 997
	public Scroller scroller;

	// Token: 0x040003E6 RID: 998
	public GameObject Content;

	// Token: 0x040003E7 RID: 999
	public SpriteRenderer ChatNotifyDot;

	// Token: 0x040003E8 RID: 1000
	public TextRenderer SendRateMessage;

	// Token: 0x040003E9 RID: 1001
	private List<ChatBubble> bubbles = new List<ChatBubble>();

	// Token: 0x040003EA RID: 1002
	public Vector3 SourcePos = new Vector3(0f, 0f, -10f);

	// Token: 0x040003EB RID: 1003
	public Vector3 TargetPos = new Vector3(-0.35f, 0.02f, -10f);

	// Token: 0x040003EC RID: 1004
	private const float MaxChatSendRate = 3f;

	// Token: 0x040003ED RID: 1005
	private float TimeSinceLastMessage = 3f;

	// Token: 0x040003EE RID: 1006
	public AudioClip MessageSound;

	// Token: 0x040003EF RID: 1007
	private bool animating;

	// Token: 0x040003F0 RID: 1008
	private Coroutine notificationRoutine;

	// Token: 0x040003F1 RID: 1009
	public BanMenu BanButton;
}
