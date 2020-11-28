using System;
using UnityEngine;

// Token: 0x020000CC RID: 204
public class KeyboardJoystick : MonoBehaviour, IVirtualJoystick
{
	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x06000451 RID: 1105 RVA: 0x00004C28 File Offset: 0x00002E28
	public Vector2 Delta
	{
		get
		{
			return this.del;
		}
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x0001A6EC File Offset: 0x000188EC
	private void FixedUpdate()
	{
		if (!PlayerControl.LocalPlayer)
		{
			return;
		}
		this.del.x = (this.del.y = 0f);
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			this.del.x = this.del.x + 1f;
		}
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			this.del.x = this.del.x - 1f;
		}
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
		{
			this.del.y = this.del.y + 1f;
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
		{
			this.del.y = this.del.y - 1f;
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			DestroyableSingleton<HudManager>.Instance.ReportButton.DoClick();
		}
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            DestroyableSingleton<HudManager>.Instance.UseButton.DoClick();
        }
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			DestroyableSingleton<HudManager>.Instance.OpenMap();
		}
		if ((PlayerControl.LocalPlayer.Data.IsImpostor || PlayerControl.LocalPlayer.Data.role == GameData.PlayerInfo.Role.Sheriff) && Input.GetKeyDown(KeyCode.Q))
		{
			DestroyableSingleton<HudManager>.Instance.KillButton.PerformKill();
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (Minigame.Instance)
			{
				Minigame.Instance.Close();
			}
			else if (DestroyableSingleton<HudManager>.InstanceExists && MapBehaviour.Instance && MapBehaviour.Instance.IsOpen)
			{
				MapBehaviour.Instance.Close();
			}
			else
			{
				CustomPlayerMenu customPlayerMenu = UnityEngine.Object.FindObjectOfType<CustomPlayerMenu>();
				if (customPlayerMenu)
				{
					customPlayerMenu.Close(true);
				}
			}
		}
		this.del.Normalize();
	}

	// Token: 0x0400043F RID: 1087
	private Vector2 del;
}
