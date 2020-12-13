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
			door.SetCooldownNormalizedUvs();
		}
		if ((bool)special)
		{
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
		if (Parent.CanUseSpecial)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Sabotage, 3);
		}
	}

	public void SabotageComms()
	{
		if (Parent.CanUseSpecial)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Sabotage, 14);
		}
	}

	public void SabotageOxygen()
	{
		if (Parent.CanUseSpecial)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Sabotage, 8);
		}
	}

	public void SabotageLights()
	{
		if (Parent.CanUseSpecial)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Sabotage, 7);
		}
	}

	public void SabotageDoors()
	{
		if (Parent.CanUseDoors && !(((DoorsSystemType)ShipStatus.Instance.Systems[SystemTypes.Doors]).GetTimer(room) > 0f))
		{
			ShipStatus.Instance.RpcCloseDoorsOfType(room);
		}
	}
}
