using System;
using UnityEngine;

// Token: 0x0200019F RID: 415
[CreateAssetMenu]
public class SkinData : ScriptableObject, IBuyable
{
	// Token: 0x1700014B RID: 331
	// (get) Token: 0x060008C6 RID: 2246 RVA: 0x00007616 File Offset: 0x00005816
	public string ProdId
	{
		get
		{
			return this.RelatedHat.ProductId;
		}
	}

	// Token: 0x04000874 RID: 2164
	public Sprite IdleFrame;

	// Token: 0x04000875 RID: 2165
	public AnimationClip IdleAnim;

	// Token: 0x04000876 RID: 2166
	public AnimationClip RunAnim;

	// Token: 0x04000877 RID: 2167
	public AnimationClip EnterVentAnim;

	// Token: 0x04000878 RID: 2168
	public AnimationClip ExitVentAnim;

	// Token: 0x04000879 RID: 2169
	public AnimationClip KillTongueImpostor;

	// Token: 0x0400087A RID: 2170
	public AnimationClip KillTongueVictim;

	// Token: 0x0400087B RID: 2171
	public AnimationClip KillShootImpostor;

	// Token: 0x0400087C RID: 2172
	public AnimationClip KillShootVictim;

	// Token: 0x0400087D RID: 2173
	public AnimationClip KillStabVictim;

	// Token: 0x0400087E RID: 2174
	public AnimationClip KillNeckVictim;

	// Token: 0x0400087F RID: 2175
	public Sprite EjectFrame;

	// Token: 0x04000880 RID: 2176
	public AnimationClip SpawnAnim;

	// Token: 0x04000881 RID: 2177
	public bool Free;

	// Token: 0x04000882 RID: 2178
	public HatBehaviour RelatedHat;

	// Token: 0x04000883 RID: 2179
	public string StoreName;

	// Token: 0x04000884 RID: 2180
	public int Order;
}
