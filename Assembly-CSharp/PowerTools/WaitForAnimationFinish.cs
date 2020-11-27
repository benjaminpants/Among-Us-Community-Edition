using System;
using System.Collections;
using UnityEngine;

namespace PowerTools
{
	// Token: 0x0200023E RID: 574
	public class WaitForAnimationFinish : IEnumerator
	{
		// Token: 0x06000C58 RID: 3160 RVA: 0x0000966B File Offset: 0x0000786B
		public WaitForAnimationFinish(SpriteAnim animator, AnimationClip clip)
		{
			this.animator = animator;
			this.clip = clip;
			this.animator.Play(this.clip, 1f);
		}

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x06000C59 RID: 3161 RVA: 0x000034DF File Offset: 0x000016DF
		public object Current
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x0003C154 File Offset: 0x0003A354
		public bool MoveNext()
		{
			if (this.first)
			{
				this.first = false;
				return true;
			}
			bool result;
			try
			{
				result = this.animator.IsPlaying(this.clip);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x0000969E File Offset: 0x0000789E
		public void Reset()
		{
			this.first = true;
			this.animator.Play(this.clip, 1f);
		}

		// Token: 0x04000BE4 RID: 3044
		private SpriteAnim animator;

		// Token: 0x04000BE5 RID: 3045
		private AnimationClip clip;

		// Token: 0x04000BE6 RID: 3046
		private bool first = true;
	}
}
