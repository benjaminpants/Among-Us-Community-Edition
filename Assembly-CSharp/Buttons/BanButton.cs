using System;
using UnityEngine;

// Token: 0x020000B8 RID: 184
public class BanButton : MonoBehaviour
{
	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x060003D7 RID: 983 RVA: 0x0000474C File Offset: 0x0000294C
	// (set) Token: 0x060003D8 RID: 984 RVA: 0x00004754 File Offset: 0x00002954
	public BanMenu Parent { get; set; }

	// Token: 0x060003D9 RID: 985 RVA: 0x0000475D File Offset: 0x0000295D
	public void Select()
	{
		this.Background.color = new Color(1f, 1f, 1f, 1f);
		this.Parent.Select(this.TargetClientId);
	}

	// Token: 0x060003DA RID: 986 RVA: 0x00004794 File Offset: 0x00002994
	public void Unselect()
	{
		this.Background.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
	}

	// Token: 0x040003C8 RID: 968
	public TextRenderer NameText;

	// Token: 0x040003C9 RID: 969
	public SpriteRenderer Background;

	// Token: 0x040003CA RID: 970
	public int TargetClientId;
}
