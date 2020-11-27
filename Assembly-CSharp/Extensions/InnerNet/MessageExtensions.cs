using System;
using Hazel;

namespace InnerNet
{
	// Token: 0x02000262 RID: 610
	public static class MessageExtensions
	{
		// Token: 0x06000D2F RID: 3375 RVA: 0x00009E99 File Offset: 0x00008099
		public static void WriteNetObject(this MessageWriter self, InnerNetObject obj)
		{
			if (!obj)
			{
				self.Write(0);
				return;
			}
			self.WritePacked(obj.NetId);
		}

		// Token: 0x06000D30 RID: 3376 RVA: 0x0003F2F8 File Offset: 0x0003D4F8
		public static T ReadNetObject<T>(this MessageReader self) where T : InnerNetObject
		{
			uint netId = self.ReadPackedUInt32();
			return AmongUsClient.Instance.FindObjectByNetId<T>(netId);
		}
	}
}
