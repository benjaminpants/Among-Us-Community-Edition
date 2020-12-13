using System;
using UnityEngine;

public class TumbleBoxBehaviour : MonoBehaviour
{
	public FloatRange BoxHeight;

	public FloatRange shadowScale;

	public SpriteRenderer Shadow;

	public SpriteRenderer Box;

	public void FixedUpdate()
	{
		float z = Time.time * 15f;
		float v = Mathf.Cos(Time.time * (float)Math.PI / 10f) / 2f + 0.5f;
		float num = shadowScale.Lerp(v);
		Shadow.transform.localScale = new Vector3(num, num, num);
		float y = BoxHeight.Lerp(v);
		Box.transform.localPosition = new Vector3(0f, y, -0.01f);
		Box.transform.eulerAngles = new Vector3(0f, 0f, z);
	}
}
