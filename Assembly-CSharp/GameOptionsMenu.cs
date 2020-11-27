using System;
using UnityEngine;

// Token: 0x020001BD RID: 445
public class GameOptionsMenu : MonoBehaviour
{
	// Token: 0x060009A5 RID: 2469 RVA: 0x000328D8 File Offset: 0x00030AD8
	public void Start()
	{
		this.Children = base.GetComponentsInChildren<OptionBehaviour>();
		this.cachedData = PlayerControl.GameOptions;
		for (int i = 0; i < this.Children.Length; i++)
		{
			OptionBehaviour optionBehaviour = this.Children[i];
			optionBehaviour.OnValueChanged = new Action<OptionBehaviour>(this.ValueChanged);
			if (AmongUsClient.Instance && !AmongUsClient.Instance.AmHost)
			{
				optionBehaviour.SetAsPlayer();
			}
		}
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x00032948 File Offset: 0x00030B48
	public void Update()
	{
		if (this.cachedData != PlayerControl.GameOptions)
		{
			this.cachedData = PlayerControl.GameOptions;
			this.RefreshChildren();
		}
		if (!AmongUsClient.Instance.AmHost)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow) && this.selectedcustom != 0)
		{
			this.selectedcustom--;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow) && this.selectedcustom != 6)
		{
			this.selectedcustom++;
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			GameOptionsData gameOptions = PlayerControl.GameOptions;
			if (this.selectedcustom == 0 && gameOptions.Venting != 3)
			{
				GameOptionsData gameOptionsData = gameOptions;
				gameOptionsData.Venting += 1;
			}
			if (this.selectedcustom == 1 && gameOptions.VentMode != 3)
			{
				GameOptionsData gameOptionsData2 = gameOptions;
				gameOptionsData2.VentMode += 1;
			}
			if (this.selectedcustom == 2)
			{
				gameOptions.AnonVotes = !gameOptions.AnonVotes;
			}
			if (this.selectedcustom == 4)
			{
				gameOptions.Visuals = !gameOptions.Visuals;
			}
			if (this.selectedcustom == 3)
			{
				gameOptions.ConfirmEject = !gameOptions.ConfirmEject;
			}
			if (this.selectedcustom == 5 && gameOptions.Gamemode != 2)
			{
				GameOptionsData gameOptionsData3 = gameOptions;
				gameOptionsData3.Gamemode += 1;
			}
			if (this.selectedcustom == 6 && gameOptions.SabControl != 4)
			{
				GameOptionsData gameOptionsData4 = gameOptions;
				gameOptionsData4.SabControl += 1;
			}
			this.Sync();
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			GameOptionsData gameOptions2 = PlayerControl.GameOptions;
			if (this.selectedcustom == 0 && gameOptions2.Venting != 0)
			{
				GameOptionsData gameOptionsData5 = gameOptions2;
				gameOptionsData5.Venting -= 1;
			}
			if (this.selectedcustom == 1 && gameOptions2.VentMode != 0)
			{
				GameOptionsData gameOptionsData6 = gameOptions2;
				gameOptionsData6.VentMode -= 1;
			}
			if (this.selectedcustom == 5 && gameOptions2.Gamemode != 0)
			{
				GameOptionsData gameOptionsData7 = gameOptions2;
				gameOptionsData7.Gamemode -= 1;
			}
			if (this.selectedcustom == 6 && gameOptions2.SabControl != 0)
			{
				GameOptionsData gameOptionsData8 = gameOptions2;
				gameOptionsData8.SabControl -= 1;
			}
			this.Sync();
		}
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x00032B48 File Offset: 0x00030D48
	private void RefreshChildren()
	{
		for (int i = 0; i < this.Children.Length; i++)
		{
			OptionBehaviour optionBehaviour = this.Children[i];
			optionBehaviour.enabled = false;
			optionBehaviour.enabled = true;
		}
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x00032B80 File Offset: 0x00030D80
	public void ValueChanged(OptionBehaviour option)
	{
		if (!AmongUsClient.Instance || !AmongUsClient.Instance.AmHost)
		{
			return;
		}
		if (option.Title == "Recommended Settings")
		{
			if (this.cachedData.isDefaults)
			{
				this.cachedData.isDefaults = false;
			}
			else
			{
				this.cachedData.SetRecommendations(GameData.Instance.PlayerCount, AmongUsClient.Instance.GameMode);
			}
			this.RefreshChildren();
		}
		else
		{
			GameOptionsData gameOptions = PlayerControl.GameOptions;
			string title = option.Title;
			uint num = PrivateImplementationDetails.ComputeStringHash(title);
			if (num <= 1551499986U)
			{
				if (num <= 568237726U)
				{
					if (num != 536396U)
					{
						if (num != 467257133U)
						{
							if (num == 568237726U)
							{
								if (title == "Kill Cooldown")
								{
									gameOptions.KillCooldown = option.GetFloat();
									goto IL_32C;
								}
							}
						}
						else if (title == "# Common Tasks")
						{
							gameOptions.NumCommonTasks = option.GetInt();
							goto IL_32C;
						}
					}
					else if (title == "# Impostors")
					{
						gameOptions.NumImpostors = option.GetInt();
						goto IL_32C;
					}
				}
				else if (num != 998922042U)
				{
					if (num != 1151856721U)
					{
						if (num == 1551499986U)
						{
							if (title == "# Long Tasks")
							{
								gameOptions.NumLongTasks = option.GetInt();
								goto IL_32C;
							}
						}
					}
					else if (title == "Map")
					{
						gameOptions.MapId = (byte)option.GetInt();
						goto IL_32C;
					}
				}
				else if (title == "Discussion Time")
				{
					gameOptions.DiscussionTime = option.GetInt();
					goto IL_32C;
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
								gameOptions.ImpostorLightMod = option.GetFloat();
								goto IL_32C;
							}
						}
					}
					else if (title == "Crewmate Vision")
					{
						gameOptions.CrewLightMod = option.GetFloat();
						goto IL_32C;
					}
				}
				else if (title == "# Emergency Meetings")
				{
					gameOptions.NumEmergencyMeetings = option.GetInt();
					goto IL_32C;
				}
			}
			else if (num <= 2910279947U)
			{
				if (num != 2591742836U)
				{
					if (num == 2910279947U)
					{
						if (title == "Voting Time")
						{
							gameOptions.VotingTime = option.GetInt();
							goto IL_32C;
						}
					}
				}
				else if (title == "Kill Distance")
				{
					gameOptions.KillDistance = option.GetInt();
					goto IL_32C;
				}
			}
			else if (num != 3062221931U)
			{
				if (num == 4029529074U)
				{
					if (title == "# Short Tasks")
					{
						gameOptions.NumShortTasks = option.GetInt();
						goto IL_32C;
					}
				}
			}
			else if (title == "Player Speed")
			{
				gameOptions.PlayerSpeedMod = option.GetFloat();
				goto IL_32C;
			}
			Debug.Log("Ono, unrecognized setting: " + option.Title);
			IL_32C:
			if (gameOptions.isDefaults && option.Title != "Map")
			{
				gameOptions.isDefaults = false;
				this.RefreshChildren();
			}
		}
		PlayerControl localPlayer = PlayerControl.LocalPlayer;
		if (localPlayer == null)
		{
			return;
		}
		localPlayer.RpcSyncSettings(PlayerControl.GameOptions);
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x00032EF4 File Offset: 0x000310F4
	public void Sync()
	{
		PlayerControl localPlayer = PlayerControl.LocalPlayer;
		if (localPlayer == null)
		{
			return;
		}
		localPlayer.RpcSyncSettings(PlayerControl.GameOptions);
	}

	// Token: 0x04000947 RID: 2375
	private GameOptionsData cachedData;

	// Token: 0x04000948 RID: 2376
	public GameObject ResetButton;

	// Token: 0x04000949 RID: 2377
	private OptionBehaviour[] Children;

	// Token: 0x0400094A RID: 2378
	public int selectedcustom;
}
