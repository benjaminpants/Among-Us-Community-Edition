using UnityEngine;

public class Controller
{
	public class TouchState
	{
		public Vector2 DownAt;

		public Vector2 Position;

		public bool WasDown;

		public bool IsDown;

		public bool TouchStart;

		public bool TouchEnd;
	}

	public readonly TouchState[] Touches = new TouchState[2];

	private Collider2D amTouching;

	private int touchId = -1;

	public bool AnyTouch
	{
		get
		{
			if (!Touches[0].IsDown)
			{
				return Touches[1].IsDown;
			}
			return true;
		}
	}

	public bool AnyTouchDown
	{
		get
		{
			if (!Touches[0].TouchStart)
			{
				return Touches[1].TouchStart;
			}
			return true;
		}
	}

	public bool AnyTouchUp
	{
		get
		{
			if (!Touches[0].TouchEnd)
			{
				return Touches[1].TouchEnd;
			}
			return true;
		}
	}

	public bool FirstDown => Touches[0].TouchStart;

	public Vector2 DragPosition => Touches[touchId].Position;

	public Vector2 DragStartPosition => Touches[touchId].DownAt;

	public Controller()
	{
		for (int i = 0; i < Touches.Length; i++)
		{
			Touches[i] = new TouchState();
		}
	}

	public DragState CheckDrag(Collider2D coll, bool firstOnly = false)
	{
		if (!coll)
		{
			return DragState.NoTouch;
		}
		if (touchId > -1)
		{
			if (coll != amTouching)
			{
				return DragState.NoTouch;
			}
			if (Touches[touchId].IsDown)
			{
				return DragState.Dragging;
			}
			amTouching = null;
			touchId = -1;
			return DragState.Released;
		}
		if (firstOnly)
		{
			TouchState touchState = Touches[0];
			if (touchState.TouchStart && coll.OverlapPoint(touchState.Position))
			{
				amTouching = coll;
				touchId = 0;
				return DragState.TouchStart;
			}
		}
		else
		{
			for (int i = 0; i < Touches.Length; i++)
			{
				TouchState touchState2 = Touches[i];
				if (touchState2.TouchStart && coll.OverlapPoint(touchState2.Position))
				{
					amTouching = coll;
					touchId = i;
					return DragState.TouchStart;
				}
			}
		}
		return DragState.NoTouch;
	}

	public void Update()
	{
		TouchState touchState = Touches[0];
		bool mouseButton = Input.GetMouseButton(0);
		touchState.Position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		touchState.TouchStart = !touchState.IsDown && mouseButton;
		if (touchState.TouchStart)
		{
			touchState.DownAt = touchState.Position;
		}
		touchState.TouchEnd = touchState.IsDown && !mouseButton;
		touchState.IsDown = mouseButton;
	}

	public void Reset()
	{
		for (int i = 0; i < Touches.Length; i++)
		{
			Touches[i] = new TouchState();
		}
		touchId = -1;
		amTouching = null;
	}

	public TouchState GetTouch(int i)
	{
		return Touches[i];
	}
}
