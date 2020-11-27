using System;
using UnityEngine;

// Token: 0x0200007D RID: 125
public class DivertPowerMinigame : Minigame
{
	// Token: 0x060002AA RID: 682 RVA: 0x00014B00 File Offset: 0x00012D00
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		DivertPowerTask powerTask = (DivertPowerTask)task;
		this.sliderId = this.SliderOrder.IndexOf((SystemTypes t) => t == powerTask.TargetSystem);
		for (int i = 0; i < this.Sliders.Length; i++)
		{
			if (i != this.sliderId)
			{
				this.Sliders[i].GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
			}
		}
	}

	// Token: 0x060002AB RID: 683 RVA: 0x00014B88 File Offset: 0x00012D88
	public void FixedUpdate()
	{
		this.myController.Update();
		float num = 0f;
		for (int i = 0; i < this.Sliders.Length; i++)
		{
			num += this.SliderY.ReverseLerp(this.Sliders[i].transform.localPosition.y) / (float)this.Sliders.Length;
		}
		for (int j = 0; j < this.Sliders.Length; j++)
		{
			float num2 = this.SliderY.ReverseLerp(this.Sliders[j].transform.localPosition.y);
			float num3 = num2 / num / 1.6f;
			this.Gauges[j].value = num3 + (Mathf.PerlinNoise((float)j, Time.time * 51f) - 0.5f) * 0.04f;
			Color value = Color.Lerp(Color.gray, Color.yellow, num2 * num2);
			value.a = (float)((num3 < 0.1f) ? 0 : 1);
			Vector2 textureOffset = this.Wires[j].material.GetTextureOffset("_MainTex");
			textureOffset.x -= Time.fixedDeltaTime * 3f * Mathf.Lerp(0.1f, 2f, num3);
			this.Wires[j].material.SetTextureOffset("_MainTex", textureOffset);
			this.Wires[j].material.SetColor("_Color", value);
		}
		if (this.sliderId < 0)
		{
			return;
		}
		Collider2D collider2D = this.Sliders[this.sliderId];
		Vector2 vector = collider2D.transform.localPosition;
		DragState dragState = this.myController.CheckDrag(collider2D, false);
		if (dragState == DragState.Dragging)
		{
			Vector2 vector2 = this.myController.DragPosition - (Vector2)collider2D.transform.parent.position;
			vector2.y = this.SliderY.Clamp(vector2.y);
			vector.y = vector2.y;
			collider2D.transform.localPosition = vector;
			return;
		}
		if (dragState != DragState.Released)
		{
			return;
		}
		if (this.SliderY.max - vector.y < 0.05f)
		{
			this.MyNormTask.NextStep();
			base.StartCoroutine(base.CoStartClose(0.75f));
			this.sliderId = -1;
			collider2D.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
		}
	}

	// Token: 0x04000290 RID: 656
	public SystemTypes[] SliderOrder = new SystemTypes[]
	{
		SystemTypes.LowerEngine,
		SystemTypes.UpperEngine,
		SystemTypes.Weapons,
		SystemTypes.Shields,
		SystemTypes.Nav,
		SystemTypes.Comms,
		SystemTypes.LifeSupp,
		SystemTypes.Security
	};

	// Token: 0x04000291 RID: 657
	public Collider2D[] Sliders;

	// Token: 0x04000292 RID: 658
	public LineRenderer[] Wires;

	// Token: 0x04000293 RID: 659
	public VerticalGauge[] Gauges;

	// Token: 0x04000294 RID: 660
	private int sliderId;

	// Token: 0x04000295 RID: 661
	public FloatRange SliderY = new FloatRange(-1f, 1f);

	// Token: 0x04000296 RID: 662
	private Controller myController = new Controller();
}
