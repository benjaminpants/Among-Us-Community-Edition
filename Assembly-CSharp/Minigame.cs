using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001DB RID: 475
public abstract class Minigame : MonoBehaviour
{
	// Token: 0x1700018D RID: 397
	// (get) Token: 0x06000A3F RID: 2623 RVA: 0x0000837E File Offset: 0x0000657E
	// (set) Token: 0x06000A40 RID: 2624 RVA: 0x00008386 File Offset: 0x00006586
	public global::Console Console { get; set; }

	// Token: 0x1700018E RID: 398
	// (get) Token: 0x06000A41 RID: 2625 RVA: 0x0000838F File Offset: 0x0000658F
	protected int ConsoleId
	{
		get
		{
			if (!this.Console)
			{
				return 0;
			}
			return this.Console.ConsoleId;
		}
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x000350CC File Offset: 0x000332CC
	public virtual void Begin(PlayerTask task)
	{
		Minigame.Instance = this;
		this.MyTask = task;
		this.MyNormTask = (task as NormalPlayerTask);
		if (PlayerControl.LocalPlayer)
		{
			if (MapBehaviour.Instance)
			{
				MapBehaviour.Instance.Close();
			}
			PlayerControl.LocalPlayer.NetTransform.Halt();
		}
		base.StartCoroutine(this.CoAnimateOpen());
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x000083AB File Offset: 0x000065AB
	protected IEnumerator CoStartClose(float duration = 0.75f)
	{
		if (this.amClosing != Minigame.CloseState.None)
		{
			yield break;
		}
		this.amClosing = Minigame.CloseState.Waiting;
		yield return Effects.Wait(duration);
		this.Close();
		yield break;
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x000083C1 File Offset: 0x000065C1
	[Obsolete("Don't use, I just don't want to reselect the close button event handlers", true)]
	public void Close(bool allowMovement)
	{
		this.Close();
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x00035130 File Offset: 0x00033330
	public virtual void Close()
	{
		if (this.amClosing != Minigame.CloseState.Closing)
		{
			if (this.CloseSound && Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(this.CloseSound, false, 1f);
			}
			this.amClosing = Minigame.CloseState.Closing;
			base.StartCoroutine(this.CoDestroySelf());
			return;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x000083C9 File Offset: 0x000065C9
	protected virtual IEnumerator CoAnimateOpen()
	{
		if (this.OpenSound && Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(this.OpenSound, false, 1f);
		}
		TransitionType transType = this.TransType;
		if (transType != TransitionType.SlideBottom)
		{
			if (transType == TransitionType.Alpha)
			{
				SpriteRenderer[] rends = base.GetComponentsInChildren<SpriteRenderer>();
				for (float timer = 0f; timer < 0.25f; timer += Time.deltaTime)
				{
					float t = timer / 0.25f;
					for (int i = 0; i < rends.Length; i++)
					{
						rends[i].color = Color.Lerp(Palette.ClearWhite, Color.white, t);
					}
					yield return null;
				}
				for (int j = 0; j < rends.Length; j++)
				{
					rends[j].color = Color.white;
				}
				rends = null;
			}
		}
		else
		{
			for (float timer = 0f; timer < 0.25f; timer += Time.deltaTime)
			{
				float t2 = timer / 0.25f;
				base.transform.localPosition = new Vector3(0f, Mathf.SmoothStep(-8f, 0f, t2), -50f);
				yield return null;
			}
			base.transform.localPosition = new Vector3(0f, 0f, -50f);
		}
		yield break;
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x000083D8 File Offset: 0x000065D8
	protected virtual IEnumerator CoDestroySelf()
	{
		TransitionType transType = this.TransType;
		if (transType != TransitionType.SlideBottom)
		{
			if (transType == TransitionType.Alpha)
			{
				SpriteRenderer[] rends = base.GetComponentsInChildren<SpriteRenderer>();
				for (float timer = 0f; timer < 0.25f; timer += Time.deltaTime)
				{
					float t = timer / 0.25f;
					for (int i = 0; i < rends.Length; i++)
					{
						rends[i].color = Color.Lerp(Color.white, Palette.ClearWhite, t);
					}
					yield return null;
				}
				rends = null;
			}
		}
		else
		{
			for (float timer = 0f; timer < 0.25f; timer += Time.deltaTime)
			{
				float t2 = timer / 0.25f;
				base.transform.localPosition = new Vector3(0f, Mathf.SmoothStep(0f, -8f, t2), -50f);
				yield return null;
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x040009D4 RID: 2516
	public static Minigame Instance;

	// Token: 0x040009D5 RID: 2517
	public const float Depth = -50f;

	// Token: 0x040009D6 RID: 2518
	public TransitionType TransType;

	// Token: 0x040009D7 RID: 2519
	protected PlayerTask MyTask;

	// Token: 0x040009D8 RID: 2520
	protected NormalPlayerTask MyNormTask;

	// Token: 0x040009DA RID: 2522
	protected Minigame.CloseState amClosing;

	// Token: 0x040009DB RID: 2523
	public AudioClip OpenSound;

	// Token: 0x040009DC RID: 2524
	public AudioClip CloseSound;

	// Token: 0x020001DC RID: 476
	protected enum CloseState
	{
		// Token: 0x040009DE RID: 2526
		None,
		// Token: 0x040009DF RID: 2527
		Waiting,
		// Token: 0x040009E0 RID: 2528
		Closing
	}
}
