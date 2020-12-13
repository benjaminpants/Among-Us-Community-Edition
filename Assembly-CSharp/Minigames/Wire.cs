using UnityEngine;

public class Wire : MonoBehaviour
{
	private const int WireDepth = -14;

	public SpriteRenderer Liner;

	public SpriteRenderer ColorBase;

	public SpriteRenderer ColorEnd;

	public Collider2D hitbox;

	public SpriteRenderer WireTip;

	public sbyte WireId;

	public Vector2 BaseWorldPos
	{
		get;
		internal set;
	}

	public void Start()
	{
		BaseWorldPos = base.transform.position;
	}

	public void ResetLine(Vector3 targetWorldPos, bool reset = false)
	{
		if (reset)
		{
			Liner.transform.localScale = new Vector3(0f, 0f, 0f);
			WireTip.transform.eulerAngles = Vector3.zero;
			WireTip.transform.position = base.transform.position;
			return;
		}
		Vector2 vector = targetWorldPos - base.transform.position;
		Vector2 normalized = vector.normalized;
		Vector3 localPosition = default(Vector3);
		localPosition = vector - normalized * 0.075f;
		localPosition.z = -0.01f;
		WireTip.transform.localPosition = localPosition;
		float magnitude = vector.magnitude;
		Liner.transform.localScale = new Vector3(magnitude, 1f, 1f);
		Liner.transform.localPosition = vector / 2f;
		WireTip.transform.LookAt2d(targetWorldPos);
		Liner.transform.localEulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.right, vector));
	}

	public void ConnectRight(WireNode node)
	{
		Vector3 position = node.transform.position;
		ResetLine(position);
	}

	public void SetColor(Color color)
	{
		Liner.material.SetColor("_Color", color);
		ColorBase.color = color;
		ColorEnd.color = color;
	}
}
