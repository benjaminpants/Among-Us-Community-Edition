using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200010B RID: 267
public class TransitionOpen : MonoBehaviour
{
	// Token: 0x060005B5 RID: 1461 RVA: 0x000059DC File Offset: 0x00003BDC
	public void OnEnable()
	{
		base.StartCoroutine(this.AnimateOpen());
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x000059EB File Offset: 0x00003BEB
	public void Close()
	{
		base.StartCoroutine(this.AnimateClose());
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x000059FA File Offset: 0x00003BFA
	private IEnumerator AnimateClose()
	{
		Vector3 vec = default(Vector3);
		for (float t = 0f; t < this.duration; t += Time.deltaTime)
		{
			float t2 = t / this.duration;
			float num = Mathf.SmoothStep(1f, 0f, t2);
			vec.Set(num, num, num);
			base.transform.localScale = vec;
			yield return null;
		}
		vec.Set(0f, 0f, 0f);
		base.transform.localScale = vec;
		this.OnClose.Invoke();
		yield break;
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x00005A09 File Offset: 0x00003C09
	private IEnumerator AnimateOpen()
	{
		Vector3 vec = default(Vector3);
		for (float t = 0f; t < this.duration; t += Time.deltaTime)
		{
			float t2 = t / this.duration;
			float num = Mathf.SmoothStep(0f, 1f, t2);
			vec.Set(num, num, num);
			base.transform.localScale = vec;
			yield return null;
		}
		vec.Set(1f, 1f, 1f);
		base.transform.localScale = vec;
		yield break;
	}

	// Token: 0x04000595 RID: 1429
	public float duration = 0.2f;

	// Token: 0x04000596 RID: 1430
	public Button.ButtonClickedEvent OnClose = new Button.ButtonClickedEvent();
}
