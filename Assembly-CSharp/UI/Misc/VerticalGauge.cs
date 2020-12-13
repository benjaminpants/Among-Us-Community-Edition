using UnityEngine;

public class VerticalGauge : MonoBehaviour
{
	public float value = 0.5f;

	public float MaxValue = 1f;

	public float maskScale = 1f;

	public SpriteMask Mask;

	private float lastValue = float.MinValue;

	public void Update()
	{
		if (lastValue != value)
		{
			lastValue = value;
			float num = Mathf.Clamp(lastValue / MaxValue, 0f, 1f) * maskScale;
			Vector3 localScale = Mask.transform.localScale;
			localScale.y = num;
			Mask.transform.localScale = localScale;
			Mask.transform.localPosition = new Vector3(0f, (0f - Mask.sprite.bounds.size.y) * (maskScale - num) / 2f, 0f);
		}
	}
}
