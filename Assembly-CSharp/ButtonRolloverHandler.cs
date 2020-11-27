using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000F5 RID: 245
public class ButtonRolloverHandler : MonoBehaviour
{
	// Token: 0x0600053B RID: 1339 RVA: 0x00005400 File Offset: 0x00003600
	public void Start()
	{
		PassiveButton component = base.GetComponent<PassiveButton>();
		component.OnMouseOver.AddListener(new UnityAction(this.DoMouseOver));
		component.OnMouseOut.AddListener(new UnityAction(this.DoMouseOut));
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x00005435 File Offset: 0x00003635
	public void DoMouseOver()
	{
		this.Target.color = this.OverColor;
		if (this.HoverSound)
		{
			SoundManager.Instance.PlaySound(this.HoverSound, false, 1f);
		}
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x0000546C File Offset: 0x0000366C
	public void DoMouseOut()
	{
		this.Target.color = this.OutColor;
	}

	// Token: 0x04000509 RID: 1289
	public SpriteRenderer Target;

	// Token: 0x0400050A RID: 1290
	public Color OverColor = Color.green;

	// Token: 0x0400050B RID: 1291
	public Color OutColor = Color.white;

	// Token: 0x0400050C RID: 1292
	public AudioClip HoverSound;
}
