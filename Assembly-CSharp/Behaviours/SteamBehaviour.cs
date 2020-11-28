using System;
using System.Collections;
using PowerTools;
using UnityEngine;

// Token: 0x0200008D RID: 141
public class SteamBehaviour : MonoBehaviour
{
	// Token: 0x060002F5 RID: 757 RVA: 0x00003F42 File Offset: 0x00002142
	public void OnEnable()
	{
		base.StartCoroutine(this.Run());
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x00003F51 File Offset: 0x00002151
	private IEnumerator Run()
	{
		for (;;)
		{
			float time = this.PlayRate.Next();
			while (time > 0f)
			{
				time -= Time.deltaTime;
				yield return null;
			}
			this.anim.Play(null, 1f);
			while (this.anim.IsPlaying((AnimationClip)null))
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x040002F6 RID: 758
	public SpriteAnim anim;

	// Token: 0x040002F7 RID: 759
	public FloatRange PlayRate = new FloatRange(0.5f, 1f);
}
