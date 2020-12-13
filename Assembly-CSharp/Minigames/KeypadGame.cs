using System.Collections;
using UnityEngine;

public class KeypadGame : Minigame
{
	public TextRenderer TargetText;

	public TextRenderer NumberText;

	public int number;

	public string numString = string.Empty;

	private bool animating;

	public SpriteRenderer AcceptButton;

	private LifeSuppSystemType system;

	private NoOxyTask oxyTask;

	private bool done;

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		oxyTask = (NoOxyTask)task;
		TargetText.Text = "today's code:\r\n" + oxyTask.targetNumber.ToString("D5");
		NumberText.Text = string.Empty;
		system = (LifeSuppSystemType)ShipStatus.Instance.Systems[SystemTypes.LifeSupp];
		done = system.GetConsoleComplete(base.ConsoleId);
	}

	public void ClickNumber(int i)
	{
		if (!animating && !done)
		{
			if (NumberText.Text.Length >= 5)
			{
				StartCoroutine(BlinkAccept());
				return;
			}
			numString += i;
			number = number * 10 + i;
			NumberText.Text = numString;
		}
	}

	private IEnumerator BlinkAccept()
	{
		int i = 0;
		while (i < 5)
		{
			AcceptButton.color = Color.gray;
			yield return null;
			yield return null;
			AcceptButton.color = Color.white;
			yield return null;
			yield return null;
			int num = i + 1;
			i = num;
		}
	}

	public void ClearEntry()
	{
		if (!animating)
		{
			number = 0;
			numString = string.Empty;
			NumberText.Text = string.Empty;
		}
	}

	public void Enter()
	{
		if (!animating)
		{
			StartCoroutine(Animate());
		}
	}

	private IEnumerator Animate()
	{
		animating = true;
		WaitForSeconds wait = new WaitForSeconds(0.1f);
		yield return wait;
		NumberText.Text = string.Empty;
		yield return wait;
		if (oxyTask.targetNumber == number)
		{
			done = true;
			byte amount = (byte)((uint)base.ConsoleId | 0x40u);
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, amount);
			try
			{
				((SabotageTask)MyTask).MarkContributed();
			}
			catch
			{
			}
			NumberText.Text = "OK";
			yield return wait;
			NumberText.Text = string.Empty;
			yield return wait;
			NumberText.Text = "OK";
			yield return wait;
			NumberText.Text = string.Empty;
			yield return wait;
			NumberText.Text = "OK";
		}
		else
		{
			NumberText.Text = "Bad";
			yield return wait;
			NumberText.Text = string.Empty;
			yield return wait;
			NumberText.Text = "Bad";
			yield return wait;
			numString = string.Empty;
			number = 0;
			NumberText.Text = numString;
		}
		animating = false;
	}
}
