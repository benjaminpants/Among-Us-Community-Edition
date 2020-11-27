using System;
using UnityEngine;

// Token: 0x020001BF RID: 447
public abstract class OptionBehaviour : MonoBehaviour
{
	// Token: 0x060009B2 RID: 2482 RVA: 0x00002732 File Offset: 0x00000932
	public virtual float GetFloat()
	{
		throw new NotImplementedException();
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x00002732 File Offset: 0x00000932
	public virtual int GetInt()
	{
		throw new NotImplementedException();
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x00002732 File Offset: 0x00000932
	public virtual bool GetBool()
	{
		throw new NotImplementedException();
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x00033220 File Offset: 0x00031420
	public void SetAsPlayer()
	{
		PassiveButton[] componentsInChildren = base.GetComponentsInChildren<PassiveButton>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x04000953 RID: 2387
	public string Title;

	// Token: 0x04000954 RID: 2388
	public Action<OptionBehaviour> OnValueChanged;
}
