using System;
using UnityEngine;

// Token: 0x020000C9 RID: 201
public interface IFocusHolder
{
	// Token: 0x0600044A RID: 1098
	void GiveFocus();

	// Token: 0x0600044B RID: 1099
	void LoseFocus();

	// Token: 0x0600044C RID: 1100
	bool CheckCollision(Vector2 pt);
}
