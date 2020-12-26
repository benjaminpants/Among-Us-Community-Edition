using UnityEngine;

public class ToggleOption : OptionBehaviour
{
	public TextRenderer TitleText;

	public SpriteRenderer CheckMark;

	private bool oldValue;

	public void OnEnable()
	{
		TitleText.Text = Title;
		GameOptionsData gameOptions = PlayerControl.GameOptions;
		string title = Title;
		if (title == "Recommended Settings")
		{
			CheckMark.enabled = gameOptions.isDefaults;
		}
		else
		{
			Debug.Log("Ono, unrecognized setting: " + Title);
		}
	}

	private void FixedUpdate()
	{
		bool @bool = GetBool();
		if (oldValue != @bool)
		{
			oldValue = @bool;
			CheckMark.enabled = @bool;
		}
	}

	public void Toggle()
	{
		CheckMark.enabled = !CheckMark.enabled;
		OnValueChanged(this);
	}

	public override bool GetBool()
	{
		return CheckMark.enabled;
	}
}
