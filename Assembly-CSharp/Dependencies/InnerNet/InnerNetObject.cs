using System;
using Hazel;
using UnityEngine;

namespace InnerNet
{
	public abstract class InnerNetObject : MonoBehaviour, IComparable<InnerNetObject>
	{
		public uint SpawnId;

		public uint NetId;

		public uint DirtyBits;

		public SpawnFlags SpawnFlags;

		public SendOption sendMode = SendOption.Reliable;

		public int OwnerId;

		protected bool DespawnOnDestroy = true;

		public bool AmOwner => OwnerId == AmongUsClient.Instance.ClientId;

		public void Despawn()
		{
			UnityEngine.Object.Destroy(base.gameObject);
			AmongUsClient.Instance.Despawn(this);
		}

		public virtual void OnDestroy()
		{
			if ((bool)AmongUsClient.Instance && NetId != uint.MaxValue)
			{
				if (DespawnOnDestroy && AmOwner)
				{
					AmongUsClient.Instance.Despawn(this);
				}
				else
				{
					AmongUsClient.Instance.RemoveNetObject(this);
				}
			}
		}

		public abstract void HandleRpc(byte callId, MessageReader reader);

		public abstract bool Serialize(MessageWriter writer, bool initialState);

		public abstract void Deserialize(MessageReader reader, bool initialState);

		public int CompareTo(InnerNetObject other)
		{
			if (NetId > other.NetId)
			{
				return 1;
			}
			if (NetId < other.NetId)
			{
				return -1;
			}
			return 0;
		}

		protected void SetDirtyBit(uint val)
		{
			DirtyBits |= val;
		}
	}
}
