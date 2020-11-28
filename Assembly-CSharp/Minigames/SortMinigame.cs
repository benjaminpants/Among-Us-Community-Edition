using System;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class SortMinigame : Minigame
{
	// Token: 0x06000609 RID: 1545 RVA: 0x00025550 File Offset: 0x00023750
	public void Start()
	{
		this.Objects.Shuffle<SortGameObject>();
		for (int i = 0; i < this.Objects.Length; i++)
		{
			this.Objects[i].transform.localPosition = new Vector3(Mathf.Lerp(-2f, 2f, (float)i / ((float)this.Objects.Length - 1f)), FloatRange.Next(-1.75f, -1f), -1f);
		}
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x000255C8 File Offset: 0x000237C8
	public void Update()
	{
		if (this.amClosing != Minigame.CloseState.None)
		{
			return;
		}
		this.myController.Update();
		for (int i = 0; i < this.Objects.Length; i++)
		{
			SortGameObject sortGameObject = this.Objects[i];
			switch (this.myController.CheckDrag(sortGameObject.Collider, false))
			{
			case DragState.Dragging:
			{
				Vector2 dragPosition = this.myController.DragPosition;
				sortGameObject.Collider.attachedRigidbody.position = dragPosition;
				break;
			}
			case DragState.Released:
			{
				bool flag = true;
				for (int j = 0; j < this.Objects.Length; j++)
				{
					SortGameObject sortGameObject2 = this.Objects[j];
					switch (sortGameObject2.MyType)
					{
					case SortGameObject.ObjType.Plant:
						flag &= sortGameObject2.Collider.IsTouching(this.PlantBox);
						break;
					case SortGameObject.ObjType.Mineral:
						flag &= sortGameObject2.Collider.IsTouching(this.MineralBox);
						break;
					case SortGameObject.ObjType.Animal:
						flag &= sortGameObject2.Collider.IsTouching(this.AnimalBox);
						break;
					}
				}
				if (flag)
				{
					this.MyTask.Complete();
					base.StartCoroutine(base.CoStartClose(0.75f));
				}
				break;
			}
			}
		}
	}

	// Token: 0x040005F2 RID: 1522
	public SortGameObject[] Objects;

	// Token: 0x040005F3 RID: 1523
	public BoxCollider2D AnimalBox;

	// Token: 0x040005F4 RID: 1524
	public BoxCollider2D PlantBox;

	// Token: 0x040005F5 RID: 1525
	public BoxCollider2D MineralBox;

	// Token: 0x040005F6 RID: 1526
	private Controller myController = new Controller();
}
