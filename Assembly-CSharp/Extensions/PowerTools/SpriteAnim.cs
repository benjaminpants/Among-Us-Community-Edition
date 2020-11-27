using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PowerTools
{
	// Token: 0x02000239 RID: 569
	[RequireComponent(typeof(Animator))]
	[DisallowMultipleComponent]
	public class SpriteAnim : SpriteAnimEventHandler
	{
		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000C26 RID: 3110 RVA: 0x000094ED File Offset: 0x000076ED
		public bool Playing
		{
			get
			{
				return this.IsPlaying((AnimationClip)null);
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000C27 RID: 3111 RVA: 0x000094F6 File Offset: 0x000076F6
		// (set) Token: 0x06000C28 RID: 3112 RVA: 0x000094FE File Offset: 0x000076FE
		public bool Paused
		{
			get
			{
				return this.IsPaused();
			}
			set
			{
				if (value)
				{
					this.Pause();
					return;
				}
				this.Resume();
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000C29 RID: 3113 RVA: 0x00009510 File Offset: 0x00007710
		// (set) Token: 0x06000C2A RID: 3114 RVA: 0x00009518 File Offset: 0x00007718
		public float Speed
		{
			get
			{
				return this.m_speed;
			}
			set
			{
				this.SetSpeed(value);
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000C2B RID: 3115 RVA: 0x00009521 File Offset: 0x00007721
		// (set) Token: 0x06000C2C RID: 3116 RVA: 0x00009529 File Offset: 0x00007729
		public float Time
		{
			get
			{
				return this.GetTime();
			}
			set
			{
				this.SetTime(value);
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06000C2D RID: 3117 RVA: 0x00009532 File Offset: 0x00007732
		// (set) Token: 0x06000C2E RID: 3118 RVA: 0x0000953A File Offset: 0x0000773A
		public float NormalizedTime
		{
			get
			{
				return this.GetNormalisedTime();
			}
			set
			{
				this.SetNormalizedTime(value);
			}
		}

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x06000C2F RID: 3119 RVA: 0x00009543 File Offset: 0x00007743
		public AnimationClip Clip
		{
			get
			{
				return this.m_currAnim;
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x06000C30 RID: 3120 RVA: 0x0000954B File Offset: 0x0000774B
		public string ClipName
		{
			get
			{
				if (!(this.m_currAnim != null))
				{
					return string.Empty;
				}
				return this.m_currAnim.name;
			}
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x0003B698 File Offset: 0x00039898
		public void Play(AnimationClip anim = null, float speed = 1f)
		{
			if (anim == null)
			{
				anim = (this.m_currAnim ? this.m_currAnim : this.m_defaultAnim);
				if (anim == null)
				{
					return;
				}
			}
			if (!this.m_animator.enabled)
			{
				this.m_animator.enabled = true;
			}
			if (this.m_nodes != null)
			{
				this.m_nodes.Reset();
			}
			this.m_clipPairList[0] = new KeyValuePair<AnimationClip, AnimationClip>(this.m_clipPairList[0].Key, anim);
			this.m_controller.ApplyOverrides(this.m_clipPairList);
			this.m_animator.Update(0f);
			this.m_animator.Play(SpriteAnim.STATE_NAME, 0, 0f);
			this.m_speed = Mathf.Max(0f, speed);
			this.m_animator.speed = this.m_speed;
			this.m_currAnim = anim;
			this.m_animator.Update(0f);
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x0000956C File Offset: 0x0000776C
		public void Stop()
		{
			this.m_animator.enabled = false;
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x0000957A File Offset: 0x0000777A
		public void Pause()
		{
			this.m_animator.speed = 0f;
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x0000958C File Offset: 0x0000778C
		public void Resume()
		{
			this.m_animator.speed = this.m_speed;
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x00009543 File Offset: 0x00007743
		public AnimationClip GetCurrentAnimation()
		{
			return this.m_currAnim;
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x0003B7A0 File Offset: 0x000399A0
		public bool IsPlaying(AnimationClip clip = null)
		{
			return (clip == null || this.m_currAnim == clip) && this.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x0003B7E4 File Offset: 0x000399E4
		public bool IsPlaying(string animName)
		{
			return !(this.m_currAnim == null) && this.m_currAnim.name == animName && this.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
		}

		// Token: 0x06000C38 RID: 3128 RVA: 0x0000959F File Offset: 0x0000779F
		public bool IsPaused()
		{
			return !(this.m_currAnim == null) && this.m_animator.speed == 0f;
		}

		// Token: 0x06000C39 RID: 3129 RVA: 0x000095C3 File Offset: 0x000077C3
		public void SetSpeed(float speed)
		{
			this.m_speed = Mathf.Max(0f, speed);
			this.m_animator.speed = this.m_speed;
		}

		// Token: 0x06000C3A RID: 3130 RVA: 0x00009510 File Offset: 0x00007710
		public float GetSpeed()
		{
			return this.m_speed;
		}

		// Token: 0x06000C3B RID: 3131 RVA: 0x0003B834 File Offset: 0x00039A34
		public float GetTime()
		{
			if (this.m_currAnim != null)
			{
				return this.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime * this.m_currAnim.length;
			}
			return 0f;
		}

		// Token: 0x06000C3C RID: 3132 RVA: 0x000095E7 File Offset: 0x000077E7
		public void SetTime(float time)
		{
			if (this.m_currAnim == null || this.m_currAnim.length <= 0f)
			{
				return;
			}
			this.SetNormalizedTime(time / this.m_currAnim.length);
		}

		// Token: 0x06000C3D RID: 3133 RVA: 0x0003B878 File Offset: 0x00039A78
		public float GetNormalisedTime()
		{
			if (this.m_currAnim != null)
			{
				return this.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			}
			return 0f;
		}

		// Token: 0x06000C3E RID: 3134 RVA: 0x0003B8B0 File Offset: 0x00039AB0
		public void SetNormalizedTime(float ratio)
		{
			if (this.m_currAnim == null)
			{
				return;
			}
			this.m_animator.Play(SpriteAnim.STATE_NAME, 0, this.m_currAnim.isLooping ? Mathf.Repeat(ratio, 1f) : Mathf.Clamp01(ratio));
		}

		// Token: 0x06000C3F RID: 3135 RVA: 0x0003B900 File Offset: 0x00039B00
		private void Awake()
		{
			this.m_controller = new AnimatorOverrideController();
			if (SpriteAnim.m_sharedAnimatorController == null)
			{
				SpriteAnim.m_sharedAnimatorController = Resources.Load<RuntimeAnimatorController>(SpriteAnim.CONTROLLER_PATH);
			}
			this.m_controller.runtimeAnimatorController = SpriteAnim.m_sharedAnimatorController;
			this.m_animator = base.GetComponent<Animator>();
			this.m_animator.runtimeAnimatorController = this.m_controller;
			this.m_controller.GetOverrides(this.m_clipPairList);
			this.Play(this.m_defaultAnim, 1f);
			this.m_nodes = base.GetComponent<SpriteAnimNodes>();
		}

		// Token: 0x06000C40 RID: 3136 RVA: 0x0003B990 File Offset: 0x00039B90
		private void Reset()
		{
			if (base.GetComponent<RectTransform>() == null)
			{
				if (base.GetComponent<Sprite>() == null)
				{
					base.gameObject.AddComponent<SpriteRenderer>();
					return;
				}
			}
			else if (base.GetComponent<Image>() == null)
			{
				base.gameObject.AddComponent<Image>();
			}
		}

		// Token: 0x04000BB7 RID: 2999
		private static readonly string STATE_NAME = "a";

		// Token: 0x04000BB8 RID: 3000
		private static readonly string CONTROLLER_PATH = "SpriteAnimController";

		// Token: 0x04000BB9 RID: 3001
		[SerializeField]
		private AnimationClip m_defaultAnim;

		// Token: 0x04000BBA RID: 3002
		private static RuntimeAnimatorController m_sharedAnimatorController = null;

		// Token: 0x04000BBB RID: 3003
		private Animator m_animator;

		// Token: 0x04000BBC RID: 3004
		private AnimatorOverrideController m_controller;

		// Token: 0x04000BBD RID: 3005
		private SpriteAnimNodes m_nodes;

		// Token: 0x04000BBE RID: 3006
		private List<KeyValuePair<AnimationClip, AnimationClip>> m_clipPairList = new List<KeyValuePair<AnimationClip, AnimationClip>>(1);

		// Token: 0x04000BBF RID: 3007
		private AnimationClip m_currAnim;

		// Token: 0x04000BC0 RID: 3008
		private float m_speed = 1f;
	}
}
