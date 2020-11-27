using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000202 RID: 514
public class SpinAnimator : MonoBehaviour
{
	// Token: 0x06000B0F RID: 2831 RVA: 0x000089EE File Offset: 0x00006BEE
	private void Update()
	{
		if (this.curState == SpinAnimator.States.Spinning)
		{
			base.transform.Rotate(0f, 0f, this.Speed * Time.deltaTime);
		}
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x00037E98 File Offset: 0x00036098
	public void Appear()
	{
		if (this.curState != SpinAnimator.States.Invisible)
		{
			return;
		}
		this.curState = SpinAnimator.States.Visible;
		base.gameObject.SetActive(true);
		base.StopAllCoroutines();
		base.StartCoroutine(Effects.ScaleIn(base.transform, 0f, 1f, 0.125f));
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x00008A1A File Offset: 0x00006C1A
	public void Disappear()
	{
		if (this.curState == SpinAnimator.States.Invisible)
		{
			return;
		}
		this.curState = SpinAnimator.States.Invisible;
		base.StopAllCoroutines();
		base.StartCoroutine(this.CoDisappear());
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x00008A40 File Offset: 0x00006C40
	private IEnumerator CoDisappear()
	{
		yield return Effects.ScaleIn(base.transform, 1f, 0f, 0.125f);
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x00037EEC File Offset: 0x000360EC
	public void StartPulse()
	{
		if (this.curState == SpinAnimator.States.Pulsing)
		{
			return;
		}
		this.curState = SpinAnimator.States.Pulsing;
		SpriteRenderer component = base.GetComponent<SpriteRenderer>();
		base.StartCoroutine(Effects.CycleColors(component, Color.white, Color.green, 1f, float.MaxValue));
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x00008A4F File Offset: 0x00006C4F
	internal void Play()
	{
		this.curState = SpinAnimator.States.Spinning;
	}

	// Token: 0x04000AB8 RID: 2744
	public float Speed = 60f;

	// Token: 0x04000AB9 RID: 2745
	private SpinAnimator.States curState;

	// Token: 0x02000203 RID: 515
	private enum States
	{
		// Token: 0x04000ABB RID: 2747
		Visible,
		// Token: 0x04000ABC RID: 2748
		Invisible,
		// Token: 0x04000ABD RID: 2749
		Spinning,
		// Token: 0x04000ABE RID: 2750
		Pulsing
	}
}
