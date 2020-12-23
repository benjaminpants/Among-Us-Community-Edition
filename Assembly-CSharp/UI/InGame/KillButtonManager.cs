using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoonSharp.Interpreter;

public class KillButtonManager : MonoBehaviour
{
	public PlayerControl CurrentTarget;

	public SpriteRenderer renderer;

	public TextRenderer TimerText;

	public bool isCoolingDown = true;

	public bool isActive;

	private Vector2 uv;

	private bool isKeyPressKill;

	public void Start()
	{
		renderer.SetCooldownNormalizedUvs();
		PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
		SetTarget(null);
	}

	public void PerformKill()
	{
		if ((!isKeyPressKill && SaveManager.EnableProHUDMode) || !base.isActiveAndEnabled || !CurrentTarget || isCoolingDown || PlayerControl.LocalPlayer.Data.IsDead)
		{
			return;
		}
		if (CE_LuaLoader.CurrentGMLua)
		{
			DynValue dyn = CE_LuaLoader.GetGamemodeResult("BeforeKill", new CE_PlayerInfoLua(PlayerControl.LocalPlayer.Data), new CE_PlayerInfoLua(CurrentTarget.Data));
			if (dyn.Boolean)
			{
				PlayerControl.LocalPlayer.RpcMurderPlayer(CurrentTarget);
			}
			PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
		}
		else
        {
			PlayerControl.LocalPlayer.RpcMurderPlayer(CurrentTarget);
		}
		SetTarget(null);
	}

	public void SetTarget(PlayerControl target)
	{
		if ((bool)CurrentTarget && CurrentTarget != target)
		{
			CurrentTarget.GetComponent<SpriteRenderer>().material.SetFloat("_Outline", 0f);
		}
		CurrentTarget = target;
		if ((bool)CurrentTarget)
		{
			SpriteRenderer component = CurrentTarget.GetComponent<SpriteRenderer>();
			component.material.SetFloat("_Outline", isActive ? 1 : 0);
			component.material.SetColor("_OutlineColor", (PlayerControl.LocalPlayer.Data.role != 0) ? CE_RoleManager.GetRoleFromID(PlayerControl.LocalPlayer.Data.role).RoleColor : Color.red);
			renderer.color = Palette.EnabledColor;
			renderer.material.SetFloat("_Desat", 0f);
		}
		else
		{
			renderer.color = Palette.DisabledColor;
			renderer.material.SetFloat("_Desat", 1f);
		}
	}

	public void SetCoolDown(float timer, float maxTimer)
	{
		float num = Mathf.Clamp(timer / maxTimer, 0f, 1f);
		if ((bool)renderer)
		{
			renderer.material.SetFloat("_Percent", num);
		}
		isCoolingDown = num > 0f;
		if (isCoolingDown)
		{
			TimerText.Text = Mathf.CeilToInt(timer).ToString();
			TimerText.gameObject.SetActive(value: true);
		}
		else
		{
			TimerText.gameObject.SetActive(value: false);
		}
	}

	public void PerformKeybindKill()
	{
		isKeyPressKill = true;
		PerformKill();
		isKeyPressKill = false;
	}
}
