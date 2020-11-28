using System;
using PowerTools;
using UnityEngine;

// Token: 0x020001A0 RID: 416
public class SkinLayer : MonoBehaviour
{
	// Token: 0x1700014C RID: 332
	// (set) Token: 0x060008C8 RID: 2248 RVA: 0x00007623 File Offset: 0x00005823
	public bool Flipped
	{
		set
		{
			this.layer.flipX = value;
		}
	}

	// Token: 0x1700014D RID: 333
	// (set) Token: 0x060008C9 RID: 2249 RVA: 0x00007633 File Offset: 0x00005833
	public bool Visible
	{
		set
		{
			this.layer.enabled = value;
		}
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x0002F6F0 File Offset: 0x0002D8F0
	public void SetRun()
	{
		bool flag = !this.skin || !this.animator;
		bool flag2 = flag;
		if (flag2)
		{
			this.SetGhost();
		}
		else
		{
			bool flag3 = !this.animator.IsPlaying(this.skin.RunAnim);
			bool flag4 = flag3;
			if (flag4)
			{
				this.animator.Play(this.skin.RunAnim, 1f);
			}
		}
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x0002F76C File Offset: 0x0002D96C
	public void SetSpawn(float time = 0f)
	{
		bool flag = !this.skin || !this.animator;
		bool flag2 = flag;
		if (flag2)
		{
			this.SetGhost();
		}
		else
		{
			this.animator.Play(this.skin.SpawnAnim, 1f);
			this.animator.Time = time;
		}
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x0002F7D4 File Offset: 0x0002D9D4
	public void SetExitVent()
	{
		bool flag = !this.skin || !this.animator;
		bool flag2 = flag;
		if (flag2)
		{
			this.SetGhost();
		}
		else
		{
			this.animator.Play(this.skin.ExitVentAnim, 1f);
		}
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x0002F830 File Offset: 0x0002DA30
	public void SetEnterVent()
	{
		bool flag = !this.skin || !this.animator;
		bool flag2 = flag;
		if (flag2)
		{
			this.SetGhost();
		}
		else
		{
			this.animator.Play(this.skin.EnterVentAnim, 1f);
		}
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x0002F88C File Offset: 0x0002DA8C
	public void SetIdle()
	{
		bool flag = !this.skin || !this.animator;
		bool flag2 = flag;
		if (flag2)
		{
			this.SetGhost();
		}
		else
		{
			bool flag3 = !this.animator.IsPlaying(this.skin.IdleAnim);
			bool flag4 = flag3;
			if (flag4)
			{
				this.animator.Play(this.skin.IdleAnim, 1f);
			}
		}
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x0002F908 File Offset: 0x0002DB08
	public void SetGhost()
	{
		bool flag = !this.animator;
		bool flag2 = !flag;
		if (flag2)
		{
			this.animator.Stop();
			this.layer.sprite = null;
		}
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x0002F948 File Offset: 0x0002DB48
	internal void SetSkin(uint skinId)
	{
		this.skin = DestroyableSingleton<HatManager>.Instance.GetSkinById(skinId);
		this.SetIdle();
	}

	// Token: 0x04000885 RID: 2181
	public SpriteRenderer layer;

	// Token: 0x04000886 RID: 2182
	public SpriteAnim animator;

	// Token: 0x04000887 RID: 2183
	public SkinData skin;
}
