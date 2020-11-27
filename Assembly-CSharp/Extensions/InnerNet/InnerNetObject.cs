using System;
using Hazel;
using UnityEngine;

namespace InnerNet
{
	// Token: 0x0200025A RID: 602
	public abstract class InnerNetObject : MonoBehaviour, IComparable<InnerNetObject>
	{
		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06000D09 RID: 3337 RVA: 0x00009D03 File Offset: 0x00007F03
		public bool AmOwner
		{
			get
			{
				return this.OwnerId == AmongUsClient.Instance.ClientId;
			}
		}

		// Token: 0x06000D0A RID: 3338 RVA: 0x00009D17 File Offset: 0x00007F17
		public void Despawn()
		{
			UnityEngine.Object.Destroy(base.gameObject);
			AmongUsClient.Instance.Despawn(this);
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x00009D2F File Offset: 0x00007F2F
		public virtual void OnDestroy()
		{
			if (AmongUsClient.Instance && this.NetId != 4294967295U)
			{
				if (this.DespawnOnDestroy && this.AmOwner)
				{
					AmongUsClient.Instance.Despawn(this);
					return;
				}
				AmongUsClient.Instance.RemoveNetObject(this);
			}
		}

		// Token: 0x06000D0C RID: 3340
		public abstract void HandleRpc(byte callId, MessageReader reader);

		// Token: 0x06000D0D RID: 3341
		public abstract bool Serialize(MessageWriter writer, bool initialState);

		// Token: 0x06000D0E RID: 3342
		public abstract void Deserialize(MessageReader reader, bool initialState);

		// Token: 0x06000D0F RID: 3343 RVA: 0x00009D6D File Offset: 0x00007F6D
		public int CompareTo(InnerNetObject other)
		{
			if (this.NetId > other.NetId)
			{
				return 1;
			}
			if (this.NetId < other.NetId)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x06000D10 RID: 3344 RVA: 0x00009D90 File Offset: 0x00007F90
		protected void SetDirtyBit(uint val)
		{
			this.DirtyBits |= val;
		}

		// Token: 0x04000C78 RID: 3192
		public uint SpawnId;

		// Token: 0x04000C79 RID: 3193
		public uint NetId;

		// Token: 0x04000C7A RID: 3194
		public uint DirtyBits;

		// Token: 0x04000C7B RID: 3195
		public SpawnFlags SpawnFlags;

		// Token: 0x04000C7C RID: 3196
		public SendOption sendMode = SendOption.Reliable;

		// Token: 0x04000C7D RID: 3197
		public int OwnerId;

		// Token: 0x04000C7E RID: 3198
		protected bool DespawnOnDestroy = true;
	}
}
