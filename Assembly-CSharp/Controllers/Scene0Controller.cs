using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class Scene0Controller : SceneController
{
	// Token: 0x0600039C RID: 924 RVA: 0x00004581 File Offset: 0x00002781
	public void OnEnable()
	{
		base.StartCoroutine(this.Run());
	}

	// Token: 0x0600039D RID: 925 RVA: 0x000184E4 File Offset: 0x000166E4
	public void OnDisable()
	{
		for (int i = 0; i < this.ExtraBoys.Length; i++)
		{
			this.ExtraBoys[i].enabled = false;
		}
	}

	// Token: 0x0600039E RID: 926 RVA: 0x00004590 File Offset: 0x00002790
	private IEnumerator Run()
	{
		int lastBoy = 0;
		float start = Time.time;
		for (;;)
		{
			float num = (Time.time - start) / this.Duration;
			int num2 = Mathf.RoundToInt((Mathf.Cos(3.14159274f * num + 3.14159274f) + 1f) / 2f * (float)this.ExtraBoys.Length);
			if (lastBoy < num2)
			{
				base.StartCoroutine(this.PopIn(this.ExtraBoys[lastBoy]));
				lastBoy = num2;
			}
			else if (lastBoy > num2)
			{
				lastBoy = num2;
				base.StartCoroutine(this.PopOut(this.ExtraBoys[lastBoy]));
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600039F RID: 927 RVA: 0x0000459F File Offset: 0x0000279F
	private IEnumerator PopIn(SpriteRenderer boy)
	{
		boy.enabled = true;
		for (float timer = 0f; timer < 0.2f; timer += Time.deltaTime)
		{
			float num = this.PopInCurve.Evaluate(timer / 0.2f);
			boy.transform.localScale = new Vector3(num, num, num);
			yield return null;
		}
		boy.transform.localScale = Vector3.one;
		yield break;
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x000045B5 File Offset: 0x000027B5
	private IEnumerator PopOut(SpriteRenderer boy)
	{
		boy.enabled = true;
		for (float timer = 0f; timer < this.OutDuration; timer += Time.deltaTime)
		{
			float num = this.PopOutCurve.Evaluate(timer / this.OutDuration);
			boy.transform.localScale = new Vector3(num, num, num);
			yield return null;
		}
		boy.transform.localScale = Vector3.one;
		boy.enabled = false;
		yield break;
	}

	// Token: 0x0400038B RID: 907
	public float Duration = 3f;

	// Token: 0x0400038C RID: 908
	public SpriteRenderer[] ExtraBoys;

	// Token: 0x0400038D RID: 909
	public AnimationCurve PopInCurve;

	// Token: 0x0400038E RID: 910
	public AnimationCurve PopOutCurve;

	// Token: 0x0400038F RID: 911
	public float OutDuration = 0.2f;
}
