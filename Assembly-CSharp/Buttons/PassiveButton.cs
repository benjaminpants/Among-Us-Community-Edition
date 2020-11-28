using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020000F6 RID: 246
public class PassiveButton : MonoBehaviour
{
	// Token: 0x0600053F RID: 1343 RVA: 0x0000549D File Offset: 0x0000369D
	public void Start()
	{
		DestroyableSingleton<PassiveButtonManager>.Instance.RegisterOne(this);
		if (this.Colliders == null || this.Colliders.Length == 0)
		{
			this.Colliders = base.GetComponents<Collider2D>();
		}
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x000054C7 File Offset: 0x000036C7
	public void DoClick()
	{
		if (this.ClickSound)
		{
			SoundManager.Instance.PlaySound(this.ClickSound, false, 1f);
		}
		this.OnClick.Invoke();
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x000054F8 File Offset: 0x000036F8
	public void OnDestroy()
	{
		if (DestroyableSingleton<PassiveButtonManager>.InstanceExists)
		{
			DestroyableSingleton<PassiveButtonManager>.Instance.RemoveOne(this);
		}
	}

	// Token: 0x0400050D RID: 1293
	public bool OnUp = true;

	// Token: 0x0400050E RID: 1294
	public bool OnDown;

	// Token: 0x0400050F RID: 1295
	public Button.ButtonClickedEvent OnClick = new Button.ButtonClickedEvent();

	// Token: 0x04000510 RID: 1296
	public AudioClip ClickSound;

	// Token: 0x04000511 RID: 1297
	public UnityEvent OnMouseOver;

	// Token: 0x04000512 RID: 1298
	public UnityEvent OnMouseOut;

	// Token: 0x04000513 RID: 1299
	public Collider2D[] Colliders;
}
