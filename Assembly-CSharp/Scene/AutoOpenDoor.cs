using System;

public class AutoOpenDoor : ManualDoor
{
	private const float ClosedDuration = 10f;

	public SystemTypes Room;

	public float ClosedTimer;

	public float CooldownTimer;

	public bool DoUpdate(float dt)
	{
		CooldownTimer = Math.Max(CooldownTimer - dt, 0f);
		if (ClosedTimer > 0f)
		{
			ClosedTimer = Math.Max(ClosedTimer - dt, 0f);
			if (ClosedTimer == 0f)
			{
				SetDoorway(open: true);
				return true;
			}
		}
		return false;
	}

	public override void SetDoorway(bool open)
	{
		if (!open)
		{
			ClosedTimer = 10f;
			CooldownTimer = 30f;
		}
		base.SetDoorway(open);
	}
}
