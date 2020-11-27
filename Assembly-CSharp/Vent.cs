using System;
using PowerTools;
using UnityEngine;

// Token: 0x020001F7 RID: 503
public class Vent : MonoBehaviour, IUsable
{
	// Token: 0x1700019C RID: 412
	// (get) Token: 0x06000ACE RID: 2766 RVA: 0x0000874C File Offset: 0x0000694C
	public float UsableDistance
	{
		get
		{
			return 0.75f;
		}
	}

	// Token: 0x1700019D RID: 413
	// (get) Token: 0x06000ACF RID: 2767 RVA: 0x0000640F File Offset: 0x0000460F
	public float PercentCool
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x00036C00 File Offset: 0x00034E00
	private void Start()
	{
		this.SetButtons(false);
		this.myRend = base.GetComponent<SpriteRenderer>();
		byte ventMode = PlayerControl.GameOptions.VentMode;
		if (PlayerControl.GameOptions.Venting == 2 && !GameData.Instance.GetPlayerById(PlayerControl.LocalPlayer.PlayerId).IsImpostor)
		{
			this.Left = null;
			this.Right = null;
			return;
		}
		if (ventMode == 1)
		{
			Vent[] array = UnityEngine.Object.FindObjectsOfType<Vent>();
			int num = array.IndexOf(this);
			int num2 = num + 1;
			int num3 = num - 1;
			this.Left = null;
			this.Right = null;
			if (num2 > array.Length)
			{
				this.Left = null;
			}
			else
			{
				this.Left = array[num2];
			}
			if (num3 == -1)
			{
				this.Right = null;
				return;
			}
			this.Right = array[num3];
			return;
		}
		else
		{
			if (ventMode == 2)
			{
				this.Left = null;
				return;
			}
			if (ventMode == 3)
			{
				this.Left = null;
				this.Right = null;
			}
			return;
		}
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x00036CD4 File Offset: 0x00034ED4
	public void SetButtons(bool enabled)
	{
		Vent[] array = new Vent[]
		{
			this.Right,
			this.Left
		};
		for (int i = 0; i < this.Buttons.Length; i++)
		{
			ButtonBehavior buttonBehavior = this.Buttons[i];
			if (enabled)
			{
				Vent vent = array[i];
				if (vent)
				{
					buttonBehavior.gameObject.SetActive(true);
					Vector3 localPosition = (vent.transform.position - base.transform.position).normalized * 0.7f;
					localPosition.y -= 0.08f;
					localPosition.z = -10f;
					buttonBehavior.transform.localPosition = localPosition;
					buttonBehavior.transform.LookAt2d(vent.transform);
				}
				else
				{
					buttonBehavior.gameObject.SetActive(false);
				}
			}
			else
			{
				buttonBehavior.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x00036DC0 File Offset: 0x00034FC0
	public float CanUse(GameData.PlayerInfo pc, out bool canUse, out bool couldUse)
	{
		float num = float.MaxValue;
		PlayerControl @object = pc.Object;
		couldUse = ((pc.IsImpostor || PlayerControl.GameOptions.Venting != 0) && PlayerControl.GameOptions.Venting != 3 && !pc.IsDead && (@object.CanMove || @object.inVent));
		canUse = couldUse;
		if (canUse)
		{
			num = Vector2.Distance(@object.GetTruePosition(), base.transform.position);
			canUse &= (num <= this.UsableDistance);
		}
		return num;
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x00036E50 File Offset: 0x00035050
	public void SetOutline(bool on, bool mainTarget)
	{
		this.myRend.material.SetFloat("_Outline", (float)(on ? 1 : 0));
		this.myRend.material.SetColor("_OutlineColor", Color.red);
		this.myRend.material.SetColor("_AddColor", mainTarget ? Color.red : Color.clear);
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x00036EB8 File Offset: 0x000350B8
	public void ClickRight()
	{
		if (this.Right)
		{
			Vent.DoMove(this.Right.transform.position - Vent.CollOffset);
			this.SetButtons(false);
			this.Right.SetButtons(true);
		}
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x00036F04 File Offset: 0x00035104
	public void ClickLeft()
	{
		if (this.Left)
		{
			Vent.DoMove(this.Left.transform.position - Vent.CollOffset);
			this.SetButtons(false);
			this.Left.SetButtons(true);
		}
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x00036F50 File Offset: 0x00035150
	private static void DoMove(Vector3 pos)
	{
		PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(pos);
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.VentMoveSounds.Random<AudioClip>(), false, 1f).pitch = FloatRange.Next(0.8f, 1.2f);
		}
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x00036FAC File Offset: 0x000351AC
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
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.StopSound(localPlayer.VentEnterSound);
			SoundManager.Instance.PlaySound(localPlayer.VentEnterSound, false, 1f).pitch = FloatRange.Next(0.8f, 1.2f);
		}
		if (localPlayer.inVent)
		{
			localPlayer.MyPhysics.RpcExitVent(this.Id);
			this.SetButtons(false);
			return;
		}
		localPlayer.MyPhysics.RpcEnterVent(this.Id);
		this.SetButtons(true);
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x00008753 File Offset: 0x00006953
	internal void EnterVent()
	{
		base.GetComponent<SpriteAnim>().Play(this.EnterVentAnim, 1f);
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x0000876B File Offset: 0x0000696B
	internal void ExitVent()
	{
		base.GetComponent<SpriteAnim>().Play(this.ExitVentAnim, 1f);
	}

	// Token: 0x04000A83 RID: 2691
	public int Id;

	// Token: 0x04000A84 RID: 2692
	public Vent Left;

	// Token: 0x04000A85 RID: 2693
	public Vent Right;

	// Token: 0x04000A86 RID: 2694
	public ButtonBehavior[] Buttons;

	// Token: 0x04000A87 RID: 2695
	public AnimationClip EnterVentAnim;

	// Token: 0x04000A88 RID: 2696
	public AnimationClip ExitVentAnim;

	// Token: 0x04000A89 RID: 2697
	private static readonly Vector3 CollOffset = new Vector3(0f, -0.3636057f, 0f);

	// Token: 0x04000A8A RID: 2698
	private SpriteRenderer myRend;
}
