using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000BA RID: 186
public class ButtonBehavior : MonoBehaviour
{
	// Token: 0x060003E4 RID: 996 RVA: 0x00004880 File Offset: 0x00002A80
	public void OnEnable()
	{
		this.colliders = base.GetComponents<Collider2D>();
		this.myController.Reset();
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x00019070 File Offset: 0x00017270
	public void Update()
	{
		this.myController.Update();
		foreach (Collider2D coll in this.colliders)
		{
			switch (this.myController.CheckDrag(coll, false))
			{
			case DragState.TouchStart:
				if (this.OnDown)
				{
					this.OnClick.Invoke();
				}
				break;
			case DragState.Dragging:
				if (this.Repeat)
				{
					this.downTime += Time.fixedDeltaTime;
					if (this.downTime >= 0.3f)
					{
						this.downTime = 0f;
						this.OnClick.Invoke();
					}
				}
				else
				{
					this.downTime = 0f;
				}
				break;
			case DragState.Released:
				if (this.OnUp)
				{
					this.OnClick.Invoke();
				}
				break;
			}
		}
	}

	// Token: 0x040003D3 RID: 979
	public bool OnUp = true;

	// Token: 0x040003D4 RID: 980
	public bool OnDown;

	// Token: 0x040003D5 RID: 981
	public bool Repeat;

	// Token: 0x040003D6 RID: 982
	public Button.ButtonClickedEvent OnClick = new Button.ButtonClickedEvent();

	// Token: 0x040003D7 RID: 983
	private Controller myController = new Controller();

	// Token: 0x040003D8 RID: 984
	private Collider2D[] colliders;

	// Token: 0x040003D9 RID: 985
	private float downTime;
}
