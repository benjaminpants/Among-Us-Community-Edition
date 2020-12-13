using PowerTools;
using UnityEngine;

public class SurvCamera : MonoBehaviour
{
	public SpriteAnim Image;

	public float CamSize = 3f;

	public float CamAspect = 1f;

	public Vector3 Offset;

	public AnimationClip OnAnim;

	public AnimationClip OffAnim;

	public void Start()
	{
		Image = GetComponent<SpriteAnim>();
	}

	public void SetAnimation(bool on)
	{
		Image.Play(on ? OnAnim : OffAnim);
	}
}
