using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatController : MonoBehaviour
{
	public ObjectPoolBehavior chatBubPool;

	public Transform TypingArea;

	public SpriteRenderer TextBubble;

	public TextBox TextArea;

	public TextRenderer CharCount;

	public int MaxChat = 15;

	public Scroller scroller;

	public GameObject Content;

	public SpriteRenderer ChatNotifyDot;

	public TextRenderer SendRateMessage;

	private List<ChatBubble> bubbles = new List<ChatBubble>();

	public Vector3 SourcePos = new Vector3(0f, 0f, -10f);

	public Vector3 TargetPos = new Vector3(-0.35f, 0.02f, -10f);

	private const float MaxChatSendRate = 3f;

	private float TimeSinceLastMessage = 3f;

	public AudioClip MessageSound;

	private bool animating;

	private Coroutine notificationRoutine;

    public BanMenu BanButton;

	private bool TypingImpOnly;

	public bool IsOpen => Content.activeInHierarchy;

	public void Toggle()
	{
		if (!animating)
		{
			if (IsOpen)
			{
				StartCoroutine(CoClose());
				return;
			}
			Content.SetActive(value: true);
			PlayerControl.LocalPlayer.NetTransform.Halt();
			StartCoroutine(CoOpen());
		}
	}

	public void SetVisible(bool visible)
	{
		Debug.Log("Chat is hidden: " + visible);
		DestroyableSingleton<HudManager>.Instance.Chat.ForceClosed();
		base.gameObject.SetActive(visible || GameOptionsData.Gamemodes[PlayerControl.GameOptions.Gamemode] == "Debug");
	}

	public void ForceClosed()
	{
		StopAllCoroutines();
		Content.SetActive(value: false);
		animating = false;
	}

	public IEnumerator CoOpen()
	{
		animating = true;
		Vector3 scale = Vector3.one;
		BanButton.Hide();
		BanButton.ShowButton(show: true);
		float timer = 0f;
		while (timer < 0.15f)
		{
			timer += Time.deltaTime;
			float num = Mathf.SmoothStep(0f, 1f, timer / 0.15f);
			scale.y = (scale.x = Mathf.Lerp(0.1f, 1f, num));
			Content.transform.localScale = scale;
			Content.transform.localPosition = Vector3.Lerp(SourcePos, TargetPos, num);
			BanButton.transform.localPosition = new Vector3(0f, (0f - num) * 0.75f, -20f);
			yield return null;
		}
		BanButton.SetButtonActive(show: true);
		ChatNotifyDot.enabled = false;
		animating = false;
		GiveFocus();
	}

	public IEnumerator CoClose()
	{
		animating = true;
		BanButton.Hide();
		BanButton.SetButtonActive(show: false);
		Vector3 scale = Vector3.one;
		for (float timer = 0f; timer < 0.15f; timer += Time.deltaTime)
		{
			float num = 1f - Mathf.SmoothStep(0f, 1f, timer / 0.15f);
			scale.y = (scale.x = Mathf.Lerp(0.1f, 1f, num));
			Content.transform.localScale = scale;
			Content.transform.localPosition = Vector3.Lerp(SourcePos, TargetPos, num);
			BanButton.transform.localPosition = new Vector3(0f, (0f - num) * 0.75f, -20f);
			yield return null;
		}
		BanButton.ShowButton(show: false);
		Content.SetActive(value: false);
		animating = false;
	}

	public void SetPosition(MeetingHud meeting)
	{
		if ((bool)meeting)
		{
			base.transform.SetParent(meeting.transform);
			base.transform.localPosition = new Vector3(3f, 2.2f, -10f);
		}
		else
		{
			base.transform.SetParent(DestroyableSingleton<HudManager>.Instance.transform);
			GetComponent<AspectPosition>().AdjustPosition();
		}
	}

	public void UpdateCharCount()
	{
		Vector2 size = TextBubble.size;
		size.y = Math.Max(0.62f, TextArea.TextHeight + 0.2f);
		TextBubble.size = size;
		Vector3 localPosition = TextBubble.transform.localPosition;
		localPosition.y = (0.62f - size.y) / 2f;
		TextBubble.transform.localPosition = localPosition;
		Vector3 localPosition2 = TypingArea.localPosition;
		localPosition2.y = -2.08f - localPosition.y * 2f;
		TypingArea.localPosition = localPosition2;
		int length = TextArea.text.Length;
		CharCount.Text = "Chars: " + length;
		CharCount.Color = Color.black;
	}

	private void Update()
	{
        TimeSinceLastMessage += Time.deltaTime;
		if (!PlayerControl.LocalPlayer.Data.IsImpostor)
        {
            TypingImpOnly = false;
			TextArea.GetTexRen().Color = Color.black;
		}
		else
        {
			if (PlayerControl.GameOptions.ImpOnlyChat)
            {
				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					TypingImpOnly = !TypingImpOnly;
					TextArea.GetTexRen().Color = TypingImpOnly ? Color.red : Color.black;
				}
			}
			else
            {
				TypingImpOnly = false;
				TextArea.GetTexRen().Color = Color.black;
			}
		}
		if (SendRateMessage.isActiveAndEnabled)
		{
			float num = 3f - TimeSinceLastMessage;
			if (num < 0f)
			{
				SendRateMessage.gameObject.SetActive(value: false);
				return;
			}
			SendRateMessage.Text = $"Too fast. Wait {Mathf.CeilToInt(num)} seconds";
		}
		UpdateCharCount();
	}

	public void SendChat()
	{
		float num = 3f - TimeSinceLastMessage;
		if (num > 0f)
		{
			SendRateMessage.gameObject.SetActive(value: true);
			SendRateMessage.Text = $"Too fast. Wait {Mathf.CeilToInt(num)} seconds";
		}
		else if (PlayerControl.LocalPlayer.RpcSendChat(TextArea.text,TypingImpOnly))
		{
			TimeSinceLastMessage = 0f;
			TextArea.Clear();
		}
	}

	public void AddChat(PlayerControl sourcePlayer, string chatText, bool impostoronly = false)
	{
		if (CE_LuaLoader.CurrentGMLua)
		{
            CE_LuaLoader.GetGamemodeResult("OnChat", chatText, new CE_PlayerInfoLua(sourcePlayer.Data),impostoronly);
		}
		if (!sourcePlayer)
		{
			return;
		}
		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		GameData.PlayerInfo data2 = sourcePlayer.Data;
		if (data2 == null || data == null || (data2.IsDead && !data.IsDead) || ((!data.IsImpostor) && impostoronly))
		{
			return;
		}
		if (bubbles.Count == MaxChat)
		{
			ChatBubble chatBubble = bubbles[0];
			bubbles.RemoveAt(0);
			chatBubble.OwnerPool.Reclaim(chatBubble);
		}
		ChatBubble chatBubble2 = chatBubPool.Get<ChatBubble>();
		try
		{
			chatBubble2.transform.SetParent(scroller.Inner);
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
			PlayerControl.SetPlayerMaterialColors(data2.ColorId, chatBubble2.ChatFace);
			chatBubble2.SetName(data2.PlayerName, data2.IsDead, ((data.IsImpostor && data2.IsImpostor) || (data.IsDead && data2.IsImpostor) && PlayerControl.GameOptions.GhostsSeeRoles), impostoronly,data2.role);
			chatBubble2.TextArea.Text = chatText;
			chatBubble2.TextArea.RefreshMesh();
			chatBubble2.Background.size = new Vector2(5.52f, 0.2f + chatBubble2.NameText.Height + chatBubble2.TextArea.Height);
			Vector3 localPosition = chatBubble2.Background.transform.localPosition;
			localPosition.y = chatBubble2.NameText.transform.localPosition.y - chatBubble2.Background.size.y / 2f + 0.05f;
			chatBubble2.Background.transform.localPosition = localPosition;
			bubbles.Add(chatBubble2);
			float num = 0f;
			for (int num2 = bubbles.Count - 1; num2 >= 0; num2--)
			{
				ChatBubble chatBubble3 = bubbles[num2];
				num += chatBubble3.Background.size.y;
				localPosition = chatBubble3.transform.localPosition;
				localPosition.y = -1.85f + num;
				chatBubble3.transform.localPosition = localPosition;
				num += 0.1f;
			}
			scroller.YBounds.min = Mathf.Min(0f, 0f - num + scroller.HitBox.bounds.size.y);
			if (!IsOpen && notificationRoutine == null)
			{
				notificationRoutine = DestroyableSingleton<HudManager>.Instance.StartCoroutine(BounceDot());
			}
			if (!flag)
			{
				SoundManager.Instance.PlaySound(MessageSound, loop: false).pitch = 0.5f + (float)(int)sourcePlayer.PlayerId / 10f;
			}
		}
		catch
		{
			chatBubPool.Reclaim(chatBubble2);
			bubbles.Remove(chatBubble2);
		}
	}

	private IEnumerator BounceDot()
	{
		ChatNotifyDot.enabled = true;
		yield return Effects.Bounce(ChatNotifyDot.transform);
		notificationRoutine = null;
	}

	public void GiveFocus()
	{
		TextArea.GiveFocus();
	}
}
