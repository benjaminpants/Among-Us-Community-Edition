using UnityEngine;

public class TaskPanelBehaviour : MonoBehaviour
{
	public Vector3 OpenPosition;

	public Vector3 ClosedPosition;

	public SpriteRenderer background;

	public SpriteRenderer tab;

	public TextRenderer TaskText;

	public bool open;

	private float timer;

	public float Duration;

	private void Update()
	{
		background.transform.localScale = new Vector3(TaskText.Width + 0.2f, TaskText.Height + 0.2f, 1f);
		Vector3 vector = background.sprite.bounds.extents;
		vector.y = 0f - vector.y;
		vector = vector.Mul(background.transform.localScale);
		background.transform.localPosition = vector;
		Vector3 vector2 = tab.sprite.bounds.extents;
		vector2 = vector2.Mul(tab.transform.localScale);
		vector2.y = 0f - vector2.y;
		vector2.x += vector.x * 2f;
		tab.transform.localPosition = vector2;
		ClosedPosition.y = (OpenPosition.y = 0.6f);
		ClosedPosition.x = (0f - background.sprite.bounds.size.x) * background.transform.localScale.x;
		if (open)
		{
			timer = Mathf.Min(1f, timer + Time.deltaTime / Duration);
		}
		else
		{
			timer = Mathf.Max(0f, timer - Time.deltaTime / Duration);
		}
		Vector3 relativePos = new Vector3(Mathf.SmoothStep(ClosedPosition.x, OpenPosition.x, timer), Mathf.SmoothStep(ClosedPosition.y, OpenPosition.y, timer), OpenPosition.z);
		base.transform.localPosition = AspectPosition.ComputePosition(AspectPosition.EdgeAlignments.LeftTop, relativePos);
	}

	public void ToggleOpen()
	{
		open = !open;
	}
}
