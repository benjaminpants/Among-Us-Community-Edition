using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
	public PlayerParticleInfo[] Sprites;

	public FloatRange velocity;

	public FloatRange scale;

	public ObjectPoolBehavior pool;

	public float StartRadius;

	public Camera FollowCamera;

	private int CurrentColor;

	private int MaxColors = Palette.PlayerColors.Length;

	private RandomFill<PlayerParticleInfo> fill;

	public void Start()
	{
		fill = new RandomFill<PlayerParticleInfo>();
		fill.Set(Sprites);
		CurrentColor = Random.Range(0, Palette.PlayerColors.Length - 1);
		while (pool.NotInUse > 0)
		{
			PlayerParticle playerParticle = pool.Get<PlayerParticle>();
			PlacePlayer(playerParticle, initial: true);
		}
	}

	private int GetPlayerColor()
    {
		CurrentColor++;
		if (CurrentColor >= MaxColors) CurrentColor = 0;
		return CurrentColor;
	}

	public void Update()
	{
		while (pool.NotInUse > 0)
		{
			PlayerParticle part = pool.Get<PlayerParticle>();
			PlacePlayer(part, initial: false);
		}
	}

	private void PlacePlayer(PlayerParticle part, bool initial)
	{
		Vector3 localPosition = Random.insideUnitCircle;
		if (!initial)
		{
			localPosition.Normalize();
		}
		PlayerControl.SetPlayerMaterialColors(GetPlayerColor(), part.myRend);
		localPosition *= StartRadius;
		float num = scale.Next();
		part.transform.localScale = new Vector3(num, num, num);
		localPosition.z = (0f - num) * 0.001f;
		if ((bool)FollowCamera)
		{
			Vector3 position = FollowCamera.transform.position;
			position.z = 0f;
			localPosition += position;
			part.FollowCamera = FollowCamera;
		}
		part.transform.localPosition = localPosition;
		PlayerParticleInfo playerParticleInfo = fill.Get();
		part.myRend.sprite = playerParticleInfo.image;
		part.myRend.flipX = BoolRange.Next();
		Vector2 self = -localPosition.normalized;
		self = self.Rotate(FloatRange.Next(-45f, 45f));
		part.velocity = self * velocity.Next();
		part.angularVelocity = playerParticleInfo.angularVel.Next();
		if (playerParticleInfo.alignToVel)
		{
			part.transform.localEulerAngles = new Vector3(0f, 0f, Vector2.up.AngleSigned(self));
		}
	}
}
