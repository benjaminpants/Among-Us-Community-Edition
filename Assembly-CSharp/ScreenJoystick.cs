using System;
using UnityEngine;

// Token: 0x020000CD RID: 205
public class ScreenJoystick : MonoBehaviour, IVirtualJoystick
{
	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x06000454 RID: 1108 RVA: 0x00004C30 File Offset: 0x00002E30
	// (set) Token: 0x06000455 RID: 1109 RVA: 0x00004C38 File Offset: 0x00002E38
	public Vector2 Delta { get; private set; }

	// Token: 0x06000456 RID: 1110 RVA: 0x0001A8C8 File Offset: 0x00018AC8
	private void FixedUpdate()
	{
		this.myController.Update();
		if (this.touchId <= -1)
		{
			for (int i = 0; i < this.myController.Touches.Length; i++)
			{
				Controller.TouchState touchState = this.myController.Touches[i];
				if (touchState.TouchStart)
				{
					bool flag = false;
					int num = Physics2D.OverlapPointNonAlloc(touchState.Position, this.hitBuffer, Constants.NotShipMask);
					for (int j = 0; j < num; j++)
					{
						Collider2D collider2D = this.hitBuffer[j];
						if (collider2D.GetComponent<ButtonBehavior>() || collider2D.GetComponent<PassiveButton>())
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						this.touchId = i;
						return;
					}
				}
			}
			return;
		}
		Controller.TouchState touchState2 = this.myController.Touches[this.touchId];
		if (touchState2.IsDown)
		{
			Vector2 b = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
			this.Delta = (touchState2.Position - b).normalized;
			return;
		}
		this.touchId = -1;
		this.Delta = Vector2.zero;
	}

	// Token: 0x04000440 RID: 1088
	private Collider2D[] hitBuffer = new Collider2D[20];

	// Token: 0x04000442 RID: 1090
	private Controller myController = new Controller();

	// Token: 0x04000443 RID: 1091
	private int touchId = -1;
}
