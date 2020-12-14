using UnityEngine;

public class MapRoom : MonoBehaviour
{
	public SystemTypes room;

	public SpriteRenderer door;

	public SpriteRenderer special;

	public InfectedOverlay Parent
	{
		get;
		set;
	}

	public void Start()
	{
		if ((bool)door)
		{
			if (PlayerControl.GameOptions.SabControl == 3 || PlayerControl.GameOptions.SabControl == 4)
			{
				Object.Destroy(base.transform.gameObject);
			}
			door.SetCooldownNormalizedUvs();
		}
		if ((bool)special)
		{
			if (PlayerControl.GameOptions.SabControl == 3 || PlayerControl.GameOptions.SabControl == 4)
			{
				Object.Destroy(base.transform.gameObject);
			}
			special.SetCooldownNormalizedUvs();
		}
	}

	public void OOBUpdate()
	{
		if ((bool)door && (bool)ShipStatus.Instance)
		{
			float timer = ((DoorsSystemType)ShipStatus.Instance.Systems[SystemTypes.Doors]).GetTimer(room);
			float value = (Parent.CanUseDoors ? (timer / 30f) : 1f);
			door.material.SetFloat("_Percent", value);
		}
	}

	internal void SetSpecialActive(float perc)
	{
		if ((bool)special)
		{
			special.material.SetFloat("_Percent", perc);
		}
	}

	public void SabotageReactor()
	{
		if (PlayerControl.GameOptions.SabControl != 2 && PlayerControl.GameOptions.SabControl != 3 && PlayerControl.GameOptions.SabControl != 4 && Parent.CanUseSpecial)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Sabotage, 3);
		}
	}

	public void SabotageComms()
	{
		if (PlayerControl.GameOptions.SabControl != 2 && PlayerControl.GameOptions.SabControl != 3 && PlayerControl.GameOptions.SabControl != 4 && Parent.CanUseSpecial)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Sabotage, 14);
		}
	}

	public void SabotageOxygen()
	{
		if (PlayerControl.GameOptions.SabControl != 2 && PlayerControl.GameOptions.SabControl != 3 && PlayerControl.GameOptions.SabControl != 4 && Parent.CanUseSpecial)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Sabotage, 8);
		}
	}

	public void SabotageLights()
	{
		if (PlayerControl.GameOptions.SabControl != 2 && PlayerControl.GameOptions.SabControl != 3 && PlayerControl.GameOptions.SabControl != 4 && Parent.CanUseSpecial)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Sabotage, 7);
		}
	}

	public void SabotageDoors()
	{
		if (PlayerControl.GameOptions.SabControl != 1 && PlayerControl.GameOptions.SabControl != 3 && PlayerControl.GameOptions.SabControl != 4 && Parent.CanUseDoors && !(((DoorsSystemType)ShipStatus.Instance.Systems[SystemTypes.Doors]).GetTimer(room) > 0f))
		{
			ShipStatus.Instance.RpcCloseDoorsOfType(room);
		}
	}
}
