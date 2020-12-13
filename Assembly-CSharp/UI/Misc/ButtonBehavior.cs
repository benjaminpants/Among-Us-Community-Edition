using UnityEngine;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour
{
	public bool OnUp = true;

	public bool OnDown;

	public bool Repeat;

	public Button.ButtonClickedEvent OnClick = new Button.ButtonClickedEvent();

	private Controller myController = new Controller();

	private Collider2D[] colliders;

	private float downTime;

	public void OnEnable()
	{
		colliders = GetComponents<Collider2D>();
		myController.Reset();
	}

	public void Update()
	{
		myController.Update();
		Collider2D[] array = colliders;
		foreach (Collider2D coll in array)
		{
			switch (myController.CheckDrag(coll))
			{
			case DragState.TouchStart:
				if (OnDown)
				{
					OnClick.Invoke();
				}
				break;
			case DragState.Released:
				if (OnUp)
				{
					OnClick.Invoke();
				}
				break;
			case DragState.Dragging:
				if (Repeat)
				{
					downTime += Time.fixedDeltaTime;
					if (downTime >= 0.3f)
					{
						downTime = 0f;
						OnClick.Invoke();
					}
				}
				else
				{
					downTime = 0f;
				}
				break;
			}
		}
	}
}
