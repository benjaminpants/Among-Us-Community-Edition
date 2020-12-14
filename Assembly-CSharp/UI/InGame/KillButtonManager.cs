using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
		if (PlayerControl.GameOptions.Gamemode == 3)
		{
			List<GameData.PlayerInfo> list = new List<GameData.PlayerInfo>();
			foreach (GameData.PlayerInfo allPlayer in GameData.Instance.AllPlayers)
			{
				if (allPlayer.IsImpostor)
				{
					list.Add(allPlayer);
				}
			}
			list.Add(CurrentTarget.Data);
			using List<PlayerControl>.Enumerator enumerator2 = PlayerControl.AllPlayerControls.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				_ = enumerator2.Current;
				PlayerControl.LocalPlayer.RpcSetInfectedNoIntro(list.ToArray());
			}
			PlayerControl.LocalPlayer.RpcMurderPlayer(PlayerControl.LocalPlayer);
		}
		else
		{
			PlayerControl.LocalPlayer.RpcMurderPlayer(CurrentTarget);
		}
		Debug.Log("role: " + PlayerControl.LocalPlayer.Data.role);
		if (PlayerControl.LocalPlayer.Data.role == GameData.PlayerInfo.Role.Sheriff)
		{
			PlayerControl.LocalPlayer.nameText.Color = Palette.White;
			base.gameObject.SetActive(value: false);
			List<GameData.PlayerInfo> list2 = (from pcd in GameData.Instance.AllPlayers
				where !pcd.Disconnected
				select pcd into pc
				where !pc.IsDead
				select pc into pci
				where !pci.IsImpostor
				select pci into pcs
				where pcs != PlayerControl.LocalPlayer.Data
				select pcs).ToList();
			list2.Shuffle();
			GameData.PlayerInfo.Role[] roles = new GameData.PlayerInfo.Role[2]
			{
				GameData.PlayerInfo.Role.None,
				GameData.PlayerInfo.Role.Sheriff
			};
			PlayerControl.LocalPlayer.RpcSetRole(new GameData.PlayerInfo[2]
			{
				PlayerControl.LocalPlayer.Data,
				list2.Take(1).ToArray()[0]
			}, roles);
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
			component.material.SetColor("_OutlineColor", (PlayerControl.LocalPlayer.Data.role == GameData.PlayerInfo.Role.Sheriff) ? Palette.SheriffYellow : Color.red);
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
