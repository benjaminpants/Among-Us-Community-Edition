using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class NavigationMinigame : Minigame
{
	// Token: 0x060007AC RID: 1964 RVA: 0x0002BC08 File Offset: 0x00029E08
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		this.crossHair = UnityEngine.Random.insideUnitCircle.normalized / 2f * 0.6f;
		Vector3 localPosition = new Vector3(this.crossHair.x * this.TwoAxisImage.bounds.size.x, this.crossHair.y * this.TwoAxisImage.bounds.size.y, -2f);
		this.CrossHairImage.transform.localPosition = localPosition;
		this.TwoAxisImage.material.SetVector("_CrossHair", this.crossHair + this.half);
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x0002BCD4 File Offset: 0x00029ED4
	public void FixedUpdate()
	{
		if (this.MyNormTask && this.MyNormTask.IsComplete)
		{
			return;
		}
		this.myController.Update();
		DragState dragState = this.myController.CheckDrag(this.hitbox, false);
		if (dragState != DragState.Dragging)
		{
			if (dragState != DragState.Released)
			{
				return;
			}
			if ((this.crossHair - this.half).magnitude < 0.05f)
			{
				base.StartCoroutine(this.CompleteGame());
				this.MyNormTask.NextStep();
			}
		}
		else
		{
			Vector2 dragPosition = this.myController.DragPosition;
			Vector2 a = dragPosition - (Vector2)(this.TwoAxisImage.transform.position - this.TwoAxisImage.bounds.size / 2f);
			this.crossHair = a.Div(this.TwoAxisImage.bounds.size);
			if ((this.crossHair - this.half).magnitude < 0.45f)
			{
				Vector3 localPosition = dragPosition - (Vector2)base.transform.position;
				localPosition.z = -2f;
				this.CrossHairImage.transform.localPosition = localPosition;
				this.TwoAxisImage.material.SetVector("_CrossHair", this.crossHair);
				return;
			}
		}
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x00006B81 File Offset: 0x00004D81
	private IEnumerator CompleteGame()
	{
		WaitForSeconds wait = new WaitForSeconds(0.1f);
		Color green = new Color(0f, 0.8f, 0f, 1f);
		Color32 yellow = new Color32(byte.MaxValue, 202, 0, byte.MaxValue);
		this.CrossHairImage.transform.localPosition = new Vector3(0f, 0f, -2f);
		this.TwoAxisImage.material.SetVector("_CrossHair", this.half);
		this.CrossHairImage.color = yellow;
		this.TwoAxisImage.material.SetColor("_CrossColor", yellow);
		yield return wait;
		this.CrossHairImage.color = Color.white;
		this.TwoAxisImage.material.SetColor("_CrossColor", Color.white);
		yield return wait;
		this.CrossHairImage.color = yellow;
		this.TwoAxisImage.material.SetColor("_CrossColor", yellow);
		yield return wait;
		this.CrossHairImage.color = Color.white;
		this.TwoAxisImage.material.SetColor("_CrossColor", Color.white);
		yield return wait;
		this.CrossHairImage.color = green;
		this.TwoAxisImage.material.SetColor("_CrossColor", green);
		yield return base.CoStartClose(0.75f);
		yield break;
	}

	// Token: 0x04000790 RID: 1936
	public MeshRenderer TwoAxisImage;

	// Token: 0x04000791 RID: 1937
	public SpriteRenderer CrossHairImage;

	// Token: 0x04000792 RID: 1938
	public Collider2D hitbox;

	// Token: 0x04000793 RID: 1939
	private Controller myController = new Controller();

	// Token: 0x04000794 RID: 1940
	private Vector2 crossHair;

	// Token: 0x04000795 RID: 1941
	private Vector2 half = new Vector2(0.5f, 0.5f);
}
