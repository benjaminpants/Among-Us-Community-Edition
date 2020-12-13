using UnityEngine;

public class HorizontalGauge : MonoBehaviour
{
	public float Value = 0.5f;

	public float MaxValue = 1f;

	public float maskScale = 1f;

	public SpriteMask Mask;

	private float lastValue = float.MinValue;

	public void Update()
	{
		if (MaxValue != 0f && lastValue != Value)
		{
			lastValue = Value;
			float num = lastValue / MaxValue * maskScale;
			Mask.transform.localScale = new Vector3(num, 1f, 1f);
			Mask.transform.localPosition = new Vector3((0f - Mask.sprite.bounds.size.x) * (maskScale - num) / 2f, 0f, 0f);
		}
	}
}
