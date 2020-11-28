using System;
using UnityEngine;

// Token: 0x020000DF RID: 223
public class MapBehaviour : MonoBehaviour
{
	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x060004AD RID: 1197 RVA: 0x00004ED0 File Offset: 0x000030D0
	public bool IsOpen
	{
		get
		{
			return base.isActiveAndEnabled;
		}
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x060004AE RID: 1198 RVA: 0x00004ED8 File Offset: 0x000030D8
	public bool IsOpenStopped
	{
		get
		{
			return this.IsOpen && this.countOverlay.activeSelf;
		}
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x00004EEF File Offset: 0x000030EF
	private void Awake()
	{
		MapBehaviour.Instance = this;
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x00004EF7 File Offset: 0x000030F7
	private void GenericShow()
	{
		base.transform.localScale = Vector3.one;
		base.transform.localPosition = new Vector3(0f, 0f, -25f);
		base.gameObject.SetActive(true);
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x0001B4C4 File Offset: 0x000196C4
	public void ShowInfectedMap()
	{
		if (this.IsOpen)
		{
			this.Close();
			return;
		}
		if (!PlayerControl.LocalPlayer.CanMove)
		{
			return;
		}
		PlayerControl.LocalPlayer.SetPlayerMaterialColors(this.HerePoint);
		this.GenericShow();
		this.infectedOverlay.gameObject.SetActive(true);
		if (PlayerControl.GameOptions.Gamemode == 1)
		{
			this.ColorControl.SetColor(Palette.InfectedGreen);
		}
		else
		{
			this.ColorControl.SetColor(Palette.ImpostorRed);
		}
		this.taskOverlay.Hide();
		DestroyableSingleton<HudManager>.Instance.SetHudActive(false);
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x0001B55C File Offset: 0x0001975C
	public void ShowNormalMap()
	{
		if (this.IsOpen)
		{
			this.Close();
			return;
		}
		if (!PlayerControl.LocalPlayer.CanMove)
		{
			return;
		}
		PlayerControl.LocalPlayer.SetPlayerMaterialColors(this.HerePoint);
		this.GenericShow();
		this.taskOverlay.Show();
		this.ColorControl.SetColor(new Color(0.05f, 0.2f, 1f, 1f));
		DestroyableSingleton<HudManager>.Instance.SetHudActive(false);
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x0001B5D8 File Offset: 0x000197D8
	public void ShowCountOverlay()
	{
		this.GenericShow();
		this.countOverlay.SetActive(true);
		this.taskOverlay.Hide();
		this.HerePoint.enabled = false;
		this.ColorControl.SetColor(Color.green);
		DestroyableSingleton<HudManager>.Instance.SetHudActive(false);
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0001B62C File Offset: 0x0001982C
	public void FixedUpdate()
	{
		if (!ShipStatus.Instance)
		{
			return;
		}
		Vector3 vector = PlayerControl.LocalPlayer.transform.position;
		vector /= ShipStatus.Instance.MapScale;
		vector.z = -1f;
		this.HerePoint.transform.localPosition = vector;
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x0001B684 File Offset: 0x00019884
	public void Close()
	{
		base.gameObject.SetActive(false);
		this.countOverlay.SetActive(false);
		this.infectedOverlay.gameObject.SetActive(false);
		this.taskOverlay.Hide();
		this.HerePoint.enabled = true;
		DestroyableSingleton<HudManager>.Instance.SetHudActive(true);
	}

	// Token: 0x0400048E RID: 1166
	public static MapBehaviour Instance;

	// Token: 0x0400048F RID: 1167
	public AlphaPulse ColorControl;

	// Token: 0x04000490 RID: 1168
	public SpriteRenderer HerePoint;

	// Token: 0x04000491 RID: 1169
	public GameObject countOverlay;

	// Token: 0x04000492 RID: 1170
	public InfectedOverlay infectedOverlay;

	// Token: 0x04000493 RID: 1171
	public MapTaskOverlay taskOverlay;
}
