using System;
using UnityEngine;

// Token: 0x0200011D RID: 285
public class CrystalMinigame : Minigame
{
	// Token: 0x06000602 RID: 1538 RVA: 0x000251C4 File Offset: 0x000233C4
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		for (int i = 0; i < this.CrystalPieces.Length; i++)
		{
			this.CrystalPieces[i].transform.localPosition = new Vector3(this.XRange.Next(), this.YRange.Next(), ((float)i - (float)this.CrystalPieces.Length / 2f) / 100f);
		}
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x00025230 File Offset: 0x00023430
	public void Update()
	{
		this.myController.Update();
		for (int i = 0; i < this.CrystalPieces.Length; i++)
		{
			CrystalBehaviour crystalBehaviour = this.CrystalPieces[i];
			switch (this.myController.CheckDrag(crystalBehaviour.Collider, false))
			{
			case DragState.TouchStart:
				this.Spread(i);
				break;
			case DragState.Dragging:
			{
				Vector3 position = this.myController.DragPosition;
				position.z = base.transform.position.z;
				crystalBehaviour.transform.position = position;
				break;
			}
			case DragState.Released:
				this.CheckSolution();
				break;
			}
		}
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x000252D8 File Offset: 0x000234D8
	private void Spread(int parent)
	{
		for (int i = 0; i < this.CrystalPieces.Length; i++)
		{
			if (i < parent)
			{
				this.CrystalPieces[i].ParentSide = CrystalBehaviour.Parentness.Below;
			}
			else if (i > parent)
			{
				this.CrystalPieces[i].ParentSide = CrystalBehaviour.Parentness.Above;
			}
			else
			{
				this.CrystalPieces[i].ParentSide = CrystalBehaviour.Parentness.None;
			}
		}
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x00025330 File Offset: 0x00023530
	private void CheckSolution()
	{
		bool flag = false;
		for (int i = 0; i < this.CrystalPieces.Length; i++)
		{
			CrystalBehaviour crystalBehaviour = this.CrystalPieces[i];
			Vector3 position = crystalBehaviour.transform.position;
			if (!crystalBehaviour.Above && i - 1 > -1)
			{
				CrystalBehaviour crystalBehaviour2 = this.CrystalPieces[i - 1];
				if (CrystalMinigame.AreTouching(crystalBehaviour2.Collider, crystalBehaviour.Collider))
				{
					crystalBehaviour.Above = crystalBehaviour2;
					crystalBehaviour2.Below = crystalBehaviour;
					crystalBehaviour.FlashUp(0f);
				}
				else
				{
					flag = true;
				}
			}
			if (!crystalBehaviour.Below && i + 1 < this.CrystalPieces.Length)
			{
				CrystalBehaviour crystalBehaviour3 = this.CrystalPieces[i + 1];
				if (CrystalMinigame.AreTouching(crystalBehaviour3.Collider, crystalBehaviour.Collider))
				{
					crystalBehaviour.Below = crystalBehaviour3;
					crystalBehaviour3.Above = crystalBehaviour;
					crystalBehaviour.FlashDown(0f);
				}
				else
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			this.MyNormTask.Complete();
			base.StartCoroutine(base.CoStartClose(0.75f));
		}
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x00025434 File Offset: 0x00023634
	private static bool AreTouching(BoxCollider2D a, BoxCollider2D b)
	{
		Vector2 a2 = (Vector2)a.transform.position + a.offset;
		Vector2 a3 = (Vector2)b.transform.position + b.offset;
		Vector2 vector = a2 - a.size / 2f;
		Vector2 vector2 = a3 - b.size / 2f;
		Vector2 vector3 = a2 + a.size / 2f;
		Vector2 vector4 = a3 + b.size / 2f;
		return ((vector.y < vector4.y && vector.y > vector2.y) || (vector3.y < vector4.y && vector3.y > vector2.y)) && ((vector.x < vector4.x && vector.x > vector2.x) || (vector3.x < vector4.x && vector3.x > vector2.x));
	}

	// Token: 0x040005E8 RID: 1512
	public CrystalBehaviour[] CrystalPieces;

	// Token: 0x040005E9 RID: 1513
	public FloatRange XRange;

	// Token: 0x040005EA RID: 1514
	public FloatRange YRange;

	// Token: 0x040005EB RID: 1515
	private Controller myController = new Controller();
}
