using System;
using System.Collections;
using System.Linq;
using UnityEngine;

// Token: 0x0200009E RID: 158
public class VendingMinigame : Minigame
{
	// Token: 0x0600034B RID: 843 RVA: 0x000042BB File Offset: 0x000024BB
	public void OnEnable()
	{
		this.Begin(null);
	}

	// Token: 0x0600034C RID: 844 RVA: 0x000175D8 File Offset: 0x000157D8
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		int num = this.Drinks.RandomIdx<Sprite>();
		this.TargetImage.sprite = this.DrawnDrinks[num];
		for (int i = 0; i < this.Drinks.Length; i++)
		{
			Sprite sprite = this.Drinks[i];
			int num2;
			while (!this.PickARandomSlot(sprite, out num2))
			{
			}
			this.Slots[num2].DrinkImage.enabled = true;
			this.Slots[num2].DrinkImage.sprite = sprite;
			if (num == i)
			{
				this.targetCode = VendingMinigame.SlotIdToString(num2);
			}
		}
		this.NumberText.Text = string.Empty;
	}

	// Token: 0x0600034D RID: 845 RVA: 0x00017678 File Offset: 0x00015878
	private static int StringToSlotId(string code)
	{
		int num;
		if (int.TryParse(code[0].ToString(), out num) || VendingMinigame.Letters.Any(new Func<string, bool>(code.EndsWith)))
		{
			return -1;
		}
		int num2 = VendingMinigame.Letters.IndexOf(new Predicate<string>(code.StartsWith));
		return int.Parse(code[1].ToString()) - 1 + num2 * 4;
	}

	// Token: 0x0600034E RID: 846 RVA: 0x000176E8 File Offset: 0x000158E8
	private static string SlotIdToString(int slotId)
	{
		int num = slotId % 4 + 1;
		int num2 = slotId / 4;
		return VendingMinigame.Letters[num2] + num;
	}

	// Token: 0x0600034F RID: 847 RVA: 0x000042C4 File Offset: 0x000024C4
	private bool PickARandomSlot(Sprite drink, out int slotId)
	{
		slotId = this.Slots.RandomIdx<VendingSlot>();
		return !this.Slots[slotId].DrinkImage.enabled;
	}

	// Token: 0x06000350 RID: 848 RVA: 0x00017714 File Offset: 0x00015914
	public void EnterDigit(string s)
	{
		if (this.animating)
		{
			return;
		}
		if (this.done)
		{
			return;
		}
		if (this.enteredCode.Length >= 2)
		{
			base.StartCoroutine(this.BlinkAccept());
			return;
		}
		this.enteredCode += s;
		this.NumberText.Text = this.enteredCode;
	}

	// Token: 0x06000351 RID: 849 RVA: 0x000042E9 File Offset: 0x000024E9
	public void ClearDigits()
	{
		if (this.animating)
		{
			return;
		}
		this.enteredCode = string.Empty;
		this.NumberText.Text = string.Empty;
	}

	// Token: 0x06000352 RID: 850 RVA: 0x0000430F File Offset: 0x0000250F
	public void AcceptDigits()
	{
		if (this.animating)
		{
			return;
		}
		base.StartCoroutine(this.Animate());
	}

	// Token: 0x06000353 RID: 851 RVA: 0x00004327 File Offset: 0x00002527
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

	// Token: 0x06000354 RID: 852 RVA: 0x00004336 File Offset: 0x00002536
	private IEnumerator Animate()
	{
		this.animating = true;
		int num = VendingMinigame.StringToSlotId(this.enteredCode);
		if (num >= 0 && this.Slots[num].DrinkImage.enabled)
		{
			yield return Effects.All(new IEnumerator[]
			{
				this.CoBlinkVend(),
				this.Slots[num].CoBuy()
			});
			if (this.targetCode == this.enteredCode)
			{
				this.done = true;
				this.MyNormTask.NextStep();
				yield return base.CoStartClose(0.25f);
			}
		}
		else
		{
			WaitForSeconds wait = new WaitForSeconds(0.1f);
			this.NumberText.Text = "XXXXXXXX";
			yield return wait;
			this.NumberText.Text = string.Empty;
			yield return wait;
			this.NumberText.Text = "XXXXXXXX";
			yield return wait;
			wait = null;
		}
		this.enteredCode = string.Empty;
		this.NumberText.Text = this.enteredCode;
		this.animating = false;
		yield break;
	}

	// Token: 0x06000355 RID: 853 RVA: 0x00004345 File Offset: 0x00002545
	private IEnumerator CoBlinkVend()
	{
		int num;
		for (int i = 0; i < 5; i = num)
		{
			this.NumberText.Text = "Vending";
			yield return Effects.Wait(0.1f);
			this.NumberText.Text = string.Empty;
			yield return Effects.Wait(0.1f);
			num = i + 1;
		}
		yield break;
	}

	// Token: 0x04000345 RID: 837
	public static readonly string[] Letters = new string[]
	{
		"a",
		"b",
		"c"
	};

	// Token: 0x04000346 RID: 838
	public TextRenderer NumberText;

	// Token: 0x04000347 RID: 839
	public SpriteRenderer TargetImage;

	// Token: 0x04000348 RID: 840
	public string enteredCode = string.Empty;

	// Token: 0x04000349 RID: 841
	private bool animating;

	// Token: 0x0400034A RID: 842
	private bool done;

	// Token: 0x0400034B RID: 843
	private string targetCode;

	// Token: 0x0400034C RID: 844
	public SpriteRenderer AcceptButton;

	// Token: 0x0400034D RID: 845
	public VendingSlot[] Slots;

	// Token: 0x0400034E RID: 846
	public Sprite[] Drinks;

	// Token: 0x0400034F RID: 847
	public Sprite[] DrawnDrinks;
}
