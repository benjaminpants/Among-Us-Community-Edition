using System;
using System.Collections;
using Hazel;
using UnityEngine;

// Token: 0x020000F1 RID: 241
public class PlayerVoteArea : MonoBehaviour
{
	// Token: 0x170000CB RID: 203
	// (get) Token: 0x0600051B RID: 1307 RVA: 0x000051EB File Offset: 0x000033EB
	// (set) Token: 0x0600051C RID: 1308 RVA: 0x000051F3 File Offset: 0x000033F3
	public MeetingHud Parent { get; set; }

	// Token: 0x0600051D RID: 1309 RVA: 0x00021F58 File Offset: 0x00020158
	public void SetDead(bool isMe, bool didReport, bool isDead)
	{
		this.isDead = isDead;
		this.didReport = didReport;
		this.Megaphone.enabled = didReport;
		this.Overlay.gameObject.SetActive(false);
		this.Overlay.transform.GetChild(0).gameObject.SetActive(isDead);
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x00021FAC File Offset: 0x000201AC
	public void SetDisabled()
	{
		if (this.isDead)
		{
			return;
		}
		if (this.Overlay)
		{
			this.Overlay.gameObject.SetActive(true);
			this.Overlay.transform.GetChild(0).gameObject.SetActive(false);
			return;
		}
		base.GetComponent<SpriteRenderer>().enabled = false;
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x000051FC File Offset: 0x000033FC
	public void SetEnabled()
	{
		if (this.isDead)
		{
			return;
		}
		if (this.Overlay)
		{
			this.Overlay.gameObject.SetActive(false);
			return;
		}
		base.GetComponent<SpriteRenderer>().enabled = true;
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x00005232 File Offset: 0x00003432
	public IEnumerator CoAnimateOverlay()
	{
		this.Overlay.gameObject.SetActive(this.isDead);
		if (this.isDead)
		{
			Transform xMark = this.Overlay.transform.GetChild(0);
			this.Overlay.color = Palette.ClearWhite;
			xMark.localScale = Vector3.zero;
			float fadeDuration = 0.5f;
			for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
			{
				this.Overlay.color = Color.Lerp(Palette.ClearWhite, Color.white, t / fadeDuration);
				yield return null;
			}
			this.Overlay.color = Color.white;
			float scaleDuration = 0.15f;
			for (float t = 0f; t < scaleDuration; t += Time.deltaTime)
			{
				float num = Mathf.Lerp(3f, 1f, t / scaleDuration);
				xMark.transform.localScale = new Vector3(num, num, num);
				yield return null;
			}
			xMark.transform.localScale = Vector3.one;
			xMark = null;
			xMark = null;
		}
		else if (this.didReport)
		{
			float scaleDuration = 1f;
			for (float fadeDuration = 0f; fadeDuration < scaleDuration; fadeDuration += Time.deltaTime)
			{
				float num2 = fadeDuration / scaleDuration;
				float num3 = PlayerVoteArea.TriangleWave(num2 * 3f) * 2f - 1f;
				this.Megaphone.transform.localEulerAngles = new Vector3(0f, 0f, num3 * 30f);
				num3 = Mathf.Lerp(0.7f, 1.2f, PlayerVoteArea.TriangleWave(num2 * 2f));
				this.Megaphone.transform.localScale = new Vector3(num3, num3, num3);
				yield return null;
			}
			this.Megaphone.transform.localEulerAngles = Vector3.zero;
			this.Megaphone.transform.localScale = Vector3.one;
		}
		yield break;
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x00005241 File Offset: 0x00003441
	private static float TriangleWave(float t)
	{
		t -= (float)((int)t);
		if (t < 0.5f)
		{
			return t * 2f;
		}
		return 1f - (t - 0.5f) * 2f;
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x0000526D File Offset: 0x0000346D
	internal void SetVote(sbyte suspectIdx)
	{
		this.didVote = true;
		this.votedFor = suspectIdx;
		this.Flag.enabled = true;
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x00005289 File Offset: 0x00003489
	public void UnsetVote()
	{
		this.Flag.enabled = false;
		this.votedFor = 0;
		this.didVote = false;
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x000052A5 File Offset: 0x000034A5
	public void ClearButtons()
	{
		this.Buttons.SetActive(false);
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x000052B3 File Offset: 0x000034B3
	public void ClearForResults()
	{
		this.resultsShowing = true;
		this.Flag.enabled = false;
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x000052C8 File Offset: 0x000034C8
	public void VoteForMe()
	{
		if (!this.voteComplete)
		{
			this.Parent.Confirm(this.TargetPlayerId);
		}
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x0002200C File Offset: 0x0002020C
	public void Select()
	{
		if (PlayerControl.LocalPlayer.Data.IsDead)
		{
			return;
		}
		if (this.isDead)
		{
			return;
		}
		if (!this.voteComplete && this.Parent.Select((int)this.TargetPlayerId))
		{
			this.Buttons.SetActive(true);
		}
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x000052A5 File Offset: 0x000034A5
	public void Cancel()
	{
		this.Buttons.SetActive(false);
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x0002205C File Offset: 0x0002025C
	public void Serialize(MessageWriter writer)
	{
		byte state = this.GetState();
		writer.Write(state);
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x00022078 File Offset: 0x00020278
	public void Deserialize(MessageReader reader)
	{
		byte b = reader.ReadByte();
		this.votedFor = (sbyte)((b & 15) - 1);
		this.isDead = ((b & 128) > 0);
		this.didVote = ((b & 64) > 0);
		this.didReport = ((b & 32) > 0);
		this.Flag.enabled = (this.didVote && !this.resultsShowing);
		this.Overlay.gameObject.SetActive(this.isDead);
		this.Megaphone.enabled = this.didReport;
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x000052E3 File Offset: 0x000034E3
	public byte GetState()
	{
		return (byte)((int)(this.votedFor + 1 & 15) | (this.isDead ? 128 : 0) | (this.didVote ? 64 : 0) | (this.didReport ? 32 : 0));
	}

	// Token: 0x040004E7 RID: 1255
	public sbyte TargetPlayerId;

	// Token: 0x040004E8 RID: 1256
	public const byte DeadBit = 128;

	// Token: 0x040004E9 RID: 1257
	public const byte VotedBit = 64;

	// Token: 0x040004EA RID: 1258
	public const byte ReportedBit = 32;

	// Token: 0x040004EB RID: 1259
	public const byte VoteMask = 31;

	// Token: 0x040004EC RID: 1260
	public GameObject Buttons;

	// Token: 0x040004ED RID: 1261
	public SpriteRenderer PlayerIcon;

	// Token: 0x040004EE RID: 1262
	public SpriteRenderer Flag;

	// Token: 0x040004EF RID: 1263
	public SpriteRenderer Megaphone;

	// Token: 0x040004F0 RID: 1264
	public SpriteRenderer Overlay;

	// Token: 0x040004F1 RID: 1265
	public TextRenderer NameText;

	// Token: 0x040004F2 RID: 1266
	public bool isDead;

	// Token: 0x040004F3 RID: 1267
	public bool didVote;

	// Token: 0x040004F4 RID: 1268
	public bool didReport;

	// Token: 0x040004F5 RID: 1269
	public sbyte votedFor;

	// Token: 0x040004F6 RID: 1270
	public bool voteComplete;

	// Token: 0x040004F7 RID: 1271
	public bool resultsShowing;
}
