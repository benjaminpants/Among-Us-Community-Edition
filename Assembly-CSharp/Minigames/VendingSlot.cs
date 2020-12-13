using System.Collections;
using UnityEngine;

public class VendingSlot : MonoBehaviour
{
	public SpriteRenderer DrinkImage;

	public SpriteRenderer GlassImage;

	public IEnumerator CoBuy()
	{
		yield return new WaitForLerp(0.75f, delegate(float v)
		{
			GlassImage.size = new Vector2(1f, Mathf.Lerp(1.7f, 0f, v));
			GlassImage.transform.localPosition = new Vector3(0f, Mathf.Lerp(0f, 0.85f, v), -1f);
		});
		yield return Effects.Shake(DrinkImage.transform, 0.75f, 0.075f);
		Vector3 localPosition = DrinkImage.transform.localPosition;
		localPosition.z = -5f;
		DrinkImage.transform.localPosition = localPosition;
		Vector3 v2 = localPosition;
		v2.y = -8f - localPosition.y;
		yield return Effects.All(Effects.Slide2D(DrinkImage.transform, localPosition, v2), Effects.Rotate2D(DrinkImage.transform, 0f, 0f - FloatRange.Next(-45f, 45f)));
		yield return new WaitForLerp(0.75f, delegate(float v)
		{
			GlassImage.size = new Vector2(1f, Mathf.Lerp(0f, 1.7f, v));
			GlassImage.transform.localPosition = new Vector3(0f, Mathf.Lerp(0.85f, 0f, v), -1f);
		});
		DrinkImage.enabled = false;
	}
}
