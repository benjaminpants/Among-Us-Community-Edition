using UnityEngine;

public class InfectedOverlay : MonoBehaviour
{
	public MapRoom[] rooms;

	private IActivatable doors;

	private SabotageSystemType SabSystem;

	public bool CanUseDoors => !SabSystem.AnyActive;

	public bool CanUseSpecial
	{
		get
		{
			if (SabSystem.Timer <= 0f)
			{
				return !doors.IsActive;
			}
			return false;
		}
	}

	public void Start()
	{
		for (int i = 0; i < rooms.Length; i++)
		{
			rooms[i].Parent = this;
		}
		SabSystem = (SabotageSystemType)ShipStatus.Instance.Systems[SystemTypes.Sabotage];
		doors = (IActivatable)ShipStatus.Instance.Systems[SystemTypes.Doors];
	}

	private void FixedUpdate()
	{
		if (doors != null)
		{
			float specialActive = (doors.IsActive ? 1f : SabSystem.PercentCool);
			for (int i = 0; i < rooms.Length; i++)
			{
				rooms[i].SetSpecialActive(specialActive);
				rooms[i].OOBUpdate();
			}
		}
	}
}
