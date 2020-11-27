using System;
using UnityEngine;

// Token: 0x02000084 RID: 132
public class WireMinigame : Minigame
{
	// Token: 0x060002CB RID: 715 RVA: 0x0001581C File Offset: 0x00013A1C
	private bool TaskIsForThisPanel()
	{
		return this.MyNormTask.taskStep < this.MyNormTask.Data.Length && !this.MyNormTask.IsComplete && (int)this.MyNormTask.Data[this.MyNormTask.taskStep] == base.ConsoleId;
	}

	// Token: 0x060002CC RID: 716 RVA: 0x00015874 File Offset: 0x00013A74
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		IntRange.FillRandomRange(this.ExpectedWires);
		for (int i = 0; i < this.LeftNodes.Length; i++)
		{
			this.ActualWires[i] = -1;
			int num = (int)this.ExpectedWires[i];
			Wire wire = this.LeftNodes[i];
			wire.SetColor(WireMinigame.colors[num]);
			wire.WireId = (sbyte)i;
			this.RightNodes[i].SetColor(WireMinigame.colors[i]);
			this.RightNodes[i].WireId = (sbyte)i;
			int num2 = (int)this.ActualWires[i];
			if (num2 > -1)
			{
				wire.ConnectRight(this.RightNodes[num2]);
			}
			else
			{
				wire.ResetLine(Vector3.zero, true);
			}
		}
		this.UpdateLights();
	}

	// Token: 0x060002CD RID: 717 RVA: 0x00015938 File Offset: 0x00013B38
	public void Update()
	{
		if (!this.TaskIsForThisPanel())
		{
			return;
		}
		this.myController.Update();
		//base.transform.position;
		for (int i = 0; i < this.LeftNodes.Length; i++)
		{
			Wire wire = this.LeftNodes[i];
			DragState dragState = this.myController.CheckDrag(wire.hitbox, false);
			if (dragState != DragState.Dragging)
			{
				if (dragState == DragState.Released)
				{
					if (this.ActualWires[(int)wire.WireId] == -1)
					{
						wire.ResetLine(wire.BaseWorldPos, true);
					}
					else if (Constants.ShouldPlaySfx())
					{
						SoundManager.Instance.PlaySound(this.WireSounds.Random<AudioClip>(), false, 1f);
					}
					this.CheckTask();
				}
			}
			else
			{
				Vector2 vector = this.myController.DragPosition;
				WireNode wireNode = this.CheckRightSide(vector);
				if (wireNode)
				{
					vector = wireNode.transform.position;
					this.ActualWires[(int)wire.WireId] = wireNode.WireId;
				}
				else
				{
					vector -= wire.BaseWorldPos.normalized * 0.05f;
					this.ActualWires[(int)wire.WireId] = -1;
				}
				wire.ResetLine(vector, false);
			}
		}
		this.UpdateLights();
	}

	// Token: 0x060002CE RID: 718 RVA: 0x00015A88 File Offset: 0x00013C88
	private void UpdateLights()
	{
		for (int i = 0; i < this.ActualWires.Length; i++)
		{
			Color color = Color.yellow;
			color *= 1f - Mathf.PerlinNoise((float)i, Time.time * 35f) * 0.3f;
			color.a = 1f;
			if (this.ActualWires[i] != this.ExpectedWires[i])
			{
				this.RightLights[(int)this.ExpectedWires[i]].color = new Color(0.2f, 0.2f, 0.2f);
			}
			else
			{
				this.RightLights[(int)this.ExpectedWires[i]].color = color;
			}
			this.LeftLights[i].color = color;
		}
	}

	// Token: 0x060002CF RID: 719 RVA: 0x00015B44 File Offset: 0x00013D44
	private WireNode CheckRightSide(Vector2 pos)
	{
		for (int i = 0; i < this.RightNodes.Length; i++)
		{
			WireNode wireNode = this.RightNodes[i];
			if (wireNode.hitbox.OverlapPoint(pos))
			{
				return wireNode;
			}
		}
		return null;
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x00015B80 File Offset: 0x00013D80
	private void CheckTask()
	{
		bool flag = true;
		for (int i = 0; i < this.ActualWires.Length; i++)
		{
			if (this.ActualWires[i] != this.ExpectedWires[i])
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			this.MyNormTask.NextStep();
			this.Close();
		}
	}

	// Token: 0x040002C6 RID: 710
	private static readonly Color[] colors = new Color[]
	{
		Color.red,
		Color.blue,
		Color.yellow,
		Color.magenta
	};

	// Token: 0x040002C7 RID: 711
	public Wire[] LeftNodes;

	// Token: 0x040002C8 RID: 712
	public WireNode[] RightNodes;

	// Token: 0x040002C9 RID: 713
	public SpriteRenderer[] LeftLights;

	// Token: 0x040002CA RID: 714
	public SpriteRenderer[] RightLights;

	// Token: 0x040002CB RID: 715
	private Controller myController = new Controller();

	// Token: 0x040002CC RID: 716
	private sbyte[] ExpectedWires = new sbyte[4];

	// Token: 0x040002CD RID: 717
	private sbyte[] ActualWires = new sbyte[4];

	// Token: 0x040002CE RID: 718
	public AudioClip[] WireSounds;
}
