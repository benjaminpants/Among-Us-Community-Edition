using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
	public float timer;

	public float frameTime = 0.2f;

	public SpriteRenderer circle;

	public SpriteRenderer middle1;

	public SpriteRenderer middle2;

	public SpriteRenderer outer1;

	public SpriteRenderer outer2;

	public void Update()
	{
		timer += Time.deltaTime;
		if (timer < frameTime)
		{
			circle.color = Color.white;
			SpriteRenderer spriteRenderer = middle1;
			SpriteRenderer spriteRenderer2 = middle2;
			SpriteRenderer spriteRenderer3 = outer1;
			Color color = (outer2.color = Color.black);
			Color color3 = (spriteRenderer3.color = color);
			Color color6 = (spriteRenderer.color = (spriteRenderer2.color = color3));
		}
		else if (timer < 2f * frameTime)
		{
			Color color6 = (middle1.color = (middle2.color = Color.white));
			SpriteRenderer spriteRenderer4 = circle;
			SpriteRenderer spriteRenderer5 = outer1;
			Color color3 = (outer2.color = Color.black);
			color6 = (spriteRenderer4.color = (spriteRenderer5.color = color3));
		}
		else if (timer < 3f * frameTime)
		{
			Color color6 = (outer1.color = (outer2.color = Color.white));
			SpriteRenderer spriteRenderer6 = middle1;
			SpriteRenderer spriteRenderer7 = middle2;
			Color color3 = (circle.color = Color.black);
			color6 = (spriteRenderer6.color = (spriteRenderer7.color = color3));
		}
		else
		{
			timer = 0f;
		}
	}
}
