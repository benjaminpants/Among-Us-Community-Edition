using UnityEngine;
using System.Collections.Generic;

public class KeyboardJoystick : MonoBehaviour, IVirtualJoystick
{
	private Vector2 del;

	public Vector2 Delta => del;

	private void FixedUpdate()
	{
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
			DestroyableSingleton<HudManager>.Instance.KillButton.PerformKeybindKill();
		}
		if (CE_Input.CE_GetKeyDown(KeyCode.Escape))
		{
			CE_Input.EscapeFunctionality();
		}
		del.Normalize();
	}

}
