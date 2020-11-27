using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000121 RID: 289
public class KeypadGame : Minigame
{
	// Token: 0x0600060C RID: 1548 RVA: 0x0002570C File Offset: 0x0002390C
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		this.oxyTask = (NoOxyTask)task;
		this.TargetText.Text = "today's code:\r\n" + this.oxyTask.targetNumber.ToString("D5");
		this.NumberText.Text = string.Empty;
		this.system = (LifeSuppSystemType)ShipStatus.Instance.Systems[SystemTypes.LifeSupp];
		this.done = this.system.GetConsoleComplete(base.ConsoleId);
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x00025798 File Offset: 0x00023998
	public void ClickNumber(int i)
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

	// Token: 0x0600060E RID: 1550 RVA: 0x00005CED File Offset: 0x00003EED
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

	// Token: 0x0600060F RID: 1551 RVA: 0x00005CFC File Offset: 0x00003EFC
	public void ClearEntry()
	{
		if (this.animating)
		{
			return;
		}
		this.number = 0;
		this.numString = string.Empty;
		this.NumberText.Text = string.Empty;
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x00005D29 File Offset: 0x00003F29
	public void Enter()
	{
		if (this.animating)
		{
			return;
		}
		base.StartCoroutine(this.Animate());
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x00005D41 File Offset: 0x00003F41
	private IEnumerator Animate()
	{
		this.animating = true;
		WaitForSeconds wait = new WaitForSeconds(0.1f);
		yield return wait;
		this.NumberText.Text = string.Empty;
		yield return wait;
		if (this.oxyTask.targetNumber == this.number)
		{
			this.done = true;
			byte amount = (byte)(base.ConsoleId | 64);
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, (int)amount);
			try
			{
				((SabotageTask)this.MyTask).MarkContributed();
			}
			catch
			{
			}
			this.NumberText.Text = "OK";
			yield return wait;
			this.NumberText.Text = string.Empty;
			yield return wait;
			this.NumberText.Text = "OK";
			yield return wait;
			this.NumberText.Text = string.Empty;
			yield return wait;
			this.NumberText.Text = "OK";
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

	// Token: 0x040005F7 RID: 1527
	public TextRenderer TargetText;

	// Token: 0x040005F8 RID: 1528
	public TextRenderer NumberText;

	// Token: 0x040005F9 RID: 1529
	public int number;

	// Token: 0x040005FA RID: 1530
	public string numString = string.Empty;

	// Token: 0x040005FB RID: 1531
	private bool animating;

	// Token: 0x040005FC RID: 1532
	public SpriteRenderer AcceptButton;

	// Token: 0x040005FD RID: 1533
	private LifeSuppSystemType system;

	// Token: 0x040005FE RID: 1534
	private NoOxyTask oxyTask;

	// Token: 0x040005FF RID: 1535
	private bool done;
}
