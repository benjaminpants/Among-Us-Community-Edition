using Hazel;
using InnerNet;
using UnityEngine;

[DisallowMultipleComponent]
public class CustomNetworkTransform : InnerNetObject
{
	private enum RpcCalls
	{
		SnapTo
	}

	private const float LocalMovementThreshold = 0.0001f;

	private const float LocalVelocityThreshold = 0.0001f;

	private const float MoveAheadRatio = 0.1f;

	private readonly FloatRange XRange = new FloatRange(-40f, 40f);

	private readonly FloatRange YRange = new FloatRange(-40f, 40f);

	[SerializeField]
	private float sendInterval = 0.1f;

	[SerializeField]
	private float snapThreshold = 5f;

	[SerializeField]
	private float interpolateMovement = 1f;

	private Rigidbody2D body;

	private Vector2 targetSyncPosition;

	private Vector2 targetSyncVelocity;

	private ushort lastSequenceId;

	private Vector2 prevPosSent;

	private Vector2 prevVelSent;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		targetSyncPosition = (prevPosSent = base.transform.position);
		targetSyncVelocity = (prevVelSent = Vector2.zero);
	}

	public void OnEnable()
	{
		SetDirtyBit(1u);
	}

    public void Halt()
    {
		RpcHalt();
    }

	public void ClientHalt()
	{
		ushort minSid = (ushort)(lastSequenceId + 1);
		targetSyncVelocity = new Vector2(0, 0);
		SnapTo(base.transform.position, minSid);
	}

	public void RpcSnapTo(Vector2 position)
    {
        ushort minSid = (ushort)(lastSequenceId + 5);
        if (AmongUsClient.Instance.AmClient)
        {
            SnapTo(position, minSid);
        }
        MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 0);
        WriteVector2(position, messageWriter);
        messageWriter.Write(lastSequenceId);
        messageWriter.EndMessage();
    }

	public void RpcHalt()
	{
		if (AmongUsClient.Instance.AmClient)
		{
			ClientHalt();
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 1);
		messageWriter.EndMessage();
	}

	public void SnapTo(Vector2 position)
	{
		ushort minSid = (ushort)(lastSequenceId + 1);
		SnapTo(position, minSid);
	}

	private void SnapTo(Vector2 position, ushort minSid)
	{
		if (SidGreaterThan(minSid, lastSequenceId))
		{
			lastSequenceId = minSid;
			Transform transform = base.transform;
			Vector2 vector2 = (body.position = position);
			transform.position = (targetSyncPosition = vector2);
			body.velocity = Vector2.zero;
			targetSyncVelocity = Vector2.zero;
			prevPosSent = position;
			prevVelSent = Vector2.zero;
		}
	}

	private void FixedUpdate()
	{
		if (base.AmOwner)
		{
			if (HasMoved())
			{
				SetDirtyBit(1u);
			}
			return;
		}
		if (interpolateMovement != 0f)
		{
			Vector2 vector = targetSyncPosition - body.position;
			if (vector.sqrMagnitude >= 0.0001f)
			{
				float num = interpolateMovement / sendInterval;
				vector.x *= num;
				vector.y *= num;
				if ((bool)PlayerControl.LocalPlayer && PlayerControl.GameOptions != null)
				{
					vector = Vector2.ClampMagnitude(vector, (vector.magnitude > ((PlayerControl.LocalPlayer.MyPhysics.TrueSpeed * PlayerControl.GameOptions.PlayerSpeedMod) * Mathf.Clamp(PlayerControl.GameOptions.SprintMultipler,1f,3f)) ? PlayerControl.LocalPlayer.MyPhysics.TrueSpeed * PlayerControl.GameOptions.SprintMultipler : PlayerControl.LocalPlayer.MyPhysics.TrueSpeed));
				}
				body.velocity = vector;
			}
			else
			{
				body.velocity = Vector2.zero;
			}
		}
		targetSyncPosition += targetSyncVelocity * Time.fixedDeltaTime * 0.1f;
	}

	private bool HasMoved()
	{
		float num = 0f;
		num = ((!(body != null)) ? Vector2.Distance(base.transform.position, prevPosSent) : Vector2.Distance(body.position, prevPosSent));
		if (num > 0.0001f)
		{
			return true;
		}
		if (body != null)
		{
			num = Vector2.Distance(body.velocity, prevVelSent);
		}
		if (num > 0.0001f)
		{
			return true;
		}
		return false;
	}

	public override void HandleRpc(byte callId, MessageReader reader)
	{
        if (base.isActiveAndEnabled && callId == 0)
        {
            Vector2 position = ReadVector2(reader);
            ushort minSid = reader.ReadUInt16();
            SnapTo(position, minSid);
        }
		if (base.isActiveAndEnabled && callId == 1) //clientside halt
		{
			ClientHalt();
		}
	}

	public override bool Serialize(MessageWriter writer, bool initialState)
	{
		if (initialState)
		{
			writer.Write(lastSequenceId);
			WriteVector2(body.position, writer);
			WriteVector2(body.velocity, writer);
			return true;
		}
		if (DirtyBits == 0)
		{
			return false;
		}
		if (!base.isActiveAndEnabled)
		{
			return false;
		}
		lastSequenceId++;
		writer.Write(lastSequenceId);
		WriteVector2(body.position, writer);
		WriteVector2(body.velocity, writer);
		prevPosSent = body.position;
		prevVelSent = body.velocity;
		DirtyBits = 0u;
		return true;
	}

	public override void Deserialize(MessageReader reader, bool initialState)
	{
		if (initialState)
		{
			lastSequenceId = reader.ReadUInt16();
			Vector3 v = (base.transform.position = ReadVector2(reader));
			targetSyncPosition = v;
			targetSyncVelocity = ReadVector2(reader);
			return;
		}
		ushort newSid = reader.ReadUInt16();
		if (!SidGreaterThan(newSid, lastSequenceId))
		{
			return;
		}
		lastSequenceId = newSid;
		targetSyncPosition = ReadVector2(reader);
		targetSyncVelocity = ReadVector2(reader);
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (Vector2.Distance(body.position, targetSyncPosition) > snapThreshold)
		{
			if ((bool)body)
			{
				body.position = targetSyncPosition;
				body.velocity = targetSyncVelocity;
			}
			else
			{
				base.transform.position = targetSyncPosition;
			}
		}
		if (interpolateMovement == 0f && (bool)body)
		{
			body.position = targetSyncPosition;
		}
	}

	private static bool SidGreaterThan(ushort newSid, ushort prevSid)
	{
		ushort num = (ushort)(prevSid + 32767);
		if (prevSid < num)
		{
			if (newSid > prevSid)
			{
				return newSid <= num;
			}
			return false;
		}
		if (newSid <= prevSid)
		{
			return newSid <= num;
		}
		return true;
	}

	private void WriteVector2(Vector2 vec, MessageWriter writer)
	{
		ushort value = (ushort)(XRange.ReverseLerp(vec.x) * 65535f);
		ushort value2 = (ushort)(YRange.ReverseLerp(vec.y) * 65535f);
		writer.Write(value);
		writer.Write(value2);
	}

	private Vector2 ReadVector2(MessageReader reader)
	{
		float v = (float)(int)reader.ReadUInt16() / 65535f;
		float v2 = (float)(int)reader.ReadUInt16() / 65535f;
		return new Vector2(XRange.Lerp(v), YRange.Lerp(v2));
	}
}
