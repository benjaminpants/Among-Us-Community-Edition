using System;
using System.Collections;
using UnityEngine;

public class EnterCodeMinigame : Minigame
{
	public TextRenderer NumberText;

	public TextRenderer TargetText;

	public int number;

	public string numString = string.Empty;

	private bool animating;

	public SpriteRenderer AcceptButton;

	private bool done;

	private int targetNumber;

	public void EnterDigit(int i)
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

	public void ClearDigits()
	{
		if (!animating)
		{
			number = 0;
			numString = string.Empty;
			NumberText.Text = string.Empty;
		}
	}

	public void AcceptDigits()
	{
		if (!animating)
		{
			StartCoroutine(Animate());
		}
	}

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		targetNumber = BitConverter.ToInt32(MyNormTask.Data, 0);
		NumberText.Text = string.Empty;
		TargetText.Text = targetNumber.ToString("D5");
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

	private IEnumerator Animate()
	{
		animating = true;
		WaitForSeconds wait = new WaitForSeconds(0.1f);
		yield return wait;
		NumberText.Text = string.Empty;
		yield return wait;
		if (targetNumber == number)
		{
			done = true;
			NumberText.Text = "OK";
			yield return wait;
			NumberText.Text = string.Empty;
			yield return wait;
			NumberText.Text = "OK";
			yield return wait;
			NumberText.Text = string.Empty;
			yield return CoStartClose(0.5f);
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
