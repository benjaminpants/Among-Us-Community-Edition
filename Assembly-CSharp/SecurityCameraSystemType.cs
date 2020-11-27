using System;
using Hazel;

// Token: 0x020001B1 RID: 433
public class SecurityCameraSystemType : ISystemType
{
	// Token: 0x06000962 RID: 2402 RVA: 0x00031400 File Offset: 0x0002F600
	public bool Detoriorate(float deltaTime)
	{
		if (this.CamsOn)
		{
			if (this.InUse == 0)
			{
				this.CamsOn = false;
				for (int i = 0; i < ShipStatus.Instance.AllRooms.Length; i++)
				{
					ShipRoom shipRoom = ShipStatus.Instance.AllRooms[i];
					if (shipRoom.survCamera)
					{
						shipRoom.survCamera.Image.Play(shipRoom.survCamera.OffAnim, 1f);
					}
				}
			}
		}
		else if (this.InUse > 0)
		{
			this.CamsOn = true;
			for (int j = 0; j < ShipStatus.Instance.AllRooms.Length; j++)
			{
				ShipRoom shipRoom2 = ShipStatus.Instance.AllRooms[j];
				if (shipRoom2.survCamera)
				{
					shipRoom2.survCamera.Image.Play(shipRoom2.survCamera.OnAnim, 1f);
				}
			}
		}
		return false;
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x00007BB0 File Offset: 0x00005DB0
	public void RepairDamage(PlayerControl player, byte amount)
	{
		if (amount == 1)
		{
			this.InUse += 1;
			return;
		}
		this.InUse -= 1;
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x00007BD5 File Offset: 0x00005DD5
	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.Write(this.InUse);
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x00007BE3 File Offset: 0x00005DE3
	public void Deserialize(MessageReader reader, bool initialState)
	{
		this.InUse = reader.ReadByte();
	}

	// Token: 0x040008F9 RID: 2297
	public const byte IncrementOp = 1;

	// Token: 0x040008FA RID: 2298
	public const byte DecrementOp = 2;

	// Token: 0x040008FB RID: 2299
	public byte InUse;

	// Token: 0x040008FC RID: 2300
	private bool CamsOn;
}
