using UnityEngine;

public class DialBehaviour : MonoBehaviour
{
	public FloatRange DialRange;

	public Collider2D collider;

	public Controller myController = new Controller();

	public float Value;

	public bool Engaged;

	public Transform DialTrans;

	public Transform DialShadTrans;

	public void Update()
	{
		Engaged = false;
		myController.Update();
		DragState dragState = myController.CheckDrag(collider);
		if (dragState == DragState.Dragging)
		{
			Vector2 vector = myController.DragPosition - (Vector2)base.transform.position;
			float num = Vector2.up.AngleSigned(vector);
			if (num < -180f)
			{
				num += 360f;
			}
			num = DialRange.Clamp(num);
			SetValue(num);
			Engaged = true;
		}
	}

	public void SetValue(float angle)
	{
		Value = angle;
		Vector3 localEulerAngles = new Vector3(0f, 0f, angle);
		DialTrans.localEulerAngles = localEulerAngles;
		DialShadTrans.localEulerAngles = localEulerAngles;
	}
}
