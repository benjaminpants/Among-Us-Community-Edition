using UnityEngine;

public static class CooldownHelpers
{
	public static void SetCooldownNormalizedUvs(this SpriteRenderer myRend)
	{
		Vector2[] uv = myRend.sprite.uv;
		Vector4 value = new Vector4(2f, -1f);
		for (int i = 0; i < uv.Length; i++)
		{
			if (value.x > uv[i].y)
			{
				value.x = uv[i].y;
			}
			if (value.y < uv[i].y)
			{
				value.y = uv[i].y;
			}
		}
		value.y -= value.x;
		myRend.material.SetVector("_NormalizedUvs", value);
	}
}
