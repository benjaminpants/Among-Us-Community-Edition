using System;
using Hazel;
using UnityEngine;

public class DeconSystem : MonoBehaviour, ISystemType
{
	[Flags]
	private enum States : byte
	{
		Idle = 0x0,
		Enter = 0x1,
		Closed = 0x2,
		Exit = 0x4,
		HeadingUp = 0x8
	}

	private const byte HeadUpCmd = 1;

	private const byte HeadDownCmd = 2;

	private const byte HeadUpInsideCmd = 3;

	private const byte HeadDownInsideCmd = 4;

	public ManualDoor UpperDoor;

	public ManualDoor LowerDoor;

	public float DoorOpenTime = 5f;

	public float DeconTime = 5f;

	private States curState;

	private float timer;

	public TextRenderer FloorText;

	public bool Detoriorate(float dt)
	{
		int num = Mathf.CeilToInt(timer);
		timer = Mathf.Max(0f, timer - dt);
		int num2 = Mathf.CeilToInt(timer);
		if (num != num2)
		{
			if (num2 == 0)
			{
				if (curState.HasFlag(States.Enter))
				{
					curState = (curState & ~States.Enter) | States.Closed;
					timer = DeconTime;
				}
				else if (curState.HasFlag(States.Closed))
				{
					curState = (curState & ~States.Closed) | States.Exit;
					timer = DoorOpenTime;
				}
				else if (curState.HasFlag(States.Exit))
				{
					curState = States.Idle;
				}
			}
			UpdateDoorsViaState();
			return true;
		}
		return false;
	}

	public void OpenDoor(bool upper)
	{
		if (curState == States.Idle)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Decontamination, (!upper) ? 1 : 2);
		}
	}

	public void OpenFromInside(bool upper)
	{
		if (curState == States.Idle)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Decontamination, upper ? 3 : 4);
		}
	}

	public void RepairDamage(PlayerControl player, byte amount)
	{
		if (curState == States.Idle)
		{
			switch (amount)
			{
			case 1:
				curState = States.Enter | States.HeadingUp;
				timer = DoorOpenTime;
				break;
			case 2:
				curState = States.Enter;
				timer = DoorOpenTime;
				break;
			case 3:
				curState = States.Exit | States.HeadingUp;
				timer = DoorOpenTime;
				break;
			case 4:
				curState = States.Exit;
				timer = DoorOpenTime;
				break;
			}
			UpdateDoorsViaState();
		}
	}

	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.Write((byte)Mathf.CeilToInt(timer));
		writer.Write((byte)curState);
	}

	public void Deserialize(MessageReader reader, bool initialState)
	{
		timer = (int)reader.ReadByte();
		curState = (States)reader.ReadByte();
		UpdateDoorsViaState();
	}

	private void UpdateDoorsViaState()
	{
		int num = Mathf.CeilToInt(timer);
		if (num > 0)
		{
			FloorText.Text = string.Join("\n", num, num, num, num);
		}
		else
		{
			FloorText.Text = string.Empty;
		}
		if (curState.HasFlag(States.Enter))
		{
			bool flag = curState.HasFlag(States.HeadingUp);
			LowerDoor.SetDoorway(flag);
			UpperDoor.SetDoorway(!flag);
		}
		else if (curState.HasFlag(States.Closed) || curState == States.Idle)
		{
			LowerDoor.SetDoorway(open: false);
			UpperDoor.SetDoorway(open: false);
		}
		else if (curState.HasFlag(States.Exit))
		{
			bool flag2 = curState.HasFlag(States.HeadingUp);
			LowerDoor.SetDoorway(!flag2);
			UpperDoor.SetDoorway(flag2);
		}
	}
}
