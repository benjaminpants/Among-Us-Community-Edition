using UnityEngine;

public class Scroller : MonoBehaviour
{
	public Transform Inner;

	public Collider2D HitBox;

	private Controller myController = new Controller();

	private Vector3 origin;

	public bool allowX;

	public FloatRange XBounds = new FloatRange(-10f, 10f);

	public bool allowY;

	public FloatRange YBounds = new FloatRange(-10f, 10f);

	public float YBoundPerItem;

	public bool AtTop => Inner.localPosition.y <= YBounds.min + 0.25f;

	public bool AtBottom => Inner.localPosition.y >= YBounds.max - 0.25f;

	public void FixedUpdate()
	{
		if (!CE_UIHelpers.IsActive() && (bool)Inner)
		{
			Vector2 mouseScrollDelta = Input.mouseScrollDelta;
			mouseScrollDelta.y = 0f - mouseScrollDelta.y;
			DoScroll(Inner.transform.localPosition, mouseScrollDelta);
			myController.Update();
			switch (myController.CheckDrag(HitBox))
			{
			case DragState.TouchStart:
				origin = Inner.transform.localPosition;
				break;
			case DragState.Dragging:
			{
				Vector2 del = myController.DragPosition - myController.DragStartPosition;
				DoScroll(origin, del);
				break;
			}
			}
		}
	}

	private void DoScroll(Vector3 origin, Vector2 del)
	{
		if (!(del.magnitude < 0.05f))
		{
			if (!allowX)
			{
				del.x = 0f;
			}
			if (!allowY)
			{
				del.y = 0f;
			}
			Vector3 localPosition = origin + (Vector3)del;
			localPosition.x = XBounds.Clamp(localPosition.x);
			int childCount = Inner.transform.childCount;
			float max = Mathf.Max(YBounds.min, YBounds.max + YBoundPerItem * (float)childCount);
			localPosition.y = Mathf.Clamp(localPosition.y, YBounds.min, max);
			Inner.transform.localPosition = localPosition;
		}
	}
}
