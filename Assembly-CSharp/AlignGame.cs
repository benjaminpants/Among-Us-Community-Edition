using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000086 RID: 134
public class AlignGame : Minigame
{
	// Token: 0x060002D5 RID: 725 RVA: 0x00015BFC File Offset: 0x00013DFC
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		float value = AlignGame.FromByte(this.MyNormTask.Data[base.ConsoleId]);
		bool flag = AlignGame.IsSuccess(this.MyNormTask.Data[base.ConsoleId]);
		Vector3 localPosition = this.col.transform.localPosition;
		localPosition.y = this.YRange.Clamp(value);
		float num = this.YRange.ReverseLerp(localPosition.y);
		localPosition.x = this.curve.Evaluate(num);
		this.col.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(20f, -20f, num));
		this.engine.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(45f, -45f, num));
		this.centerline.material.SetColor("_Color", flag ? Color.green : Color.red);
		this.engine.color = (flag ? Color.green : Color.red);
		this.col.transform.localPosition = localPosition;
		this.guidelines[0].enabled = flag;
		this.guidelines[1].enabled = flag;
		this.StatusText.gameObject.SetActive(flag);
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x00015D64 File Offset: 0x00013F64
	public void Update()
	{
		this.centerline.material.SetTextureOffset("_MainTex", new Vector2(Time.time, 0f));
		this.guidelines[0].material.SetTextureOffset("_MainTex", new Vector2(Time.time, 0f));
		this.guidelines[1].material.SetTextureOffset("_MainTex", new Vector2(Time.time, 0f));
		if (this.MyTask && this.MyNormTask.IsComplete)
		{
			return;
		}
		Vector3 localPosition = this.col.transform.localPosition;
		bool flag = AlignGame.IsSuccess(this.MyNormTask.Data[base.ConsoleId]);
		bool flag2 = AlignGame.IsSuccess(AlignGame.ToByte(localPosition.y));
		this.myController.Update();
		switch (this.myController.CheckDrag(this.col, false))
		{
		case DragState.TouchStart:
			this.pulseTimer = 0f;
			break;
		case DragState.Dragging:
			if (!flag)
			{
				Vector2 vector = this.myController.DragPosition - (Vector2)base.transform.position;
				float num = this.YRange.ReverseLerp(localPosition.y);
				localPosition.y = this.YRange.Clamp(vector.y);
				float num2 = this.YRange.ReverseLerp(localPosition.y);
				localPosition.x = this.curve.Evaluate(num2);
				this.col.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(20f, -20f, num2));
				this.engine.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(45f, -45f, num2));
				this.centerline.material.SetColor("_Color", flag2 ? Color.green : Color.red);
				if (Mathf.Abs(num2 - num) > 0.001f)
				{
					this.pulseTimer += Time.deltaTime * 25f;
					int num3 = (int)this.pulseTimer % 3;
					if (num3 > 1)
					{
						if (num3 == 2)
						{
							this.engine.color = Color.clear;
						}
					}
					else
					{
						this.engine.color = Color.red;
					}
				}
				else
				{
					this.engine.color = Color.red;
				}
			}
			break;
		case DragState.Released:
			if (!flag && flag2)
			{
				base.StartCoroutine(this.LockEngine());
				this.MyNormTask.Data[base.ConsoleId] = AlignGame.ToByte(localPosition.y);
				this.MyNormTask.NextStep();
				base.StartCoroutine(base.CoStartClose(0.75f));
			}
			break;
		}
		this.col.transform.localPosition = localPosition;
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x00003DBA File Offset: 0x00001FBA
	private IEnumerator LockEngine()
	{
		int num;
		for (int i = 0; i < 3; i = num)
		{
			this.guidelines[0].enabled = true;
			this.guidelines[1].enabled = true;
			yield return new WaitForSeconds(0.1f);
			this.guidelines[0].enabled = false;
			this.guidelines[1].enabled = false;
			yield return new WaitForSeconds(0.1f);
			num = i + 1;
		}
		this.StatusText.gameObject.SetActive(true);
		Color green = new Color(0f, 0.7f, 0f);
		yield return new WaitForLerp(1f, delegate(float t)
		{
			this.engine.color = Color.Lerp(Color.white, green, t);
		});
		this.guidelines[0].enabled = true;
		this.guidelines[1].enabled = true;
		yield break;
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x00003DC9 File Offset: 0x00001FC9
	public static float FromByte(byte b)
	{
		return (float)b * 0.025f - 3f;
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x00003DD9 File Offset: 0x00001FD9
	public static byte ToByte(float y)
	{
		return (byte)((y + 3f) / 0.025f);
	}

	// Token: 0x060002DA RID: 730 RVA: 0x00003DE9 File Offset: 0x00001FE9
	public static bool IsSuccess(byte b)
	{
		return Mathf.Abs(AlignGame.FromByte(b)) <= 0.05f;
	}

	// Token: 0x040002D2 RID: 722
	private Controller myController = new Controller();

	// Token: 0x040002D3 RID: 723
	public FloatRange YRange = new FloatRange(-0.425f, 0.425f);

	// Token: 0x040002D4 RID: 724
	public AnimationCurve curve;

	// Token: 0x040002D5 RID: 725
	public LineRenderer centerline;

	// Token: 0x040002D6 RID: 726
	public LineRenderer[] guidelines;

	// Token: 0x040002D7 RID: 727
	public SpriteRenderer engine;

	// Token: 0x040002D8 RID: 728
	public Collider2D col;

	// Token: 0x040002D9 RID: 729
	public TextController StatusText;

	// Token: 0x040002DA RID: 730
	private float pulseTimer;
}
