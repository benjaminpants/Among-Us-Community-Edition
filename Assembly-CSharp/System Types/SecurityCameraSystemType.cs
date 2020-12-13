using Hazel;

public class SecurityCameraSystemType : ISystemType
{
	public const byte IncrementOp = 1;

	public const byte DecrementOp = 2;

	public byte InUse;

	private bool CamsOn;

	public bool Detoriorate(float deltaTime)
	{
		if (CamsOn)
		{
			if (InUse == 0)
			{
				CamsOn = false;
				for (int i = 0; i < ShipStatus.Instance.AllRooms.Length; i++)
				{
					ShipRoom shipRoom = ShipStatus.Instance.AllRooms[i];
					if ((bool)shipRoom.survCamera)
					{
						shipRoom.survCamera.Image.Play(shipRoom.survCamera.OffAnim);
					}
				}
			}
		}
		else if (InUse > 0)
		{
			CamsOn = true;
			for (int j = 0; j < ShipStatus.Instance.AllRooms.Length; j++)
			{
				ShipRoom shipRoom2 = ShipStatus.Instance.AllRooms[j];
				if ((bool)shipRoom2.survCamera)
				{
					shipRoom2.survCamera.Image.Play(shipRoom2.survCamera.OnAnim);
				}
			}
		}
		return false;
	}

	public void RepairDamage(PlayerControl player, byte amount)
	{
		if (amount == 1)
		{
			InUse++;
		}
		else
		{
			InUse--;
		}
	}

	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.Write(InUse);
	}

	public void Deserialize(MessageReader reader, bool initialState)
	{
		InUse = reader.ReadByte();
	}
}
