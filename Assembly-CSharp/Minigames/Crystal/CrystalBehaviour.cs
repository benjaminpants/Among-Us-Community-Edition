using System.Collections;
using UnityEngine;

public class CrystalBehaviour : MonoBehaviour
{
	public enum Parentness
	{
		None,
		Above,
		Below
	}

	public CrystalBehaviour Above;

	public CrystalBehaviour Below;

	public Parentness ParentSide;

	public SpriteRenderer Renderer;

	public BoxCollider2D Collider;

	public FloatRange Padding;

	private const float Speed = 15f;

	private const float FloatMag = 0.05f;

	private const float FloatSpeed = 0.35f;

	private void Update()
	{
		Vector3 position = base.transform.position;
		Vector3 vector;
		if (ParentSide == Parentness.Above)
		{
			if (!Above)
			{
				return;
			}
			vector = Above.transform.position - new Vector3(0f, Above.Padding.min + Padding.max, 0f);
		}
		else
		{
			if (ParentSide != Parentness.Below || !Below)
			{
				return;
			}
			vector = Below.transform.position + new Vector3(0f, Below.Padding.max + Padding.min, 0f);
		}
		float num = Time.time / 0.35f;
		vector.x += (Mathf.PerlinNoise(num, position.z * 20f) * 2f - 1f) * 0.05f;
		vector.y += (Mathf.PerlinNoise(position.z * 20f, num) * 2f - 1f) * 0.05f;
		vector.z = position.z;
		position.x = Mathf.SmoothStep(position.x, vector.x, Time.deltaTime * 15f);
		position.y = Mathf.SmoothStep(position.y, vector.y, Time.deltaTime * 15f);
		base.transform.position = position;
	}

	public void FlashUp(float delay = 0f)
	{
		StopAllCoroutines();
		StartCoroutine(Flash(this, delay));
		if ((bool)Above)
		{
			Above.FlashUp(delay + 0.1f);
		}
	}

	public void FlashDown(float delay = 0f)
	{
		StopAllCoroutines();
		StartCoroutine(Flash(this, delay));
		if ((bool)Below)
		{
			Below.FlashDown(delay + 0.1f);
		}
	}

	private static IEnumerator Flash(CrystalBehaviour c, float delay)
	{
		for (float time = 0f; time < delay; time += Time.deltaTime)
		{
			yield return null;
		}
		Color col = Color.clear;
		for (float time = 0f; time < 0.1f; time += Time.deltaTime)
		{
			float t = time / 0.1f;
			col.r = (col.g = (col.b = Mathf.Lerp(0f, 1f, t)));
			c.Renderer.material.SetColor("_AddColor", col);
			yield return null;
		}
		for (float time = 0f; time < 0.1f; time += Time.deltaTime)
		{
			float t2 = time / 0.1f;
			col.r = (col.g = (col.b = Mathf.Lerp(1f, 0f, t2)));
			c.Renderer.material.SetColor("_AddColor", col);
			yield return null;
		}
		col.r = (col.g = (col.b = 0f));
		c.Renderer.material.SetColor("_AddColor", col);
	}
}
