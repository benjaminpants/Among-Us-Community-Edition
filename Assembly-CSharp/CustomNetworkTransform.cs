using System;
using Hazel;
using InnerNet;
using UnityEngine;

// Token: 0x02000113 RID: 275
[DisallowMultipleComponent]
public class CustomNetworkTransform : InnerNetObject
{
	// Token: 0x060005D3 RID: 1491 RVA: 0x000246E4 File Offset: 0x000228E4
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.targetSyncPosition = (this.prevPosSent = base.transform.position);
		this.targetSyncVelocity = (this.prevVelSent = Vector2.zero);
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x00005AF8 File Offset: 0x00003CF8
	public void OnEnable()
	{
		base.SetDirtyBit(1U);
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x00024730 File Offset: 0x00022930
	public void Halt()
	{
		ushort minSid = (ushort)(this.lastSequenceId + 1);
		this.SnapTo(base.transform.position, minSid);
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x00024760 File Offset: 0x00022960
	public void RpcSnapTo(Vector2 position)
	{
		ushort minSid = (ushort)(this.lastSequenceId + 5);
		if (AmongUsClient.Instance.AmClient)
		{
			this.SnapTo(position, minSid);
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 0, SendOption.Reliable);
		this.WriteVector2(position, messageWriter);
		messageWriter.Write(this.lastSequenceId);
		messageWriter.EndMessage();
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x000247B8 File Offset: 0x000229B8
	public void SnapTo(Vector2 position)
	{
		ushort minSid = (ushort)(this.lastSequenceId + 1);
		this.SnapTo(position, minSid);
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x000247D8 File Offset: 0x000229D8
	private void SnapTo(Vector2 position, ushort minSid)
	{
		if (!CustomNetworkTransform.SidGreaterThan(minSid, this.lastSequenceId))
		{
			return;
		}
		this.lastSequenceId = minSid;
		Transform transform = base.transform;
		this.body.position = position;
		this.targetSyncPosition = position;
		transform.position = position;
		this.targetSyncVelocity = (this.body.velocity = Vector2.zero);
		this.prevPosSent = position;
		this.prevVelSent = Vector2.zero;
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x00024850 File Offset: 0x00022A50
	private void FixedUpdate()
	{
		if (base.AmOwner)
		{
			if (this.HasMoved())
			{
				base.SetDirtyBit(1U);
				return;
			}
		}
		else
		{
			if (this.interpolateMovement != 0f)
			{
				Vector2 vector = this.targetSyncPosition - this.body.position;
				if (vector.sqrMagnitude >= 0.0001f)
				{
					float num = this.interpolateMovement / this.sendInterval;
					vector.x *= num;
					vector.y *= num;
					if (PlayerControl.LocalPlayer)
					{
						vector = Vector2.ClampMagnitude(vector, PlayerControl.LocalPlayer.MyPhysics.TrueSpeed);
					}
					this.body.velocity = vector;
				}
				else
				{
					this.body.velocity = Vector2.zero;
				}
			}
			this.targetSyncPosition += this.targetSyncVelocity * Time.fixedDeltaTime * 0.1f;
		}
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x00024940 File Offset: 0x00022B40
	private bool HasMoved()
	{
		float num;
		if (this.body != null)
		{
			num = Vector2.Distance(this.body.position, this.prevPosSent);
		}
		else
		{
			num = Vector2.Distance(base.transform.position, this.prevPosSent);
		}
		if (num > 0.0001f)
		{
			return true;
		}
		if (this.body != null)
		{
			num = Vector2.Distance(this.body.velocity, this.prevVelSent);
		}
		return num > 0.0001f;
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x000249D0 File Offset: 0x00022BD0
	public override void HandleRpc(byte callId, MessageReader reader)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (callId == 0)
		{
			Vector2 position = this.ReadVector2(reader);
			ushort minSid = reader.ReadUInt16();
			this.SnapTo(position, minSid);
		}
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x00024A04 File Offset: 0x00022C04
	public override bool Serialize(MessageWriter writer, bool initialState)
	{
		if (initialState)
		{
			writer.Write(this.lastSequenceId);
			this.WriteVector2(this.body.position, writer);
			this.WriteVector2(this.body.velocity, writer);
			return true;
		}
		if (this.DirtyBits == 0U)
		{
			return false;
		}
		if (!base.isActiveAndEnabled)
		{
			return false;
		}
		this.lastSequenceId += 1;
		writer.Write(this.lastSequenceId);
		this.WriteVector2(this.body.position, writer);
		this.WriteVector2(this.body.velocity, writer);
		this.prevPosSent = this.body.position;
		this.prevVelSent = this.body.velocity;
		this.DirtyBits = 0U;
		return true;
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00024AC4 File Offset: 0x00022CC4
	public override void Deserialize(MessageReader reader, bool initialState)
	{
		if (initialState)
		{
			this.lastSequenceId = reader.ReadUInt16();
			this.targetSyncPosition = (base.transform.position = this.ReadVector2(reader));
			this.targetSyncVelocity = this.ReadVector2(reader);
			return;
		}
		ushort newSid = reader.ReadUInt16();
		if (!CustomNetworkTransform.SidGreaterThan(newSid, this.lastSequenceId))
		{
			return;
		}
		this.lastSequenceId = newSid;
		this.targetSyncPosition = this.ReadVector2(reader);
		this.targetSyncVelocity = this.ReadVector2(reader);
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (Vector2.Distance(this.body.position, this.targetSyncPosition) > this.snapThreshold)
		{
			if (this.body)
			{
				this.body.position = this.targetSyncPosition;
				this.body.velocity = this.targetSyncVelocity;
			}
			else
			{
				base.transform.position = this.targetSyncPosition;
			}
		}
		if (this.interpolateMovement == 0f && this.body)
		{
			this.body.position = this.targetSyncPosition;
		}
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00024BE4 File Offset: 0x00022DE4
	private static bool SidGreaterThan(ushort newSid, ushort prevSid)
	{
		ushort num = (ushort)(prevSid + 32767);
		if (prevSid < num)
		{
			return newSid > prevSid && newSid <= num;
		}
		return newSid > prevSid || newSid <= num;
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00024C1C File Offset: 0x00022E1C
	private void WriteVector2(Vector2 vec, MessageWriter writer)
	{
		ushort value = (ushort)(this.XRange.ReverseLerp(vec.x) * 65535f);
		ushort value2 = (ushort)(this.YRange.ReverseLerp(vec.y) * 65535f);
		writer.Write(value);
		writer.Write(value2);
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00024C6C File Offset: 0x00022E6C
	private Vector2 ReadVector2(MessageReader reader)
	{
		float v = (float)reader.ReadUInt16() / 65535f;
		float v2 = (float)reader.ReadUInt16() / 65535f;
		return new Vector2(this.XRange.Lerp(v), this.YRange.Lerp(v2));
	}

	// Token: 0x040005B2 RID: 1458
	private const float LocalMovementThreshold = 0.0001f;

	// Token: 0x040005B3 RID: 1459
	private const float LocalVelocityThreshold = 0.0001f;

	// Token: 0x040005B4 RID: 1460
	private const float MoveAheadRatio = 0.1f;

	// Token: 0x040005B5 RID: 1461
	private readonly FloatRange XRange = new FloatRange(-40f, 40f);

	// Token: 0x040005B6 RID: 1462
	private readonly FloatRange YRange = new FloatRange(-40f, 40f);

	// Token: 0x040005B7 RID: 1463
	[SerializeField]
	private float sendInterval = 0.1f;

	// Token: 0x040005B8 RID: 1464
	[SerializeField]
	private float snapThreshold = 5f;

	// Token: 0x040005B9 RID: 1465
	[SerializeField]
	private float interpolateMovement = 1f;

	// Token: 0x040005BA RID: 1466
	private Rigidbody2D body;

	// Token: 0x040005BB RID: 1467
	private Vector2 targetSyncPosition;

	// Token: 0x040005BC RID: 1468
	private Vector2 targetSyncVelocity;

	// Token: 0x040005BD RID: 1469
	private ushort lastSequenceId;

	// Token: 0x040005BE RID: 1470
	private Vector2 prevPosSent;

	// Token: 0x040005BF RID: 1471
	private Vector2 prevVelSent;

	// Token: 0x02000114 RID: 276
	private enum RpcCalls
	{
		// Token: 0x040005C1 RID: 1473
		SnapTo
	}
}
