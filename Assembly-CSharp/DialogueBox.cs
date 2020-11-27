using System;
using UnityEngine;

// Token: 0x020000C0 RID: 192
public class DialogueBox : MonoBehaviour
{
	// Token: 0x0600040C RID: 1036 RVA: 0x00019D20 File Offset: 0x00017F20
	public void Show(string dialogue)
	{
		this.target.Text = dialogue;
		if (Minigame.Instance)
		{
			Minigame.Instance.Close();
			Minigame.Instance.Close();
		}
		PlayerControl.LocalPlayer.moveable = false;
		PlayerControl.LocalPlayer.NetTransform.Halt();
		base.gameObject.SetActive(true);
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x000049BE File Offset: 0x00002BBE
	public void Hide()
	{
		base.gameObject.SetActive(false);
		PlayerControl.LocalPlayer.moveable = true;
		Camera.main.GetComponent<FollowerCamera>().Locked = false;
	}

	// Token: 0x040003FF RID: 1023
	public TextRenderer target;
}
