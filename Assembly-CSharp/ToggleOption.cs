using System;
using UnityEngine;

// Token: 0x02000223 RID: 547
public class ToggleOption : OptionBehaviour
{
	// Token: 0x06000BC8 RID: 3016 RVA: 0x0003A11C File Offset: 0x0003831C
	public void OnEnable()
	{
		this.TitleText.Text = this.Title;
		GameOptionsData gameOptions = PlayerControl.GameOptions;
		string title = this.Title;
		if (title == "Recommended Settings")
		{
			this.CheckMark.enabled = gameOptions.isDefaults;
			return;
		}
		Debug.Log("Ono, unrecognized setting: " + this.Title);
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x0003A17C File Offset: 0x0003837C
	private void FixedUpdate()
	{
		bool @bool = this.GetBool();
		if (this.oldValue != @bool)
		{
			this.oldValue = @bool;
			this.CheckMark.enabled = @bool;
		}
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x00009143 File Offset: 0x00007343
	public void Toggle()
	{
		this.CheckMark.enabled = !this.CheckMark.enabled;
		this.OnValueChanged(this);
	}

	// Token: 0x06000BCB RID: 3019 RVA: 0x0000916A File Offset: 0x0000736A
	public override bool GetBool()
	{
		return this.CheckMark.enabled;
	}

	// Token: 0x04000B5E RID: 2910
	public TextRenderer TitleText;

	// Token: 0x04000B5F RID: 2911
	public SpriteRenderer CheckMark;

	// Token: 0x04000B60 RID: 2912
	private bool oldValue;
}
