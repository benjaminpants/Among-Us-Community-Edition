using System;
using System.Linq;
using UnityEngine;

// Token: 0x020001C1 RID: 449
public class ResolutionSlider : MonoBehaviour
{
	// Token: 0x060009C5 RID: 2501 RVA: 0x000334E8 File Offset: 0x000316E8
	public void OnEnable()
	{
		this.allResolutions = (from r in Screen.resolutions
		where r.height > 480
		select r).ToArray<Resolution>();
		this.targetResolution = Screen.currentResolution;
		this.targetFullscreen = Screen.fullScreen;
		this.targetIdx = this.allResolutions.IndexOf((Resolution e) => e.width == this.targetResolution.width && e.height == this.targetResolution.height);
		this.slider.Value = (float)this.targetIdx / ((float)this.allResolutions.Length - 1f);
		this.Display.Text = string.Format("{0}x{1}", this.targetResolution.width, this.targetResolution.height);
		this.Fullscreen.UpdateText(this.targetFullscreen);
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x00007EA8 File Offset: 0x000060A8
	public void ToggleFullscreen()
	{
		this.targetFullscreen = !this.targetFullscreen;
		this.Fullscreen.UpdateText(this.targetFullscreen);
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x000335C4 File Offset: 0x000317C4
	public void OnResChange(SlideBar slider)
	{
		int num = Mathf.RoundToInt((float)(this.allResolutions.Length - 1) * slider.Value);
		if (num != this.targetIdx)
		{
			this.targetIdx = num;
			this.targetResolution = this.allResolutions[num];
			this.Display.Text = string.Format("{0}x{1}", this.targetResolution.width, this.targetResolution.height);
		}
		slider.Value = (float)this.targetIdx / ((float)this.allResolutions.Length - 1f);
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x00007ECA File Offset: 0x000060CA
	public void SaveChange()
	{
		ResolutionManager.SetResolution(this.targetResolution.width, this.targetResolution.height, this.targetFullscreen);
	}

	// Token: 0x04000962 RID: 2402
	private int targetIdx;

	// Token: 0x04000963 RID: 2403
	private Resolution targetResolution;

	// Token: 0x04000964 RID: 2404
	private bool targetFullscreen;

	// Token: 0x04000965 RID: 2405
	private Resolution[] allResolutions;

	// Token: 0x04000966 RID: 2406
	public SlideBar slider;

	// Token: 0x04000967 RID: 2407
	public ToggleButtonBehaviour Fullscreen;

	// Token: 0x04000968 RID: 2408
	public TextRenderer Display;
}
