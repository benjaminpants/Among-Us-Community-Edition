using UnityEngine;

public class WireNode : MonoBehaviour
{
	public Collider2D hitbox;

	public SpriteRenderer[] WireColors;

	public sbyte WireId;

	internal void SetColor(Color color)
	{
		for (int i = 0; i < WireColors.Length; i++)
		{
			WireColors[i].color = color;
		}
	}
}
