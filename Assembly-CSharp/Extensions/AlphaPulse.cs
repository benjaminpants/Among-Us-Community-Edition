using System;
using UnityEngine;

public class AlphaPulse : MonoBehaviour
{
	public float Offset = 1f;

	public float Duration = 2.5f;

	private SpriteRenderer rend;

	public FloatRange AlphaRange = new FloatRange(0.2f, 0.5f);

	public Color baseColor = Color.white;

	public void SetColor(Color c)
	{
		rend = GetComponent<SpriteRenderer>();
		baseColor = c;
		Update();
	}

	private void Start()
	{
		rend = GetComponent<SpriteRenderer>();
	}

	public void Update()
	{
		float v = Mathf.Abs(Mathf.Cos((Offset + Time.time) * (float)Math.PI / Duration));
		rend.color = new Color(baseColor.r, baseColor.g, baseColor.b, AlphaRange.Lerp(v));
	}
}
