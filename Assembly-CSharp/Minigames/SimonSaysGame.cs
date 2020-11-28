using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A6 RID: 422
public class SimonSaysGame : Minigame
{
	// Token: 0x17000152 RID: 338
	// (get) Token: 0x060008EC RID: 2284 RVA: 0x0000696F File Offset: 0x00004B6F
	// (set) Token: 0x060008ED RID: 2285 RVA: 0x00007722 File Offset: 0x00005922
	private int IndexCount
	{
		get
		{
			return (int)this.MyNormTask.Data[0];
		}
		set
		{
			this.MyNormTask.Data[0] = (byte)value;
		}
	}

	// Token: 0x17000153 RID: 339
	private byte this[int idx]
	{
		get
		{
			return this.MyNormTask.Data[idx + 1];
		}
		set
		{
			this.MyNormTask.Data[idx + 1] = value;
		}
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x00030078 File Offset: 0x0002E278
	public override void Begin(PlayerTask task)
	{
		for (int i = 0; i < this.LeftSide.Length; i++)
		{
			this.LeftSide[i].color = Color.clear;
		}
		base.Begin(task);
		if (this.IndexCount == 0)
		{
			this.operations.Enqueue(128);
		}
		else
		{
			this.operations.Enqueue(32);
		}
		base.StartCoroutine(this.CoRun());
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x000300E8 File Offset: 0x0002E2E8
	public void HitButton(int bIdx)
	{
		if (this.MyNormTask.IsComplete || this.MyNormTask.taskStep >= this.IndexCount)
		{
			return;
		}
		if ((int)this[this.MyNormTask.taskStep] == bIdx)
		{
			this.MyNormTask.NextStep();
			this.SetLights(this.RightLights, this.MyNormTask.taskStep);
			if (this.MyNormTask.IsComplete)
			{
				this.SetLights(this.LeftLights, this.LeftLights.Length);
				for (int i = 0; i < this.Buttons.Length; i++)
				{
					SpriteRenderer spriteRenderer = this.Buttons[i];
					spriteRenderer.color = this.gray;
					base.StartCoroutine(this.FlashButton(-1, spriteRenderer, this.flashTime));
				}
				base.StartCoroutine(base.CoStartClose(0.75f));
				return;
			}
			this.operations.Enqueue(256 | bIdx);
			if (this.MyNormTask.taskStep >= this.IndexCount)
			{
				this.operations.Enqueue(128);
				return;
			}
		}
		else
		{
			this.IndexCount = 0;
			this.operations.Enqueue(64);
			this.operations.Enqueue(128);
		}
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x00007756 File Offset: 0x00005956
	private IEnumerator CoRun()
	{
		for (;;)
		{
			if (this.operations.Count <= 0)
			{
				yield return null;
			}
			else
			{
				int num = this.operations.Dequeue();
				if (num.HasAnyBit(256))
				{
					int num2 = num & -257;
					yield return this.FlashButton(num2, this.Buttons[num2], this.userButtonFlashTime);
				}
				else if (num.HasAnyBit(128))
				{
					yield return this.CoAnimateNewLeftSide();
				}
				else if (num.HasAnyBit(32))
				{
					yield return this.CoAnimateOldLeftSide();
				}
				else if (num.HasAnyBit(64))
				{
					if (Constants.ShouldPlaySfx())
					{
						SoundManager.Instance.PlaySound(this.FailSound, false, 1f);
					}
					this.SetAllColor(this.red);
					yield return new WaitForSeconds(this.flashTime);
					this.SetAllColor(Color.white);
					yield return new WaitForSeconds(this.flashTime);
					this.SetAllColor(this.red);
					yield return new WaitForSeconds(this.flashTime);
					this.SetAllColor(Color.white);
					yield return new WaitForSeconds(this.flashTime / 2f);
				}
			}
		}
		yield break;
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x0003021C File Offset: 0x0002E41C
	private void AddIndex(int idxToAdd)
	{
		this[this.IndexCount] = (byte)idxToAdd;
		int indexCount = this.IndexCount;
		this.IndexCount = indexCount + 1;
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00007765 File Offset: 0x00005965
	private IEnumerator CoAnimateNewLeftSide()
	{
		this.SetLights(this.RightLights, 0);
		for (int i = 0; i < this.Buttons.Length; i++)
		{
			this.Buttons[i].color = this.gray;
		}
		this.AddIndex(this.Buttons.RandomIdx<SpriteRenderer>());
		yield return this.CoAnimateOldLeftSide();
		yield break;
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x00007774 File Offset: 0x00005974
	private IEnumerator CoAnimateOldLeftSide()
	{
		yield return new WaitForSeconds(1f);
		this.SetLights(this.LeftLights, this.IndexCount);
		int num2;
		for (int i = 0; i < this.IndexCount; i = num2)
		{
			int num = (int)this[i];
			yield return this.FlashButton(num, this.LeftSide[num], this.flashTime);
			yield return new WaitForSeconds(0.1f);
			num2 = i + 1;
		}
		this.MyNormTask.taskStep = 0;
		for (int j = 0; j < this.Buttons.Length; j++)
		{
			this.Buttons[j].color = Color.white;
		}
		yield break;
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x00007783 File Offset: 0x00005983
	private IEnumerator FlashButton(int id, SpriteRenderer butt, float flashTime)
	{
		if (id > -1 && Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(this.ButtonPressSound, false, 1f).pitch = Mathf.Lerp(0.5f, 1.5f, (float)id / 9f);
		}
		Color c = butt.color;
		butt.color = this.blue;
		yield return new WaitForSeconds(flashTime);
		butt.color = c;
		yield break;
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x00030248 File Offset: 0x0002E448
	private void SetLights(SpriteRenderer[] lights, int num)
	{
		for (int i = 0; i < lights.Length; i++)
		{
			if (i < num)
			{
				lights[i].color = this.green;
			}
			else
			{
				lights[i].color = this.gray;
			}
		}
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x00030288 File Offset: 0x0002E488
	private void SetAllColor(Color color)
	{
		for (int i = 0; i < this.Buttons.Length; i++)
		{
			this.Buttons[i].color = color;
		}
		for (int j = 0; j < this.RightLights.Length; j++)
		{
			this.RightLights[j].color = color;
		}
	}

	// Token: 0x040008AB RID: 2219
	private Queue<int> operations = new Queue<int>();

	// Token: 0x040008AC RID: 2220
	private const int FlashOp = 256;

	// Token: 0x040008AD RID: 2221
	private const int AnimateOp = 128;

	// Token: 0x040008AE RID: 2222
	private const int ReAnimateOp = 32;

	// Token: 0x040008AF RID: 2223
	private const int FailOp = 64;

	// Token: 0x040008B0 RID: 2224
	private Color gray = new Color32(141, 141, 141, byte.MaxValue);

	// Token: 0x040008B1 RID: 2225
	private Color blue = new Color32(68, 168, byte.MaxValue, byte.MaxValue);

	// Token: 0x040008B2 RID: 2226
	private Color red = new Color32(byte.MaxValue, 58, 0, byte.MaxValue);

	// Token: 0x040008B3 RID: 2227
	private Color green = Color.green;

	// Token: 0x040008B4 RID: 2228
	public SpriteRenderer[] LeftSide;

	// Token: 0x040008B5 RID: 2229
	public SpriteRenderer[] Buttons;

	// Token: 0x040008B6 RID: 2230
	public SpriteRenderer[] LeftLights;

	// Token: 0x040008B7 RID: 2231
	public SpriteRenderer[] RightLights;

	// Token: 0x040008B8 RID: 2232
	private float flashTime = 0.25f;

	// Token: 0x040008B9 RID: 2233
	private float userButtonFlashTime = 0.175f;

	// Token: 0x040008BA RID: 2234
	public AudioClip ButtonPressSound;

	// Token: 0x040008BB RID: 2235
	public AudioClip FailSound;
}
