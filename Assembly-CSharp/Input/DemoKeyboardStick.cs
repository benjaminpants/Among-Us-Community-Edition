using UnityEngine;

public class DemoKeyboardStick : VirtualJoystick
{
	public SpriteRenderer UpKey;

	public SpriteRenderer DownKey;

	public SpriteRenderer LeftKey;

	public SpriteRenderer RightKey;

	protected override void FixedUpdate()
	{
	}

	public override void UpdateJoystick(FingerBehaviour finger, Vector2 velocity, bool syncFinger)
	{
		UpKey.enabled = velocity.y > 0.1f;
		DownKey.enabled = velocity.y < -0.1f;
		RightKey.enabled = velocity.x > 0.1f;
		LeftKey.enabled = velocity.x < -0.1f;
	}
}
