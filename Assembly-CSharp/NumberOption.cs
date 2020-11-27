using System;
using UnityEngine;

// Token: 0x020001BE RID: 446
public class NumberOption : OptionBehaviour
{
	// Token: 0x060009AB RID: 2475 RVA: 0x00032F1C File Offset: 0x0003111C
	public void OnEnable()
	{
		this.TitleText.Text = this.Title;
		this.ValueText.Text = string.Format(this.FormatString, this.Value);
		GameOptionsData gameOptions = PlayerControl.GameOptions;
		string title = this.Title;
		uint num = PrivateImplementationDetails.ComputeStringHash(title);
		if (num <= 1551499986U)
		{
			if (num <= 467257133U)
			{
				if (num != 536396U)
				{
					if (num == 467257133U)
					{
						if (title == "# Common Tasks")
						{
							this.Value = (float)gameOptions.NumCommonTasks;
							return;
						}
					}
				}
				else if (title == "# Impostors")
				{
					this.Value = (float)gameOptions.NumImpostors;
					return;
				}
			}
			else if (num != 568237726U)
			{
				if (num != 998922042U)
				{
					if (num == 1551499986U)
					{
						if (title == "# Long Tasks")
						{
							this.Value = (float)gameOptions.NumLongTasks;
							return;
						}
					}
				}
				else if (title == "Discussion Time")
				{
					this.Value = (float)gameOptions.DiscussionTime;
					return;
				}
			}
			else if (title == "Kill Cooldown")
			{
				this.Value = gameOptions.KillCooldown;
				return;
			}
		}
		else if (num <= 2473026420U)
		{
			if (num != 1836768579U)
			{
				if (num != 2459083587U)
				{
					if (num == 2473026420U)
					{
						if (title == "Impostor Vision")
						{
							this.Value = gameOptions.ImpostorLightMod;
							return;
						}
					}
				}
				else if (title == "Crewmate Vision")
				{
					this.Value = gameOptions.CrewLightMod;
					return;
				}
			}
			else if (title == "# Emergency Meetings")
			{
				this.Value = (float)gameOptions.NumEmergencyMeetings;
				return;
			}
		}
		else if (num != 2910279947U)
		{
			if (num != 3062221931U)
			{
				if (num == 4029529074U)
				{
					if (title == "# Short Tasks")
					{
						this.Value = (float)gameOptions.NumShortTasks;
						return;
					}
				}
			}
			else if (title == "Player Speed")
			{
				this.Value = gameOptions.PlayerSpeedMod;
				return;
			}
		}
		else if (title == "Voting Time")
		{
			this.Value = (float)gameOptions.VotingTime;
			return;
		}
		Debug.Log("Ono, unrecognized setting: " + this.Title);
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x000331A0 File Offset: 0x000313A0
	private void FixedUpdate()
	{
		if (this.oldValue != this.Value)
		{
			this.oldValue = this.Value;
			if (this.ZeroIsInfinity && Mathf.Abs(this.Value) < 0.0001f)
			{
				this.ValueText.Text = string.Format(this.FormatString, "∞");
				return;
			}
			this.ValueText.Text = string.Format(this.FormatString, this.Value);
		}
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x00007D1A File Offset: 0x00005F1A
	public void Increase()
	{
		this.Value = this.ValidRange.Clamp(this.Value + this.Increment);
		this.OnValueChanged(this);
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x00007D46 File Offset: 0x00005F46
	public void Decrease()
	{
		this.Value = this.ValidRange.Clamp(this.Value - this.Increment);
		this.OnValueChanged(this);
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x00007D72 File Offset: 0x00005F72
	public override float GetFloat()
	{
		return this.Value;
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x00007D7A File Offset: 0x00005F7A
	public override int GetInt()
	{
		return (int)this.Value;
	}

	// Token: 0x0400094B RID: 2379
	public TextRenderer TitleText;

	// Token: 0x0400094C RID: 2380
	public TextRenderer ValueText;

	// Token: 0x0400094D RID: 2381
	public float Value = 1f;

	// Token: 0x0400094E RID: 2382
	private float oldValue = float.MaxValue;

	// Token: 0x0400094F RID: 2383
	public float Increment;

	// Token: 0x04000950 RID: 2384
	public FloatRange ValidRange = new FloatRange(0f, 2f);

	// Token: 0x04000951 RID: 2385
	public string FormatString = "{0:0.0}x";

	// Token: 0x04000952 RID: 2386
	public bool ZeroIsInfinity;
}
