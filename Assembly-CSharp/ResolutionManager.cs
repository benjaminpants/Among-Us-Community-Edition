using System;
using UnityEngine;

// Token: 0x0200004F RID: 79
public static class ResolutionManager
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060001D2 RID: 466 RVA: 0x000102DC File Offset: 0x0000E4DC
	// (remove) Token: 0x060001D3 RID: 467 RVA: 0x00010310 File Offset: 0x0000E510
	public static event Action<float> ResolutionChanged;

	// Token: 0x060001D4 RID: 468 RVA: 0x00003238 File Offset: 0x00001438
	public static void SetResolution(int width, int height, bool fullscreen)
	{
		Action<float> resolutionChanged = ResolutionManager.ResolutionChanged;
		if (resolutionChanged != null)
		{
			resolutionChanged((float)width / (float)height);
		}
		Screen.SetResolution(width, height, fullscreen);
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x00010344 File Offset: 0x0000E544
	public static void ToggleFullscreen()
	{
		bool flag = !Screen.fullScreen;
		int width;
		int height;
		if (flag)
		{
			Resolution[] resolutions = Screen.resolutions;
			Resolution resolution = resolutions[0];
			for (int i = 0; i < resolutions.Length; i++)
			{
				if (resolution.height < resolutions[i].height)
				{
					resolution = resolutions[i];
				}
			}
			width = resolution.width;
			height = resolution.height;
		}
		else
		{
			width = 711;
			height = 400;
		}
		ResolutionManager.SetResolution(width, height, flag);
	}
}
