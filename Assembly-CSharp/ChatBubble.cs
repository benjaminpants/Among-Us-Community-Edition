using System;
using UnityEngine;

// Token: 0x020000BB RID: 187
internal class ChatBubble : PoolableBehavior
{
	// Token: 0x060003E7 RID: 999 RVA: 0x00019140 File Offset: 0x00017340
	public void SetLeft()
	{
		base.transform.localPosition = new Vector3(-3f, 0f, 0f);
		this.ChatFace.flipX = false;
		this.ChatFace.transform.localPosition = new Vector3(0f, 0.07f, 0f);
		this.Xmark.transform.localPosition = new Vector3(-0.15f, -0.13f, -0.0001f);
		this.NameText.transform.localPosition = new Vector3(0.5f, 0.34f, 0f);
		this.NameText.RightAligned = false;
		this.TextArea.transform.localPosition = new Vector3(0.5f, 0.09f, 0f);
		this.TextArea.RightAligned = false;
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x00019220 File Offset: 0x00017420
	public void SetRight()
	{
		base.transform.localPosition = new Vector3(-2.35f, 0f, 0f);
		this.ChatFace.flipX = true;
		this.ChatFace.transform.localPosition = new Vector3(4.75f, 0.07f, 0f);
		this.Xmark.transform.localPosition = new Vector3(0.15f, -0.13f, -0.0001f);
		this.NameText.transform.localPosition = new Vector3(4.35f, 0.34f, 0f);
		this.NameText.RightAligned = true;
		this.TextArea.transform.localPosition = new Vector3(4.35f, 0.09f, 0f);
		this.TextArea.RightAligned = true;
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x00019300 File Offset: 0x00017500
	public void SetName(string playerName, bool isDead, bool isImpostor)
	{
		this.NameText.Text = (playerName ?? "...");
		if (PlayerControl.GameOptions.Gamemode == 1)
		{
			this.NameText.Color = (isImpostor ? Palette.InfectedGreen : Color.white);
		}
		else
		{
			this.NameText.Color = (isImpostor ? Palette.ImpostorRed : Color.white);
		}
		this.NameText.RefreshMesh();
		if (isDead)
		{
			this.Xmark.enabled = true;
			this.Background.color = Palette.HalfWhite;
		}
	}

	// Token: 0x060003EA RID: 1002 RVA: 0x000048BE File Offset: 0x00002ABE
	public override void Reset()
	{
		this.Xmark.enabled = false;
		this.Background.color = Color.white;
	}

	// Token: 0x040003DA RID: 986
	public SpriteRenderer ChatFace;

	// Token: 0x040003DB RID: 987
	public SpriteRenderer Xmark;

	// Token: 0x040003DC RID: 988
	public TextRenderer NameText;

	// Token: 0x040003DD RID: 989
	public TextRenderer TextArea;

	// Token: 0x040003DE RID: 990
	public SpriteRenderer Background;
}
