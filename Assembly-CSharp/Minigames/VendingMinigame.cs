using System.Collections;
using System.Linq;
using UnityEngine;

public class VendingMinigame : Minigame
{
	public static readonly string[] Letters = new string[3]
	{
		"a",
		"b",
		"c"
	};

	public TextRenderer NumberText;

	public SpriteRenderer TargetImage;

	public string enteredCode = string.Empty;

	private bool animating;

	private bool done;

	private string targetCode;

	public SpriteRenderer AcceptButton;

	public VendingSlot[] Slots;

	public Sprite[] Drinks;

	public Sprite[] DrawnDrinks;

	public void OnEnable()
	{
		Begin(null);
	}

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		int num = Drinks.RandomIdx();
		TargetImage.sprite = DrawnDrinks[num];
		for (int i = 0; i < Drinks.Length; i++)
		{
			Sprite sprite = Drinks[i];
			int slotId;
			while (!PickARandomSlot(sprite, out slotId))
			{
			}
			Slots[slotId].DrinkImage.enabled = true;
			Slots[slotId].DrinkImage.sprite = sprite;
			if (num == i)
			{
				targetCode = SlotIdToString(slotId);
			}
		}
		NumberText.Text = string.Empty;
	}

	private static int StringToSlotId(string code)
	{
		if (int.TryParse(code[0].ToString(), out var _) || Letters.Any(code.EndsWith))
		{
			return -1;
		}
		int num = Letters.IndexOf(code.StartsWith);
		return int.Parse(code[1].ToString()) - 1 + num * 4;
	}

	private static string SlotIdToString(int slotId)
	{
		int num = slotId % 4 + 1;
		int num2 = slotId / 4;
		return Letters[num2] + num;
	}

	private bool PickARandomSlot(Sprite drink, out int slotId)
	{
		slotId = Slots.RandomIdx();
		return !Slots[slotId].DrinkImage.enabled;
	}

	public void EnterDigit(string s)
	{
		if (!animating && !done)
		{
			if (enteredCode.Length >= 2)
			{
				StartCoroutine(BlinkAccept());
				return;
			}
			enteredCode += s;
			NumberText.Text = enteredCode;
		}
	}

	public void ClearDigits()
	{
		if (!animating)
		{
			enteredCode = string.Empty;
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
		int num = StringToSlotId(enteredCode);
		if (num >= 0 && Slots[num].DrinkImage.enabled)
		{
			yield return Effects.All(CoBlinkVend(), Slots[num].CoBuy());
			if (targetCode == enteredCode)
			{
				done = true;
				MyNormTask.NextStep();
				yield return CoStartClose(0.25f);
			}
		}
		else
		{
			WaitForSeconds wait = new WaitForSeconds(0.1f);
			NumberText.Text = "XXXXXXXX";
			yield return wait;
			NumberText.Text = string.Empty;
			yield return wait;
			NumberText.Text = "XXXXXXXX";
			yield return wait;
		}
		enteredCode = string.Empty;
		NumberText.Text = enteredCode;
		animating = false;
	}

	private IEnumerator CoBlinkVend()
	{
		int i = 0;
		while (i < 5)
		{
			NumberText.Text = "Vending";
			yield return Effects.Wait(0.1f);
			NumberText.Text = string.Empty;
			yield return Effects.Wait(0.1f);
			int num = i + 1;
			i = num;
		}
	}
}
