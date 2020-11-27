using System;
using UnityEngine;

// Token: 0x020001FB RID: 507
public interface ISoundPlayer
{
	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x06000AE8 RID: 2792
	// (set) Token: 0x06000AE9 RID: 2793
	string Name { get; set; }

	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x06000AEA RID: 2794
	// (set) Token: 0x06000AEB RID: 2795
	AudioSource Player { get; set; }

	// Token: 0x06000AEC RID: 2796
	void Update(float dt);
}
