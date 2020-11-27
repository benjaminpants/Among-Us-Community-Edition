using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x02000217 RID: 535
public abstract class PlayerTask : MonoBehaviour
{
	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x06000B99 RID: 2969 RVA: 0x00009035 File Offset: 0x00007235
	// (set) Token: 0x06000B9A RID: 2970 RVA: 0x0000903D File Offset: 0x0000723D
	public int Index { get; internal set; }

	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x06000B9B RID: 2971 RVA: 0x00009046 File Offset: 0x00007246
	// (set) Token: 0x06000B9C RID: 2972 RVA: 0x0000904E File Offset: 0x0000724E
	public uint Id { get; internal set; }

	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x06000B9D RID: 2973 RVA: 0x00009057 File Offset: 0x00007257
	// (set) Token: 0x06000B9E RID: 2974 RVA: 0x0000905F File Offset: 0x0000725F
	public PlayerControl Owner { get; internal set; }

	// Token: 0x170001C5 RID: 453
	// (get) Token: 0x06000B9F RID: 2975
	public abstract int TaskStep { get; }

	// Token: 0x170001C6 RID: 454
	// (get) Token: 0x06000BA0 RID: 2976
	public abstract bool IsComplete { get; }

	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x06000BA1 RID: 2977 RVA: 0x00009068 File Offset: 0x00007268
	public Vector2 Location
	{
		get
		{
			this.LocationDirty = false;
			return this.FindObjectPos().transform.position;
		}
	}

	// Token: 0x06000BA2 RID: 2978
	public abstract void Initialize();

	// Token: 0x06000BA3 RID: 2979 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnRemove()
	{
	}

	// Token: 0x06000BA4 RID: 2980
	public abstract bool ValidConsole(global::Console console);

	// Token: 0x06000BA5 RID: 2981
	public abstract void Complete();

	// Token: 0x06000BA6 RID: 2982
	public abstract void AppendTaskText(StringBuilder sb);

	// Token: 0x06000BA7 RID: 2983 RVA: 0x00009086 File Offset: 0x00007286
	internal static bool TaskIsEmergency(PlayerTask arg)
	{
		return arg is NoOxyTask || arg is HudOverrideTask || arg is ReactorTask || arg is ElectricTask;
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x000398F4 File Offset: 0x00037AF4
	protected List<global::Console> FindConsoles()
	{
		List<global::Console> list = new List<global::Console>();
		global::Console[] allConsoles = ShipStatus.Instance.AllConsoles;
		for (int i = 0; i < allConsoles.Length; i++)
		{
			if (this.ValidConsole(allConsoles[i]))
			{
				list.Add(allConsoles[i]);
			}
		}
		return list;
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x00039938 File Offset: 0x00037B38
	public static bool PlayerHasHudTask(PlayerControl localPlayer)
	{
		if (!localPlayer)
		{
			return true;
		}
		for (int i = 0; i < localPlayer.myTasks.Count; i++)
		{
			if (localPlayer.myTasks[i] is HudOverrideTask)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x0003997C File Offset: 0x00037B7C
	protected List<Vector2> FindObjectsPos()
	{
		List<Vector2> list = new List<Vector2>();
		global::Console[] allConsoles = ShipStatus.Instance.AllConsoles;
		for (int i = 0; i < allConsoles.Length; i++)
		{
			if (this.ValidConsole(allConsoles[i]))
			{
				list.Add(allConsoles[i].transform.position);
			}
		}
		return list;
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x000399CC File Offset: 0x00037BCC
	protected global::Console FindSpecialConsole(Func<global::Console, bool> func)
	{
		global::Console[] allConsoles = ShipStatus.Instance.AllConsoles;
		for (int i = 0; i < allConsoles.Length; i++)
		{
			if (func(allConsoles[i]))
			{
				return allConsoles[i];
			}
		}
		return null;
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x00039A04 File Offset: 0x00037C04
	protected global::Console FindObjectPos()
	{
		global::Console[] allConsoles = ShipStatus.Instance.AllConsoles;
		for (int i = 0; i < allConsoles.Length; i++)
		{
			if (this.ValidConsole(allConsoles[i]))
			{
				return allConsoles[i];
			}
		}
		return null;
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x00039A3C File Offset: 0x00037C3C
	protected static bool AllTasksCompleted(PlayerControl player)
	{
		for (int i = 0; i < player.myTasks.Count; i++)
		{
			PlayerTask playerTask = player.myTasks[i];
			if (playerTask is NormalPlayerTask && !playerTask.IsComplete)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04000B1E RID: 2846
	public SystemTypes StartAt;

	// Token: 0x04000B1F RID: 2847
	public TaskTypes TaskType;

	// Token: 0x04000B20 RID: 2848
	public Minigame MinigamePrefab;

	// Token: 0x04000B21 RID: 2849
	public bool HasLocation;

	// Token: 0x04000B22 RID: 2850
	public bool LocationDirty = true;
}
