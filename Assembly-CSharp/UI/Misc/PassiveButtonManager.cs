using System.Collections.Generic;
using UnityEngine;

public class PassiveButtonManager : DestroyableSingleton<PassiveButtonManager>
{
	private class DepthComparer : IComparer<PassiveButton>
	{
		public static readonly DepthComparer Instance = new DepthComparer();

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
	}

	public List<PassiveButton> Buttons = new List<PassiveButton>();

	private List<IFocusHolder> FocusHolders = new List<IFocusHolder>();

	private PassiveButton currentOver;

	public Controller Controller = new Controller();

	private PassiveButton currentDown;

	private Collider2D[] results = new Collider2D[40];

	public void RegisterOne(PassiveButton button)
	{
		Buttons.Add(button);
	}

	public void RemoveOne(PassiveButton passiveButton)
	{
		Buttons.Remove(passiveButton);
	}

	public void RegisterOne(IFocusHolder focusHolder)
	{
		FocusHolders.Add(focusHolder);
	}

	public void RemoveOne(IFocusHolder focusHolder)
	{
		FocusHolders.Remove(focusHolder);
	}

	public void Update()
	{
		Controller.Update();
		for (int i = 1; i < Buttons.Count; i++)
		{
			if (DepthComparer.Instance.Compare(Buttons[i - 1], Buttons[i]) > 0)
			{
				Buttons.Sort(DepthComparer.Instance);
				break;
			}
		}
		Vector2 position = Controller.Touches[0].Position;
		int num = Physics2D.OverlapPointNonAlloc(position, results);
		bool flag = false;
		for (int j = 0; j < Buttons.Count; j++)
		{
			PassiveButton passiveButton = Buttons[j];
			if (!passiveButton)
			{
				Buttons.RemoveAt(j);
				j--;
			}
			else
			{
				if (!passiveButton.isActiveAndEnabled)
				{
					continue;
				}
				bool flag2 = false;
				for (int k = 0; k < num; k++)
				{
					if (results[k].gameObject == passiveButton.gameObject)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					continue;
				}
				flag = true;
				if (passiveButton != currentOver)
				{
					if ((bool)currentOver)
					{
						currentOver.OnMouseOut.Invoke();
					}
					currentOver = passiveButton;
					currentDown = null;
					currentOver.OnMouseOver.Invoke();
				}
				break;
			}
		}
		if (!flag && (bool)currentOver)
		{
			currentOver.OnMouseOut.Invoke();
			currentOver = null;
			currentDown = null;
		}
		if (Controller.AnyTouchDown)
		{
			if ((bool)currentOver)
			{
				currentDown = currentOver;
				if (currentOver.OnDown)
				{
					currentOver.DoClick();
				}
			}
			HandleFocus(position);
		}
		else if (Controller.AnyTouchUp && (bool)currentDown)
		{
			if (currentDown.OnUp)
			{
				currentDown.DoClick();
			}
			currentDown = null;
		}
	}

	private void CheckForDown()
	{
		Vector2 touch = GetTouch(downOrUp: true);
		for (int i = 0; i < Buttons.Count; i++)
		{
			PassiveButton passiveButton = Buttons[i];
			if (!passiveButton)
			{
				Buttons.RemoveAt(i);
				i--;
			}
			else
			{
				if (!passiveButton.isActiveAndEnabled)
				{
					continue;
				}
				for (int j = 0; j < passiveButton.Colliders.Length; j++)
				{
					Collider2D collider2D = passiveButton.Colliders[j];
					if ((bool)collider2D && collider2D.OverlapPoint(touch))
					{
						currentDown = passiveButton;
						if (passiveButton.OnDown)
						{
							passiveButton.DoClick();
						}
						return;
					}
				}
			}
		}
		HandleFocus(touch);
	}

	private void HandleFocus(Vector2 pt)
	{
		bool flag = false;
		for (int i = 0; i < FocusHolders.Count; i++)
		{
			IFocusHolder focusHolder = FocusHolders[i];
			if (!(focusHolder as MonoBehaviour))
			{
				FocusHolders.RemoveAt(i);
				i--;
			}
			else
			{
				if (!focusHolder.CheckCollision(pt))
				{
					continue;
				}
				flag = true;
				focusHolder.GiveFocus();
				for (int j = 0; j < FocusHolders.Count; j++)
				{
					if (j != i)
					{
						FocusHolders[j].LoseFocus();
					}
				}
				break;
			}
		}
		if (!flag)
		{
			for (int k = 0; k < FocusHolders.Count; k++)
			{
				FocusHolders[k].LoseFocus();
			}
		}
	}

	private void HandleMouseOut(PassiveButton button)
	{
		if (currentOver == button)
		{
			button.OnMouseOut.Invoke();
			currentOver = null;
		}
	}

	private void CheckForUp()
	{
		if (!currentDown)
		{
			return;
		}
		PassiveButton passiveButton = currentDown;
		currentDown = null;
		if (!passiveButton.OnUp)
		{
			return;
		}
		Vector2 touch = GetTouch(downOrUp: false);
		for (int i = 0; i < passiveButton.Colliders.Length; i++)
		{
			if (passiveButton.Colliders[i].OverlapPoint(touch))
			{
				if (passiveButton.OnUp)
				{
					passiveButton.DoClick();
				}
				break;
			}
		}
	}

	private Vector2 GetTouch(bool downOrUp)
	{
		if (downOrUp)
		{
			if (Controller.Touches[0].TouchStart)
			{
				return Controller.Touches[0].Position;
			}
			return Controller.Touches[1].Position;
		}
		if (Controller.Touches[0].TouchEnd)
		{
			return Controller.Touches[0].Position;
		}
		return Controller.Touches[1].Position;
	}
}
