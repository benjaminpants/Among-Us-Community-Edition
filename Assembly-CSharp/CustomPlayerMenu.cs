using System;
using UnityEngine;

// Token: 0x02000145 RID: 325
public class CustomPlayerMenu : MonoBehaviour
{
	// Token: 0x060006CE RID: 1742 RVA: 0x000063C8 File Offset: 0x000045C8
	public void Start()
	{
		PlayerControl.LocalPlayer.moveable = false;
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x000281B4 File Offset: 0x000263B4
	public void OpenTab(GameObject tab)
	{
		for (int i = 0; i < this.Tabs.Length; i++)
		{
			TabButton tabButton = this.Tabs[i];
			if (tabButton.Tab == tab)
			{
				tabButton.Tab.SetActive(true);
				tabButton.Button.sprite = this.SelectedColor;
			}
			else
			{
				tabButton.Tab.SetActive(false);
				tabButton.Button.sprite = this.NormalColor;
			}
		}
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x000063D5 File Offset: 0x000045D5
	public void Close(bool canMove)
	{
		PlayerControl.LocalPlayer.moveable = canMove;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x04000690 RID: 1680
	public TabButton[] Tabs;

	// Token: 0x04000691 RID: 1681
	public Sprite NormalColor;

	// Token: 0x04000692 RID: 1682
	public Sprite SelectedColor;
}
