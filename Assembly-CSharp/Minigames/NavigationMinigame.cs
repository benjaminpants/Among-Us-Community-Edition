using System.Collections;
using UnityEngine;

public class NavigationMinigame : Minigame
{
	public MeshRenderer TwoAxisImage;

	public SpriteRenderer CrossHairImage;

	public Collider2D hitbox;

	private Controller myController = new Controller();

	private Vector2 crossHair;

	private Vector2 half = new Vector2(0.5f, 0.5f);

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
        crossHair = Random.insideUnitCircle.normalized / 2f * 0.6f;
		half = new Vector2(0.5f, 0.5f);
		Vector3 localPosition = new Vector3(crossHair.x * TwoAxisImage.bounds.size.x, crossHair.y * TwoAxisImage.bounds.size.y, -2f);
		CrossHairImage.transform.localPosition = localPosition;
		TwoAxisImage.material.SetVector("_CrossHair", crossHair + half);
	}

	public void FixedUpdate()
	{
		if ((bool)MyNormTask && MyNormTask.IsComplete)
		{
			return;
		}
		myController.Update();
		switch (myController.CheckDrag(hitbox))
		{
		case DragState.Dragging:
		{
			Vector2 dragPosition = myController.DragPosition;
			Vector2 a = dragPosition - (Vector2)(TwoAxisImage.transform.position - TwoAxisImage.bounds.size / 2f);
			crossHair = a.Div(TwoAxisImage.bounds.size);
			if ((crossHair - half).magnitude < 0.45f)
			{
				Vector3 localPosition = dragPosition - (Vector2)base.transform.position;
				localPosition.z = -2f;
				CrossHairImage.transform.localPosition = localPosition;
				TwoAxisImage.material.SetVector("_CrossHair", crossHair);
			}
			break;
		}
		case DragState.Released:
			if ((crossHair - half).magnitude < 0.05f)
			{
				StartCoroutine(CompleteGame());
				MyNormTask.NextStep();
			}
			break;
		}
	}

	private IEnumerator CompleteGame()
	{
		WaitForSeconds wait = new WaitForSeconds(0.1f);
		Color green = new Color(0f, 0.8f, 0f, 1f);
		Color32 yellow = new Color32(byte.MaxValue, 202, 0, byte.MaxValue);
		CrossHairImage.transform.localPosition = new Vector3(0f, 0f, -2f);
		TwoAxisImage.material.SetVector("_CrossHair", half);
		CrossHairImage.color = yellow;
		TwoAxisImage.material.SetColor("_CrossColor", yellow);
		yield return wait;
		CrossHairImage.color = Color.white;
		TwoAxisImage.material.SetColor("_CrossColor", Color.white);
		yield return wait;
		CrossHairImage.color = yellow;
		TwoAxisImage.material.SetColor("_CrossColor", yellow);
		yield return wait;
		CrossHairImage.color = Color.white;
		TwoAxisImage.material.SetColor("_CrossColor", Color.white);
		yield return wait;
		CrossHairImage.color = green;
		TwoAxisImage.material.SetColor("_CrossColor", green);
		yield return CoStartClose();
	}
}
