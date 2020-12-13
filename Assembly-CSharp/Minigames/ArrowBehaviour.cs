using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
	public Vector3 target;

	public float perc = 0.925f;

	private SpriteRenderer image;

	public void Start()
	{
		image = GetComponent<SpriteRenderer>();
	}

	public void Update()
	{
		Camera main = Camera.main;
		Vector2 vector = target - main.transform.position;
		float num = vector.magnitude / (main.orthographicSize * perc);
		image.enabled = (double)num > 0.3;
		Vector2 vector2 = main.WorldToViewportPoint(target);
		if (Between(vector2.x, 0f, 1f) && Between(vector2.y, 0f, 1f))
		{
			base.transform.position = (Vector2)target - vector.normalized * 0.6f;
			float num2 = Mathf.Clamp(num, 0f, 1f);
			base.transform.localScale = new Vector3(num2, num2, num2);
		}
		else
		{
			Vector2 vector3 = new Vector2(Mathf.Clamp(vector2.x * 2f - 1f, -1f, 1f), Mathf.Clamp(vector2.y * 2f - 1f, -1f, 1f));
			float orthographicSize = main.orthographicSize;
			float num3 = main.orthographicSize * main.aspect;
			Vector3 b = new Vector3(Mathf.LerpUnclamped(0f, num3 * 0.88f, vector3.x), Mathf.LerpUnclamped(0f, orthographicSize * 0.79f, vector3.y), 0f);
			base.transform.position = main.transform.position + b;
			base.transform.localScale = Vector3.one;
		}
		base.transform.LookAt2d(target);
	}

	private bool Between(float value, float min, float max)
	{
		if (value > min)
		{
			return value < max;
		}
		return false;
	}
}
