using System;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class StringOption : OptionBehaviour
{
	// Token: 0x060009CE RID: 2510 RVA: 0x00033660 File Offset: 0x00031860
	public void OnEnable()
	{
		this.TitleText.Text = this.Title;
		this.ValueText.Text = this.Values[this.Value];
		GameOptionsData gameOptions = PlayerControl.GameOptions;
		string title = this.Title;
		if (title == "Kill Distance")
		{
			this.Value = gameOptions.KillDistance;
			return;
		}
		if (!(title == "Map"))
		{
			Debug.Log("Ono, unrecognized setting: " + this.Title);
			return;
		}
		this.Value = (int)gameOptions.MapId;
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x00007F35 File Offset: 0x00006135
	private void FixedUpdate()
	{
		if (this.oldValue != this.Value)
		{
			this.oldValue = this.Value;
			this.ValueText.Text = this.Values[this.Value];
		}
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x00007F69 File Offset: 0x00006169
	public void Increase()
	{
		this.Value = Mathf.Clamp(this.Value + 1, 0, this.Values.Length - 1);
		this.OnValueChanged(this);
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x00007F95 File Offset: 0x00006195
	public void Decrease()
	{
		this.Value = Mathf.Clamp(this.Value - 1, 0, this.Values.Length - 1);
		this.OnValueChanged(this);
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x00007FC1 File Offset: 0x000061C1
	public override int GetInt()
	{
		return this.Value;
	}

	// Token: 0x0400096B RID: 2411
	public TextRenderer TitleText;

	// Token: 0x0400096C RID: 2412
	public TextRenderer ValueText;

	// Token: 0x0400096D RID: 2413
	public string[] Values;

	// Token: 0x0400096E RID: 2414
	public int Value;

	// Token: 0x0400096F RID: 2415
	private int oldValue = -1;
}
