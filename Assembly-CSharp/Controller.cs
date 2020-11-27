using System;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class Controller
{
	// Token: 0x0600007A RID: 122 RVA: 0x0000CE38 File Offset: 0x0000B038
	public Controller()
	{
		for (int i = 0; i < this.Touches.Length; i++)
		{
			this.Touches[i] = new Controller.TouchState();
		}
	}

	// Token: 0x0600007B RID: 123 RVA: 0x0000CE80 File Offset: 0x0000B080
	public DragState CheckDrag(Collider2D coll, bool firstOnly = false)
	{
		if (!coll)
		{
			return DragState.NoTouch;
		}
		if (this.touchId <= -1)
		{
			if (firstOnly)
			{
				Controller.TouchState touchState = this.Touches[0];
				if (touchState.TouchStart && coll.OverlapPoint(touchState.Position))
				{
					this.amTouching = coll;
					this.touchId = 0;
					return DragState.TouchStart;
				}
			}
			else
			{
				for (int i = 0; i < this.Touches.Length; i++)
				{
					Controller.TouchState touchState2 = this.Touches[i];
					if (touchState2.TouchStart && coll.OverlapPoint(touchState2.Position))
					{
						this.amTouching = coll;
						this.touchId = i;
						return DragState.TouchStart;
					}
				}
			}
			return DragState.NoTouch;
		}
		if (coll != this.amTouching)
		{
			return DragState.NoTouch;
		}
		if (this.Touches[this.touchId].IsDown)
		{
			return DragState.Dragging;
		}
		this.amTouching = null;
		this.touchId = -1;
		return DragState.Released;
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x0600007C RID: 124 RVA: 0x0000261B File Offset: 0x0000081B
	public bool AnyTouch
	{
		get
		{
			return this.Touches[0].IsDown || this.Touches[1].IsDown;
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x0600007D RID: 125 RVA: 0x0000263B File Offset: 0x0000083B
	public bool AnyTouchDown
	{
		get
		{
			return this.Touches[0].TouchStart || this.Touches[1].TouchStart;
		}
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x0600007E RID: 126 RVA: 0x0000265B File Offset: 0x0000085B
	public bool AnyTouchUp
	{
		get
		{
			return this.Touches[0].TouchEnd || this.Touches[1].TouchEnd;
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x0600007F RID: 127 RVA: 0x0000267B File Offset: 0x0000087B
	public bool FirstDown
	{
		get
		{
			return this.Touches[0].TouchStart;
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x06000080 RID: 128 RVA: 0x0000268A File Offset: 0x0000088A
	public Vector2 DragPosition
	{
		get
		{
			return this.Touches[this.touchId].Position;
		}
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x06000081 RID: 129 RVA: 0x0000269E File Offset: 0x0000089E
	public Vector2 DragStartPosition
	{
		get
		{
			return this.Touches[this.touchId].DownAt;
		}
	}

	// Token: 0x06000082 RID: 130 RVA: 0x0000CF4C File Offset: 0x0000B14C
	public void Update()
	{
		Controller.TouchState touchState = this.Touches[0];
		bool mouseButton = Input.GetMouseButton(0);
		touchState.Position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		touchState.TouchStart = (!touchState.IsDown && mouseButton);
		if (touchState.TouchStart)
		{
			touchState.DownAt = touchState.Position;
		}
		touchState.TouchEnd = (touchState.IsDown && !mouseButton);
		touchState.IsDown = mouseButton;
	}

	// Token: 0x06000083 RID: 131 RVA: 0x0000CFC4 File Offset: 0x0000B1C4
	public void Reset()
	{
		for (int i = 0; i < this.Touches.Length; i++)
		{
			this.Touches[i] = new Controller.TouchState();
		}
		this.touchId = -1;
		this.amTouching = null;
	}

	// Token: 0x06000084 RID: 132 RVA: 0x000026B2 File Offset: 0x000008B2
	public Controller.TouchState GetTouch(int i)
	{
		return this.Touches[i];
	}

	// Token: 0x040000B6 RID: 182
	public readonly Controller.TouchState[] Touches = new Controller.TouchState[2];

	// Token: 0x040000B7 RID: 183
	private Collider2D amTouching;

	// Token: 0x040000B8 RID: 184
	private int touchId = -1;

	// Token: 0x0200001E RID: 30
	public class TouchState
	{
		// Token: 0x040000B9 RID: 185
		public Vector2 DownAt;

		// Token: 0x040000BA RID: 186
		public Vector2 Position;

		// Token: 0x040000BB RID: 187
		public bool WasDown;

		// Token: 0x040000BC RID: 188
		public bool IsDown;

		// Token: 0x040000BD RID: 189
		public bool TouchStart;

		// Token: 0x040000BE RID: 190
		public bool TouchEnd;
	}
}
