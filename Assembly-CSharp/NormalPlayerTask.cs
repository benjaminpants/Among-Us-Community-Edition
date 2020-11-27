using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Token: 0x02000214 RID: 532
public class NormalPlayerTask : PlayerTask
{
	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x06000B84 RID: 2948 RVA: 0x00008ED8 File Offset: 0x000070D8
	public override int TaskStep
	{
		get
		{
			return this.taskStep;
		}
	}

	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x06000B85 RID: 2949 RVA: 0x00008EE0 File Offset: 0x000070E0
	public override bool IsComplete
	{
		get
		{
			return this.taskStep >= this.MaxStep;
		}
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x000392B4 File Offset: 0x000374B4
	public override void Initialize()
	{
		this.HasLocation = true;
		this.LocationDirty = true;
		TaskTypes taskType = this.TaskType;
		switch (taskType)
		{
		case TaskTypes.PrimeShields:
		{
			this.Data = new byte[1];
			int num = 0;
			for (int i = 0; i < 7; i++)
			{
				byte b = (byte)(1 << i);
				if (BoolRange.Next(0.7f))
				{
					byte[] data = this.Data;
					int num2 = 0;
					data[num2] |= b;
					num++;
				}
			}
			byte[] data2 = this.Data;
			int num3 = 0;
			data2[num3] &= 118;
			return;
		}
		case TaskTypes.FuelEngines:
			this.Data = new byte[2];
			return;
		case TaskTypes.ChartCourse:
			this.Data = new byte[4];
			return;
		case TaskTypes.StartReactor:
			this.Data = new byte[6];
			return;
		case TaskTypes.SwipeCard:
		case TaskTypes.ClearAsteroids:
		case TaskTypes.UploadData:
		case TaskTypes.EmptyChute:
		case TaskTypes.EmptyGarbage:
			break;
		case TaskTypes.InspectSample:
			this.Data = new byte[2];
			return;
		case TaskTypes.AlignEngineOutput:
			this.Data = new byte[2];
			this.Data[0] = AlignGame.ToByte((float)IntRange.RandomSign() * FloatRange.Next(1f, 3f));
			this.Data[1] = (byte)(IntRange.RandomSign() * IntRange.Next(25, 255));
			return;
		case TaskTypes.FixWiring:
		{
			this.Data = new byte[this.MaxStep];
			List<global::Console> list = (from t in ShipStatus.Instance.AllConsoles
			where t.TaskTypes.Contains(TaskTypes.FixWiring)
			select t).ToList<global::Console>();
			List<global::Console> list2 = new List<global::Console>(list);
			for (int j = 0; j < this.Data.Length; j++)
			{
				int index = list2.RandomIdx<global::Console>();
				this.Data[j] = (byte)list2[index].ConsoleId;
				list2.RemoveAt(index);
			}
			Array.Sort<byte>(this.Data);
			global::Console console = list.First((global::Console v) => v.ConsoleId == (int)this.Data[0]);
			this.StartAt = console.Room;
			break;
		}
		default:
			if (taskType == TaskTypes.EnterIdCode)
			{
				this.Data = BitConverter.GetBytes(IntRange.Next(1, 99999));
				return;
			}
			break;
		}
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x000394C4 File Offset: 0x000376C4
	public void NextStep()
	{
		this.taskStep++;
		this.UpdateArrow();
		if (this.taskStep >= this.MaxStep)
		{
			this.taskStep = this.MaxStep;
			if (PlayerControl.LocalPlayer)
			{
				if (DestroyableSingleton<HudManager>.InstanceExists)
				{
					DestroyableSingleton<HudManager>.Instance.ShowTaskComplete();
					StatsManager instance = StatsManager.Instance;
					uint num = instance.TasksCompleted;
					instance.TasksCompleted = num + 1U;
					if (PlayerTask.AllTasksCompleted(PlayerControl.LocalPlayer))
					{
						StatsManager instance2 = StatsManager.Instance;
						num = instance2.CompletedAllTasks;
						instance2.CompletedAllTasks = num + 1U;
					}
				}
				PlayerControl.LocalPlayer.RpcCompleteTask(base.Id);
				return;
			}
		}
		else if (this.ShowTaskStep && Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(DestroyableSingleton<HudManager>.Instance.TaskUpdateSound, false, 1f);
		}
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x00039590 File Offset: 0x00037790
	public void UpdateArrow()
	{
		if (this.Arrow)
		{
			if (!this.IsComplete && base.Owner.AmOwner)
			{
				this.Arrow.gameObject.SetActive(true);
				if (this.TaskType == TaskTypes.FixWiring)
				{
					global::Console console3 = base.FindSpecialConsole((global::Console c) => c.TaskTypes.Contains(TaskTypes.FixWiring) && c.ConsoleId == (int)this.Data[this.taskStep]);
					this.Arrow.target = console3.transform.position;
					this.StartAt = console3.Room;
				}
				else if (this.TaskType == TaskTypes.AlignEngineOutput)
				{
					if (AlignGame.IsSuccess(this.Data[0]))
					{
						this.Arrow.target = base.FindSpecialConsole((global::Console c) => c.TaskTypes.Contains(TaskTypes.AlignEngineOutput) && c.ConsoleId == 1).transform.position;
						this.StartAt = SystemTypes.UpperEngine;
					}
					else
					{
						this.Arrow.target = base.FindSpecialConsole((global::Console console) => console.TaskTypes.Contains(TaskTypes.AlignEngineOutput) && console.ConsoleId == 0).transform.position;
						this.StartAt = SystemTypes.LowerEngine;
					}
				}
				else
				{
					global::Console console2 = base.FindObjectPos();
					this.Arrow.target = console2.transform.position;
					this.StartAt = console2.Room;
				}
				this.LocationDirty = true;
				return;
			}
			this.Arrow.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x00008EF3 File Offset: 0x000070F3
	private void FixedUpdate()
	{
		if (this.TimerStarted == NormalPlayerTask.TimerState.Started)
		{
			this.TaskTimer -= Time.fixedDeltaTime;
			if (this.TaskTimer <= 0f)
			{
				this.TaskTimer = 0f;
				this.TimerStarted = NormalPlayerTask.TimerState.Finished;
			}
		}
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x00039704 File Offset: 0x00037904
	public override bool ValidConsole(global::Console console)
	{
		if (this.TaskType == TaskTypes.FixWiring)
		{
			return console.TaskTypes.Contains(this.TaskType) && console.ConsoleId == (int)this.Data[this.taskStep];
		}
		if (this.TaskType == TaskTypes.AlignEngineOutput)
		{
			return console.TaskTypes.Contains(this.TaskType) && !AlignGame.IsSuccess(this.Data[console.ConsoleId]);
		}
		if (this.TaskType == TaskTypes.FuelEngines)
		{
			return (console.TaskTypes.Contains(this.TaskType) && console.ConsoleId == (int)this.Data[1]) || console.ValidTasks.Any((TaskSet set) => this.TaskType == set.taskType && set.taskStep.Contains((int)this.Data[1]));
		}
		return console.TaskTypes.Any((TaskTypes tt) => tt == this.TaskType) || console.ValidTasks.Any((TaskSet set) => this.TaskType == set.taskType && set.taskStep.Contains(this.taskStep));
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x00008F2F File Offset: 0x0000712F
	public override void Complete()
	{
		this.taskStep = this.MaxStep;
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x000397F4 File Offset: 0x000379F4
	public override void AppendTaskText(StringBuilder sb)
	{
		bool flag = this.ShouldYellowText();
		if (flag)
		{
			if (this.IsComplete)
			{
				sb.Append("[00DD00FF]");
			}
			else
			{
				sb.Append("[FFFF00FF]");
			}
		}
		sb.Append(SystemTypeHelpers.StringNames[(int)this.StartAt]);
		sb.Append(": ");
		sb.Append(TaskTypesHelpers.StringNames[(int)((byte)this.TaskType)]);
		if (this.ShowTaskTimer && this.TimerStarted == NormalPlayerTask.TimerState.Started)
		{
			sb.Append(" (");
			sb.Append((int)this.TaskTimer);
			sb.Append("s)");
		}
		else if (this.ShowTaskStep)
		{
			sb.Append(" (");
			sb.Append(this.taskStep);
			sb.Append("/");
			sb.Append(this.MaxStep);
			sb.Append(")");
		}
		if (flag)
		{
			sb.Append("[]");
		}
		sb.AppendLine();
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x00008F3D File Offset: 0x0000713D
	private bool ShouldYellowText()
	{
		return (this.TaskType == TaskTypes.FuelEngines && this.Data[1] > 0) || this.taskStep > 0 || this.TimerStarted > NormalPlayerTask.TimerState.NotStarted;
	}

	// Token: 0x04000B0B RID: 2827
	public int taskStep;

	// Token: 0x04000B0C RID: 2828
	public int MaxStep;

	// Token: 0x04000B0D RID: 2829
	public bool ShowTaskStep = true;

	// Token: 0x04000B0E RID: 2830
	public bool ShowTaskTimer;

	// Token: 0x04000B0F RID: 2831
	public NormalPlayerTask.TimerState TimerStarted;

	// Token: 0x04000B10 RID: 2832
	public float TaskTimer;

	// Token: 0x04000B11 RID: 2833
	public byte[] Data;

	// Token: 0x04000B12 RID: 2834
	public ArrowBehaviour Arrow;

	// Token: 0x02000215 RID: 533
	public enum TimerState
	{
		// Token: 0x04000B14 RID: 2836
		NotStarted,
		// Token: 0x04000B15 RID: 2837
		Started,
		// Token: 0x04000B16 RID: 2838
		Finished
	}
}
