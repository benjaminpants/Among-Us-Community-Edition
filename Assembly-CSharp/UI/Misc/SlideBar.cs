using UnityEngine;
using UnityEngine.Events;

public class SlideBar : MonoBehaviour
{
	public TextRenderer Title;

	public SpriteRenderer Bar;

	public Collider2D HitBox;

	public SpriteRenderer Dot;

	public FloatRange Range;

	public bool Vertical;

	public float Value;

	public UnityEvent OnValueChange;

	public void OnEnable()
	{
		if ((bool)Title)
		{
			Title.Color = Color.white;
		}
		Bar.color = Color.white;
		Dot.color = Color.white;
	}

	public void OnDisable()
	{
		if ((bool)Title)
		{
			Title.Color = Color.gray;
		}
		Bar.color = Color.gray;
		Dot.color = Color.gray;
	}

	public void Update()
	{
		Vector3 localPosition = Dot.transform.localPosition;
		switch (DestroyableSingleton<PassiveButtonManager>.Instance.Controller.CheckDrag(HitBox))
		{
		case DragState.Dragging:
		{
			Vector2 vector = DestroyableSingleton<PassiveButtonManager>.Instance.Controller.DragPosition - (Vector2)Bar.transform.position;
			if (Vertical)
			{
				localPosition.y = Range.Clamp(vector.y);
				Value = Range.ReverseLerp(localPosition.y);
			}
			else
			{
				localPosition.x = Range.Clamp(vector.x);
				Value = Range.ReverseLerp(localPosition.x);
			}
			OnValueChange.Invoke();
			break;
		}
		case DragState.Released:
			OnValueChange.Invoke();
			break;
		}
		if (Vertical)
		{
			localPosition.y = Range.Lerp(Value);
		}
		else
		{
			localPosition.x = Range.Lerp(Value);
		}
		Dot.transform.localPosition = localPosition;
	}
}
