using UnityEngine;

public class LeafBehaviour : MonoBehaviour
{
	public Sprite[] Images;

	public FloatRange SpinSpeed = new FloatRange(-45f, 45f);

	public Vector2Range StartVel;

	public float AccelRate = 30f;

	[HideInInspector]
	public LeafMinigame Parent;

	public bool Held;

	private static RandomFill<Sprite> ImageFiller = new RandomFill<Sprite>();

	[HideInInspector]
	public Rigidbody2D body;

	public void Start()
	{
		ImageFiller.Set(Images);
		GetComponent<SpriteRenderer>().sprite = ImageFiller.Get();
		body = GetComponent<Rigidbody2D>();
		body.angularVelocity = SpinSpeed.Next();
		body.velocity = StartVel.Next();
	}

	public void FixedUpdate()
	{
		if (!Held && (double)base.transform.localPosition.x < -2.5)
		{
			Parent.LeafDone(this);
		}
	}
}
