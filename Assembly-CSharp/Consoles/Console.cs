using System;
using Assets.CoreScripts;
using UnityEngine;

// Token: 0x020001CB RID: 459
public class Console : MonoBehaviour, IUsable
{
	// Token: 0x1700017E RID: 382
	// (get) Token: 0x060009F1 RID: 2545 RVA: 0x00008143 File Offset: 0x00006343
	public float UsableDistance
	{
		get
		{
			return this.usableDistance;
		}
	}

	// Token: 0x1700017F RID: 383
	// (get) Token: 0x060009F2 RID: 2546 RVA: 0x0000640F File Offset: 0x0000460F
	public float PercentCool
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x0003430C File Offset: 0x0003250C
	public void SetOutline(bool on, bool mainTarget)
	{
		if (this.Image)
		{
			this.Image.material.SetFloat("_Outline", (float)(on ? 1 : 0));
			this.Image.material.SetColor("_OutlineColor", Color.yellow);
			this.Image.material.SetColor("_AddColor", mainTarget ? Color.yellow : Color.clear);
		}
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x00034384 File Offset: 0x00032584
	public float CanUse(GameData.PlayerInfo pc, out bool canUse, out bool couldUse)
	{
		float num = float.MaxValue;
		PlayerControl @object = pc.Object;
		couldUse = ((!pc.IsDead || (PlayerControl.GameOptions.GhostsDoTasks && !this.GhostsIgnored)) && @object.CanMove && !this.RestrictedByBeingImposter(pc, @object) && (!this.onlyFromBelow || @object.transform.position.y < base.transform.position.y) && this.FindTask(@object));
		canUse = couldUse;
		if (canUse)
		{
			num = Vector2.Distance(@object.GetTruePosition(), base.transform.position);
			canUse &= (num <= this.UsableDistance);
		}
		return num;
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x00034440 File Offset: 0x00032640
	private PlayerTask FindTask(PlayerControl pc)
	{
		for (int i = 0; i < pc.myTasks.Count; i++)
		{
			PlayerTask playerTask = pc.myTasks[i];
			if (!playerTask.IsComplete && playerTask.ValidConsole(this))
			{
				return playerTask;
			}
		}
		return null;
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x00034484 File Offset: 0x00032684
	public void Use()
	{
		bool flag;
		bool flag2;
		this.CanUse(PlayerControl.LocalPlayer.Data, out flag, out flag2);
		if (!flag)
		{
			return;
		}
		PlayerControl localPlayer = PlayerControl.LocalPlayer;
		PlayerTask playerTask = this.FindTask(localPlayer);
		if (playerTask.MinigamePrefab)
		{
			Minigame minigame = UnityEngine.Object.Instantiate<Minigame>(playerTask.MinigamePrefab);
			minigame.transform.SetParent(Camera.main.transform, false);
			minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
			minigame.Console = this;
			minigame.Begin(playerTask);
			DestroyableSingleton<Telemetry>.Instance.WriteUse(localPlayer.PlayerId, playerTask.TaskType, base.transform.position);
		}
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x00034534 File Offset: 0x00032734
	public bool RestrictedByBeingImposter(GameData.PlayerInfo pc, PlayerControl @object)
	{
		try
		{
			switch (this.FindTask(@object).TaskType)
			{
			case global::TaskTypes.ResetReactor:
				return false;
			case global::TaskTypes.FixLights:
				return false;
			case global::TaskTypes.FixComms:
				return false;
			case global::TaskTypes.RestoreOxy:
				return false;
			}
		}
		catch (Exception)
		{
		}
		return pc.IsImpostor;
	}

	// Token: 0x04000995 RID: 2453
	public float usableDistance = 1f;

	// Token: 0x04000996 RID: 2454
	public int ConsoleId;

	// Token: 0x04000997 RID: 2455
	public bool onlyFromBelow;

	// Token: 0x04000998 RID: 2456
	public bool GhostsIgnored;

	// Token: 0x04000999 RID: 2457
	public SystemTypes Room;

	// Token: 0x0400099A RID: 2458
	public TaskTypes[] TaskTypes;

	// Token: 0x0400099B RID: 2459
	public TaskSet[] ValidTasks;

	// Token: 0x0400099C RID: 2460
	public SpriteRenderer Image;
}
