using UnityEngine;

public class VirtualJoystick : MonoBehaviour, IVirtualJoystick
{
	public float InnerRadius = 0.64f;

	public float OuterRadius = 1.28f;

	public CircleCollider2D Outer;

	public SpriteRenderer Inner;

	private Controller myController = new Controller();

	public Vector2 Delta
	{
		get;
		private set;
	}

	protected virtual void FixedUpdate()
	{
		myController.Update();
		switch (myController.CheckDrag(Outer))
		{
		case DragState.TouchStart:
		case DragState.Dragging:
		{
			float maxLength = OuterRadius - InnerRadius;
			Vector2 v = myController.DragPosition - (Vector2)base.transform.position;
			_ = v.magnitude;
			Vector2 a = new Vector2(Mathf.Sqrt(Mathf.Abs(v.x)) * Mathf.Sign(v.x), Mathf.Sqrt(Mathf.Abs(v.y)) * Mathf.Sign(v.y));
			Delta = Vector2.ClampMagnitude(a / OuterRadius, 1f);
			Inner.transform.localPosition = Vector3.ClampMagnitude(v, maxLength) + Vector3.back;
			break;
		}
		case DragState.Released:
			Delta = Vector2.zero;
			Inner.transform.localPosition = Vector3.back;
			break;
		}
	}

	public virtual void UpdateJoystick(FingerBehaviour finger, Vector2 velocity, bool syncFinger)
	{
		Vector3 vector = Inner.transform.localPosition;
		Vector3 vector2 = velocity.normalized * InnerRadius;
		vector2.z = vector.z;
		if (syncFinger)
		{
			vector = Vector3.Lerp(vector, vector2, Time.fixedDeltaTime * 5f);
			Inner.transform.localPosition = vector;
			vector = Inner.transform.position;
			vector.z = -26f;
			finger.transform.position = vector;
		}
		else if (Inner.gameObject != finger.gameObject)
		{
			Inner.transform.localPosition = vector2;
		}
	}
}
