using UnityEngine;

public class CrystalMinigame : Minigame
{
	public CrystalBehaviour[] CrystalPieces;

	public FloatRange XRange;

	public FloatRange YRange;

	private Controller myController = new Controller();

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		for (int i = 0; i < CrystalPieces.Length; i++)
		{
			CrystalPieces[i].transform.localPosition = new Vector3(XRange.Next(), YRange.Next(), ((float)i - (float)CrystalPieces.Length / 2f) / 100f);
		}
	}

	public void Update()
	{
		myController.Update();
		for (int i = 0; i < CrystalPieces.Length; i++)
		{
			CrystalBehaviour crystalBehaviour = CrystalPieces[i];
			switch (myController.CheckDrag(crystalBehaviour.Collider))
			{
			case DragState.TouchStart:
				Spread(i);
				break;
			case DragState.Dragging:
			{
				Vector3 position = myController.DragPosition;
				position.z = base.transform.position.z;
				crystalBehaviour.transform.position = position;
				break;
			}
			case DragState.Released:
				CheckSolution();
				break;
			}
		}
	}

	private void Spread(int parent)
	{
		for (int i = 0; i < CrystalPieces.Length; i++)
		{
			if (i < parent)
			{
				CrystalPieces[i].ParentSide = CrystalBehaviour.Parentness.Below;
			}
			else if (i > parent)
			{
				CrystalPieces[i].ParentSide = CrystalBehaviour.Parentness.Above;
			}
			else
			{
				CrystalPieces[i].ParentSide = CrystalBehaviour.Parentness.None;
			}
		}
	}

	private void CheckSolution()
	{
		bool flag = false;
		for (int i = 0; i < CrystalPieces.Length; i++)
		{
			CrystalBehaviour crystalBehaviour = CrystalPieces[i];
			_ = crystalBehaviour.transform.position;
			if (!crystalBehaviour.Above && i - 1 > -1)
			{
				CrystalBehaviour crystalBehaviour2 = CrystalPieces[i - 1];
				if (AreTouching(crystalBehaviour2.Collider, crystalBehaviour.Collider))
				{
					crystalBehaviour.Above = crystalBehaviour2;
					crystalBehaviour2.Below = crystalBehaviour;
					crystalBehaviour.FlashUp();
				}
				else
				{
					flag = true;
				}
			}
			if (!crystalBehaviour.Below && i + 1 < CrystalPieces.Length)
			{
				CrystalBehaviour crystalBehaviour3 = CrystalPieces[i + 1];
				if (AreTouching(crystalBehaviour3.Collider, crystalBehaviour.Collider))
				{
					crystalBehaviour.Below = crystalBehaviour3;
					crystalBehaviour3.Above = crystalBehaviour;
					crystalBehaviour.FlashDown();
				}
				else
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			MyNormTask.Complete();
			StartCoroutine(CoStartClose());
		}
	}

	private static bool AreTouching(BoxCollider2D a, BoxCollider2D b)
	{
		Vector2 a2 = (Vector2)a.transform.position + a.offset;
		Vector2 a3 = (Vector2)b.transform.position + b.offset;
		Vector2 vector = a2 - a.size / 2f;
		Vector2 vector2 = a3 - b.size / 2f;
		Vector2 vector3 = a2 + a.size / 2f;
		Vector2 vector4 = a3 + b.size / 2f;
		if ((vector.y < vector4.y && vector.y > vector2.y) || (vector3.y < vector4.y && vector3.y > vector2.y))
		{
			if (!(vector.x < vector4.x) || !(vector.x > vector2.x))
			{
				if (vector3.x < vector4.x)
				{
					return vector3.x > vector2.x;
				}
				return false;
			}
			return true;
		}
		return false;
	}
}
