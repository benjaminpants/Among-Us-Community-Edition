using UnityEngine;

public class PlayerParticle : PoolableBehavior
{
	public SpriteRenderer myRend;

	public float maxDistance = 6f;

	public Vector2 velocity;

	public float angularVelocity;

	private Vector3 lastCamera;

	public Camera FollowCamera
	{
		get;
		set;
	}

	public void Update()
	{
		Vector3 localPosition = base.transform.localPosition;
		float sqrMagnitude = localPosition.sqrMagnitude;
		if ((bool)FollowCamera)
		{
			Vector3 position = FollowCamera.transform.position;
			position.z = 0f;
			localPosition += (position - lastCamera) * (1f - base.transform.localScale.x);
			lastCamera = position;
			sqrMagnitude = ((Vector2)(localPosition - position)).sqrMagnitude;
		}
		if (sqrMagnitude > maxDistance * maxDistance)
		{
			OwnerPool.Reclaim(this);
			return;
		}
		localPosition += (Vector3)(velocity * Time.deltaTime);
		base.transform.localPosition = localPosition;
		base.transform.Rotate(0f, 0f, Time.deltaTime * angularVelocity);
        if (Input.GetKey(KeyCode.UpArrow))
        {
            velocity += new Vector2(0f, 0.1f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            velocity += new Vector2(0f, -0.1f);
        }
		if (Input.GetKey(KeyCode.RightArrow))
		{
			velocity += new Vector2(0.1f, 0f);
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			velocity += new Vector2(-0.1f, 0f);
		}
	}
}
