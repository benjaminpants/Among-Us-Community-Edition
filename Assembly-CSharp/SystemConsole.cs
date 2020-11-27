using System;
using UnityEngine;

// Token: 0x020001F2 RID: 498
public class SystemConsole : MonoBehaviour, IUsable
{
	// Token: 0x1700019A RID: 410
	// (get) Token: 0x06000AC0 RID: 2752 RVA: 0x00008715 File Offset: 0x00006915
	public float UsableDistance
	{
		get
		{
			return this.usableDistance;
		}
	}

	// Token: 0x1700019B RID: 411
	// (get) Token: 0x06000AC1 RID: 2753 RVA: 0x0000640F File Offset: 0x0000460F
	public float PercentCool
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x0000871D File Offset: 0x0000691D
	public void Start()
	{
		if (this.FreeplayOnly && !DestroyableSingleton<TutorialManager>.InstanceExists)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x000368F4 File Offset: 0x00034AF4
	public void SetOutline(bool on, bool mainTarget)
	{
		if (this.Image)
		{
			this.Image.material.SetFloat("_Outline", (float)(on ? 1 : 0));
			this.Image.material.SetColor("_OutlineColor", Color.white);
			this.Image.material.SetColor("_AddColor", mainTarget ? Color.white : Color.clear);
		}
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x0003696C File Offset: 0x00034B6C
	public float CanUse(GameData.PlayerInfo pc, out bool canUse, out bool couldUse)
	{
		float num = float.MaxValue;
		PlayerControl @object = pc.Object;
		couldUse = (pc.Object.CanMove && (!pc.IsDead || !(this.MinigamePrefab is EmergencyMinigame)));
		canUse = couldUse;
		if (canUse)
		{
			num = Vector2.Distance(@object.GetTruePosition(), base.transform.position);
			canUse &= (num <= this.UsableDistance);
		}
		return num;
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x000369EC File Offset: 0x00034BEC
	public void Use()
	{
		bool flag;
		bool flag2;
		this.CanUse(PlayerControl.LocalPlayer.Data, out flag, out flag2);
		if (!flag)
		{
			return;
		}
		PlayerControl.LocalPlayer.NetTransform.Halt();
		Minigame minigame = UnityEngine.Object.Instantiate<Minigame>(this.MinigamePrefab);
		minigame.transform.SetParent(Camera.main.transform, false);
		minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
		minigame.Begin(null);
	}

	// Token: 0x04000A5E RID: 2654
	public float usableDistance = 1f;

	// Token: 0x04000A5F RID: 2655
	public bool FreeplayOnly;

	// Token: 0x04000A60 RID: 2656
	public SpriteRenderer Image;

	// Token: 0x04000A61 RID: 2657
	public Minigame MinigamePrefab;
}
