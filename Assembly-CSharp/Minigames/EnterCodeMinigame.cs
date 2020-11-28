using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class EnterCodeMinigame : Minigame
{
	// Token: 0x06000011 RID: 17 RVA: 0x0000B18C File Offset: 0x0000938C
	public void EnterDigit(int i)
	{
		if (this.animating)
		{
			return;
		}
		if (this.done)
		{
			return;
		}
		if (this.NumberText.Text.Length >= 5)
		{
			base.StartCoroutine(this.BlinkAccept());
			return;
		}
		this.numString += i;
		this.number = this.number * 10 + i;
		this.NumberText.Text = this.numString;
	}

	// Token: 0x06000012 RID: 18 RVA: 0x0000228D File Offset: 0x0000048D
	public void ClearDigits()
	{
		if (this.animating)
		{
			return;
		}
		this.number = 0;
		this.numString = string.Empty;
		this.NumberText.Text = string.Empty;
	}

	// Token: 0x06000013 RID: 19 RVA: 0x000022BA File Offset: 0x000004BA
	public void AcceptDigits()
	{
		if (this.animating)
		{
			return;
		}
		base.StartCoroutine(this.Animate());
	}

	// Token: 0x06000014 RID: 20 RVA: 0x0000B208 File Offset: 0x00009408
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		this.targetNumber = BitConverter.ToInt32(this.MyNormTask.Data, 0);
		this.NumberText.Text = string.Empty;
		this.TargetText.Text = this.targetNumber.ToString("D5");
	}

	// Token: 0x06000015 RID: 21 RVA: 0x000022D2 File Offset: 0x000004D2
	private IEnumerator BlinkAccept()
	{
		int num;
		for (int i = 0; i < 5; i = num)
		{
			this.AcceptButton.color = Color.gray;
			yield return null;
			yield return null;
			this.AcceptButton.color = Color.white;
			yield return null;
			yield return null;
			num = i + 1;
		}
		yield break;
	}

	// Token: 0x06000016 RID: 22 RVA: 0x000022E1 File Offset: 0x000004E1
	private IEnumerator Animate()
	{
		this.animating = true;
		WaitForSeconds wait = new WaitForSeconds(0.1f);
		yield return wait;
		this.NumberText.Text = string.Empty;
		yield return wait;
		if (this.targetNumber == this.number)
		{
			this.done = true;
			this.NumberText.Text = "OK";
			yield return wait;
			this.NumberText.Text = string.Empty;
			yield return wait;
			this.NumberText.Text = "OK";
			yield return wait;
			this.NumberText.Text = string.Empty;
			yield return base.CoStartClose(0.5f);
		}
		else
		{
			this.NumberText.Text = "Bad";
			yield return wait;
			this.NumberText.Text = string.Empty;
			yield return wait;
			this.NumberText.Text = "Bad";
			yield return wait;
			this.numString = string.Empty;
			this.number = 0;
			this.NumberText.Text = this.numString;
		}
		this.animating = false;
		yield break;
	}

	// Token: 0x04000021 RID: 33
	public TextRenderer NumberText;

	// Token: 0x04000022 RID: 34
	public TextRenderer TargetText;

	// Token: 0x04000023 RID: 35
	public int number;

	// Token: 0x04000024 RID: 36
	public string numString = string.Empty;

	// Token: 0x04000025 RID: 37
	private bool animating;

	// Token: 0x04000026 RID: 38
	public SpriteRenderer AcceptButton;

	// Token: 0x04000027 RID: 39
	private bool done;

	// Token: 0x04000028 RID: 40
	private int targetNumber;
}
