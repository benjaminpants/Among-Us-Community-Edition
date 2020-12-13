using UnityEngine;

public class SortMinigame : Minigame
{
	public SortGameObject[] Objects;

	public BoxCollider2D AnimalBox;

	public BoxCollider2D PlantBox;

	public BoxCollider2D MineralBox;

	private Controller myController = new Controller();

	public void Start()
	{
		Objects.Shuffle();
		for (int i = 0; i < Objects.Length; i++)
		{
			Objects[i].transform.localPosition = new Vector3(Mathf.Lerp(-2f, 2f, (float)i / ((float)Objects.Length - 1f)), FloatRange.Next(-1.75f, -1f), -1f);
		}
	}

	public void Update()
	{
		if (amClosing != 0)
		{
			return;
		}
		myController.Update();
		for (int i = 0; i < Objects.Length; i++)
		{
			SortGameObject sortGameObject = Objects[i];
			switch (myController.CheckDrag(sortGameObject.Collider))
			{
			case DragState.Dragging:
			{
				Vector2 dragPosition = myController.DragPosition;
				sortGameObject.Collider.attachedRigidbody.position = dragPosition;
				break;
			}
			case DragState.Released:
			{
				bool flag = true;
				for (int j = 0; j < Objects.Length; j++)
				{
					SortGameObject sortGameObject2 = Objects[j];
					switch (sortGameObject2.MyType)
					{
					case SortGameObject.ObjType.Animal:
						flag &= sortGameObject2.Collider.IsTouching(AnimalBox);
						break;
					case SortGameObject.ObjType.Mineral:
						flag &= sortGameObject2.Collider.IsTouching(MineralBox);
						break;
					case SortGameObject.ObjType.Plant:
						flag &= sortGameObject2.Collider.IsTouching(PlantBox);
						break;
					}
				}
				if (flag)
				{
					MyTask.Complete();
					StartCoroutine(CoStartClose());
				}
				break;
			}
			}
		}
	}
}
