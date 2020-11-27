using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D7 RID: 215
public class KillOverlay : MonoBehaviour
{
	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x06000483 RID: 1155 RVA: 0x00004D40 File Offset: 0x00002F40
	public bool IsOpen
	{
		get
		{
			return this.showAll != null || this.queue.Count > 0;
		}
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x00004D5A File Offset: 0x00002F5A
	public IEnumerator WaitForFinish()
	{
		while (this.showAll != null || this.queue.Count > 0)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x0001AF84 File Offset: 0x00019184
	public void ShowOne(PlayerControl killer, GameData.PlayerInfo victim)
	{
		this.queue.Enqueue(() => this.CoShowOne(this.KillAnims.Random<OverlayKillAnimation>(), killer, victim));
		if (this.showAll == null)
		{
			this.showAll = base.StartCoroutine(this.ShowAll());
		}
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x0001AFE0 File Offset: 0x000191E0
	public void ShowOne(OverlayKillAnimation killAnimPrefab, PlayerControl killer, GameData.PlayerInfo victim)
	{
		this.queue.Enqueue(() => this.CoShowOne(killAnimPrefab, killer, victim));
		if (this.showAll == null)
		{
			this.showAll = base.StartCoroutine(this.ShowAll());
		}
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x00004D69 File Offset: 0x00002F69
	private IEnumerator ShowAll()
	{
		while (this.queue.Count > 0 || this.showOne != null)
		{
			if (this.showOne == null)
			{
				this.showOne = base.StartCoroutine(this.queue.Dequeue()());
			}
			yield return null;
		}
		this.showAll = null;
		yield break;
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x00004D78 File Offset: 0x00002F78
	private IEnumerator CoShowOne(OverlayKillAnimation killAnimPrefab, PlayerControl killer, GameData.PlayerInfo victim)
	{
		OverlayKillAnimation overlayKillAnimation = UnityEngine.Object.Instantiate<OverlayKillAnimation>(killAnimPrefab, base.transform);
		overlayKillAnimation.Begin(killer, victim);
		overlayKillAnimation.gameObject.SetActive(false);
		yield return this.CoShowOne(overlayKillAnimation);
		yield break;
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x00004D9C File Offset: 0x00002F9C
	private IEnumerator CoShowOne(OverlayKillAnimation anim)
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(anim.Stinger, false, 1f).volume = anim.StingerVolume;
		}
		WaitForSeconds wait = new WaitForSeconds(0.0833333358f);
		this.background.enabled = true;
		yield return wait;
		this.background.enabled = false;
		this.flameParent.SetActive(true);
		this.flameParent.transform.localScale = new Vector3(1f, 0.3f, 1f);
		this.flameParent.transform.localEulerAngles = new Vector3(0f, 0f, 25f);
		yield return wait;
		this.flameParent.transform.localScale = new Vector3(1f, 0.5f, 1f);
		this.flameParent.transform.localEulerAngles = new Vector3(0f, 0f, -15f);
		yield return wait;
		this.flameParent.transform.localScale = new Vector3(1f, 1f, 1f);
		this.flameParent.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		anim.gameObject.SetActive(true);
		yield return anim.WaitForFinish();
		UnityEngine.Object.Destroy(anim.gameObject);
		yield return new WaitForLerp(0.166666672f, delegate(float t)
		{
			this.flameParent.transform.localScale = new Vector3(1f, 1f - t, 1f);
		});
		this.flameParent.SetActive(false);
		this.showOne = null;
		yield break;
	}

	// Token: 0x0400046A RID: 1130
	public SpriteRenderer background;

	// Token: 0x0400046B RID: 1131
	public GameObject flameParent;

	// Token: 0x0400046C RID: 1132
	public OverlayKillAnimation[] KillAnims;

	// Token: 0x0400046D RID: 1133
	public float FadeTime = 0.6f;

	// Token: 0x0400046E RID: 1134
	public OverlayKillAnimation EmergencyOverlay;

	// Token: 0x0400046F RID: 1135
	public OverlayKillAnimation ReportOverlay;

	// Token: 0x04000470 RID: 1136
	private Queue<Func<IEnumerator>> queue = new Queue<Func<IEnumerator>>();

	// Token: 0x04000471 RID: 1137
	private Coroutine showAll;

	// Token: 0x04000472 RID: 1138
	private Coroutine showOne;
}
