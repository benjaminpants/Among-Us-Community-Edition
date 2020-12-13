using System.Linq;
using UnityEngine;

public class ResolutionSlider : MonoBehaviour
{
	private int targetIdx;

	private Resolution targetResolution;

	private bool targetFullscreen;

	private Resolution[] allResolutions;

	public SlideBar slider;

	public ToggleButtonBehaviour Fullscreen;

	public TextRenderer Display;

	public void OnEnable()
	{
		allResolutions = Screen.resolutions.Where((Resolution r) => r.height > 480).ToArray();
		targetResolution = Screen.currentResolution;
		targetFullscreen = Screen.fullScreen;
		targetIdx = allResolutions.IndexOf((Resolution e) => e.width == targetResolution.width && e.height == targetResolution.height);
		slider.Value = (float)targetIdx / ((float)allResolutions.Length - 1f);
		Display.Text = $"{targetResolution.width}x{targetResolution.height}";
		Fullscreen.UpdateText(targetFullscreen);
	}

	public void ToggleFullscreen()
	{
		targetFullscreen = !targetFullscreen;
		Fullscreen.UpdateText(targetFullscreen);
	}

	public void OnResChange(SlideBar slider)
	{
		int num = Mathf.RoundToInt((float)(allResolutions.Length - 1) * slider.Value);
		if (num != targetIdx)
		{
			targetIdx = num;
			targetResolution = allResolutions[num];
			Display.Text = $"{targetResolution.width}x{targetResolution.height}";
		}
		slider.Value = (float)targetIdx / ((float)allResolutions.Length - 1f);
	}

	public void SaveChange()
	{
		ResolutionManager.SetResolution(targetResolution.width, targetResolution.height, targetFullscreen);
	}
}
