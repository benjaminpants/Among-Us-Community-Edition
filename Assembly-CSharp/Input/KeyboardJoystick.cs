using UnityEngine;
using System.Collections.Generic;

public class KeyboardJoystick : MonoBehaviour, IVirtualJoystick
{
	private Vector2 del;

	public Vector2 Delta => del;

	public bool IsSprinting = false; //currently unusable, please fix

	private void FixedUpdate()
	{
		IsSprinting = false;
		if (CE_UIHelpers.IsActive() || !PlayerControl.LocalPlayer)
		{
			return;
		}
		del.x = (del.y = 0f);
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			del.x += 1f;
		}
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			del.x -= 1f;
		}
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
		{
			del.y += 1f;
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
		{
			del.y -= 1f;
		}
		if (CE_Input.CE_GetKeyDown(KeyCode.R))
		{
			DestroyableSingleton<HudManager>.Instance.ReportButton.DoKeyClick();
		}
		if (CE_Input.CE_GetKeyDown(KeyCode.Space) || CE_Input.CE_GetKeyDown(KeyCode.E))
		{
			DestroyableSingleton<HudManager>.Instance.UseButton.DoKeyClick();
		}
		if (CE_Input.CE_GetKeyDown(KeyCode.Tab) && !(bool)LobbyBehaviour.Instance)
		{
			DestroyableSingleton<HudManager>.Instance.OpenMap();
		}
		if (CE_Input.CE_GetKeyDown(KeyCode.F) && PlayerControl.LocalPlayer.Data.IsImpostor && !(bool)LobbyBehaviour.Instance)
		{
			DestroyableSingleton<HudManager>.Instance.OpenInfectedMap();
		}
		if ((PlayerControl.LocalPlayer.Data.IsImpostor || CE_RoleManager.GetRoleFromID(PlayerControl.LocalPlayer.Data.role).CanDo(CE_Specials.Kill)) && CE_Input.CE_GetKeyDown(KeyCode.Q))
		{
			DestroyableSingleton<HudManager>.Instance.KillButton.PerformKeybindKill(); //THIS IS DUMB BUT APPARENTLY ITS NECESSARY????
		}
		if (CE_Input.CE_GetKeyDown(KeyCode.Escape))
		{
			CE_Input.EscapeFunctionality();
		}
		del.Normalize();
		return; //temp disabled
		if (PlayerControl.GameOptions != null) //NOTE: Optimzie this later please I beg of you
		{
			bool cansprint = false;
			if (PlayerControl.LocalPlayer.Data != null)
			{
				cansprint = (PlayerControl.GameOptions.SneakAllowance == 0) || (PlayerControl.GameOptions.SneakAllowance == 1 && PlayerControl.LocalPlayer.Data.IsImpostor) || (PlayerControl.GameOptions.SneakAllowance == 4 && PlayerControl.LocalPlayer.Data.IsDead);
				if (PlayerControl.GameOptions.SneakAllowance == 2)
                {
					return;
                }
			}
			else
            {
				IsSprinting = Input.GetKey(KeyCode.LeftShift);
				del *= (Input.GetKey(KeyCode.LeftShift) ? (PlayerControl.GameOptions == null ? 0.5f : PlayerControl.GameOptions.SprintMultipler) : 1f);
				return;
			}
			if (PlayerControl.GameOptions.SneakAllowance == 3)
            {
				if (CE_LuaLoader.CurrentGMLua)
				{
					cansprint = CE_LuaLoader.GetGamemodeResult("CanSneak", (CE_PlayerInfoLua)PlayerControl.LocalPlayer.Data, PlayerControl.GameOptions.SneakAllowance == 0).Boolean;
				}
            }
			else
            {
				if (CE_LuaLoader.CurrentGMLua)
				{
					cansprint = cansprint && CE_LuaLoader.GetGamemodeResult("CanSneak", (CE_PlayerInfoLua)PlayerControl.LocalPlayer.Data, PlayerControl.GameOptions.SneakAllowance == 0).Boolean;
				}
			}
			if (cansprint)
            {
				IsSprinting = Input.GetKey(KeyCode.LeftShift);
				del *= (Input.GetKey(KeyCode.LeftShift) ? (PlayerControl.GameOptions == null ? 0.5f : PlayerControl.GameOptions.SprintMultipler) : 1f);
				return;
			}
		
		}
		else
		{
			del *= (Input.GetKey(KeyCode.LeftShift) ? (PlayerControl.GameOptions == null ? 0.5f : PlayerControl.GameOptions.SprintMultipler) : 1f);
		}
		IsSprinting = Input.GetKey(KeyCode.LeftShift);
	}

}
