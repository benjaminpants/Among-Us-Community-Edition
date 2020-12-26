using UnityEngine;

public class StringOption : OptionBehaviour
{
	public TextRenderer TitleText;

	public TextRenderer ValueText;

	public string[] Values;

	public int Value;

	private int oldValue = -1;

	public void OnEnable()
	{
		TitleText.Text = Title;
		ValueText.Text = Values[Value];
		GameOptionsData gameOptions = PlayerControl.GameOptions;
		string title = Title;
		if (!(title == "Kill Distance"))
		{
			if (title == "Map")
			{
				Value = gameOptions.MapId;
			}
			else
			{
				Debug.Log("Ono, unrecognized setting: " + Title);
			}
		}
		else
		{
			Value = gameOptions.KillDistance;
		}
	}

	private void FixedUpdate()
	{
		if (oldValue != Value)
		{
			oldValue = Value;
			ValueText.Text = Values[Value];
		}
	}

	public void Increase()
	{
		Value = Mathf.Clamp(Value + 1, 0, Values.Length - 1);
		OnValueChanged(this);
	}

	public void Decrease()
	{
		Value = Mathf.Clamp(Value - 1, 0, Values.Length - 1);
		OnValueChanged(this);
	}

	public override int GetInt()
	{
		return Value;
	}
}
