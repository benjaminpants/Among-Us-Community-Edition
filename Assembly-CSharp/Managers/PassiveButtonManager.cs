using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class PassiveButtonManager : DestroyableSingleton<PassiveButtonManager>
{
	// Token: 0x06000543 RID: 1347 RVA: 0x00005526 File Offset: 0x00003726
	public void RegisterOne(PassiveButton button)
	{
		this.Buttons.Add(button);
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x00005534 File Offset: 0x00003734
	public void RemoveOne(PassiveButton passiveButton)
	{
		this.Buttons.Remove(passiveButton);
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x00005543 File Offset: 0x00003743
	public void RegisterOne(IFocusHolder focusHolder)
	{
		this.FocusHolders.Add(focusHolder);
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x00005551 File Offset: 0x00003751
	public void RemoveOne(IFocusHolder focusHolder)
	{
		this.FocusHolders.Remove(focusHolder);
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x000225EC File Offset: 0x000207EC
	public void Update()
	{
		this.Controller.Update();
		for (int i = 1; i < this.Buttons.Count; i++)
		{
			if (PassiveButtonManager.DepthComparer.Instance.Compare(this.Buttons[i - 1], this.Buttons[i]) > 0)
			{
				this.Buttons.Sort(PassiveButtonManager.DepthComparer.Instance);
				break;
			}
		}
		Vector2 position = this.Controller.Touches[0].Position;
		int num = Physics2D.OverlapPointNonAlloc(position, this.results);
		bool flag = false;
		for (int j = 0; j < this.Buttons.Count; j++)
		{
			PassiveButton passiveButton = this.Buttons[j];
			if (!passiveButton)
			{
				this.Buttons.RemoveAt(j);
				j--;
			}
			else if (passiveButton.isActiveAndEnabled)
			{
				bool flag2 = false;
				for (int k = 0; k < num; k++)
				{
					if (this.results[k].gameObject == passiveButton.gameObject)
					{
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					flag = true;
					if (passiveButton != this.currentOver)
					{
						if (this.currentOver)
						{
							this.currentOver.OnMouseOut.Invoke();
						}
						this.currentOver = passiveButton;
						this.currentDown = null;
						this.currentOver.OnMouseOver.Invoke();
						break;
					}
					break;
				}
			}
		}
		if (!flag && this.currentOver)
		{
			this.currentOver.OnMouseOut.Invoke();
			this.currentOver = null;
			this.currentDown = null;
		}
		if (this.Controller.AnyTouchDown)
		{
			if (this.currentOver)
			{
				this.currentDown = this.currentOver;
				if (this.currentOver.OnDown)
				{
					this.currentOver.DoClick();
				}
			}
			this.HandleFocus(position);
			return;
		}
		if (this.Controller.AnyTouchUp && this.currentDown)
		{
			if (this.currentDown.OnUp)
			{
				this.currentDown.DoClick();
			}
			this.currentDown = null;
		}
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x00022808 File Offset: 0x00020A08
	private void CheckForDown()
	{
		Vector2 touch = this.GetTouch(true);
		for (int i = 0; i < this.Buttons.Count; i++)
		{
			PassiveButton passiveButton = this.Buttons[i];
			if (!passiveButton)
			{
				this.Buttons.RemoveAt(i);
				i--;
			}
			else if (passiveButton.isActiveAndEnabled)
			{
				for (int j = 0; j < passiveButton.Colliders.Length; j++)
				{
					Collider2D collider2D = passiveButton.Colliders[j];
					if (collider2D && collider2D.OverlapPoint(touch))
					{
						this.currentDown = passiveButton;
						if (passiveButton.OnDown)
						{
							passiveButton.DoClick();
						}
						return;
					}
				}
			}
		}
		this.HandleFocus(touch);
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x000228B4 File Offset: 0x00020AB4
	private void HandleFocus(Vector2 pt)
	{
		bool flag = false;
		for (int i = 0; i < this.FocusHolders.Count; i++)
		{
			IFocusHolder focusHolder = this.FocusHolders[i];
			if (!(focusHolder as MonoBehaviour))
			{
				this.FocusHolders.RemoveAt(i);
				i--;
			}
			else if (focusHolder.CheckCollision(pt))
			{
				flag = true;
				focusHolder.GiveFocus();
				for (int j = 0; j < this.FocusHolders.Count; j++)
				{
					if (j != i)
					{
						this.FocusHolders[j].LoseFocus();
					}
				}
				break;
			}
		}
		if (!flag)
		{
			for (int k = 0; k < this.FocusHolders.Count; k++)
			{
				this.FocusHolders[k].LoseFocus();
			}
		}
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x00005560 File Offset: 0x00003760
	private void HandleMouseOut(PassiveButton button)
	{
		if (this.currentOver == button)
		{
			button.OnMouseOut.Invoke();
			this.currentOver = null;
		}
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x00022974 File Offset: 0x00020B74
	private void CheckForUp()
	{
		if (!this.currentDown)
		{
			return;
		}
		PassiveButton passiveButton = this.currentDown;
		this.currentDown = null;
		if (!passiveButton.OnUp)
		{
			return;
		}
		Vector2 touch = this.GetTouch(false);
		for (int i = 0; i < passiveButton.Colliders.Length; i++)
		{
			if (passiveButton.Colliders[i].OverlapPoint(touch))
			{
				if (passiveButton.OnUp)
				{
					passiveButton.DoClick();
				}
				return;
			}
		}
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x000229E0 File Offset: 0x00020BE0
	private Vector2 GetTouch(bool downOrUp)
	{
		if (downOrUp)
		{
			if (this.Controller.Touches[0].TouchStart)
			{
				return this.Controller.Touches[0].Position;
			}
			return this.Controller.Touches[1].Position;
		}
		else
		{
			if (this.Controller.Touches[0].TouchEnd)
			{
				return this.Controller.Touches[0].Position;
			}
			return this.Controller.Touches[1].Position;
		}
	}

	// Token: 0x04000514 RID: 1300
	public List<PassiveButton> Buttons = new List<PassiveButton>();

	// Token: 0x04000515 RID: 1301
	private List<IFocusHolder> FocusHolders = new List<IFocusHolder>();

	// Token: 0x04000516 RID: 1302
	private PassiveButton currentOver;

	// Token: 0x04000517 RID: 1303
	public Controller Controller = new Controller();

	// Token: 0x04000518 RID: 1304
	private PassiveButton currentDown;

	// Token: 0x04000519 RID: 1305
	private Collider2D[] results = new Collider2D[40];

	// Token: 0x020000F8 RID: 248
	private class DepthComparer : IComparer<PassiveButton>
	{
		// Token: 0x0600054E RID: 1358 RVA: 0x00022A64 File Offset: 0x00020C64
		public int Compare(PassiveButton x, PassiveButton y)
		{
			if (x == null)
			{
				return 1;
			}
			if (y == null)
			{
				return -1;
			}
			return x.transform.position.z.CompareTo(y.transform.position.z);
		}

		// Token: 0x0400051A RID: 1306
		public static readonly PassiveButtonManager.DepthComparer Instance = new PassiveButtonManager.DepthComparer();
	}
}
