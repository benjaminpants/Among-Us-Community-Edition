using UnityEngine;

public class TextLink : MonoBehaviour
{
	public BoxCollider2D boxCollider;

	public string targetUrl;

	public bool needed;

	public void Set(Vector2 from, Vector2 to, string target)
	{
		targetUrl = target;
		Vector2 size = to + from;
		base.transform.localPosition = new Vector3(size.x / 2f, size.y / 2f, -1f);
		size = to - from;
		size.y = 0f - size.y;
		boxCollider.size = size;
	}

	public void Click()
	{
		Application.OpenURL(targetUrl);
	}
}
