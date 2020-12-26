using UnityEngine;

public class NumberOption : OptionBehaviour
{
	public TextRenderer TitleText;

	public TextRenderer ValueText;

	public float Value = 1f;

	private float oldValue = float.MaxValue;

	public float Increment;

	public FloatRange ValidRange = new FloatRange(0f, 2f);

	public string FormatString = "{0:0.0}x";

	public bool ZeroIsInfinity;

	public void OnEnable()
	{
		TitleText.Text = Title;
		ValueText.Text = string.Format(FormatString, Value);
		GameOptionsData gameOptions = PlayerControl.GameOptions;
		switch (Title)
		{
		case "Player Speed":
			Value = gameOptions.PlayerSpeedMod;
			break;
		case "Crewmate Vision":
			Value = gameOptions.CrewLightMod;
			break;
		case "Impostor Vision":
			Value = gameOptions.ImpostorLightMod;
			break;
		case "Kill Cooldown":
			Value = gameOptions.KillCooldown;
			break;
		case "# Common Tasks":
			Value = gameOptions.NumCommonTasks;
			break;
		case "# Long Tasks":
			Value = gameOptions.NumLongTasks;
			break;
		case "# Short Tasks":
			Value = gameOptions.NumShortTasks;
			break;
		case "# Impostors":
			Value = gameOptions.NumImpostors;
			break;
		case "# Emergency Meetings":
			Value = gameOptions.NumEmergencyMeetings;
			break;
		case "Discussion Time":
			Value = gameOptions.DiscussionTime;
			break;
		case "Voting Time":
			Value = gameOptions.VotingTime;
			break;
		default:
			Debug.Log("Ono, unrecognized setting: " + Title);
			break;
		}
	}

	private void FixedUpdate()
	{
		if (oldValue != Value)
		{
			oldValue = Value;
			if (ZeroIsInfinity && Mathf.Abs(Value) < 0.0001f)
			{
				ValueText.Text = string.Format(FormatString, "∞");
			}
			else
			{
				ValueText.Text = string.Format(FormatString, Value);
			}
		}
	}

	public void Increase()
	{
		Value = ValidRange.Clamp(Value + Increment);
		OnValueChanged(this);
	}

	public void Decrease()
	{
		Value = ValidRange.Clamp(Value - Increment);
		OnValueChanged(this);
	}

	public override float GetFloat()
	{
		return Value;
	}

	public override int GetInt()
	{
		return (int)Value;
	}
}
