using System.Linq;
using UnityEngine;

public class ResolutionSlider : MonoBehaviour
{
	private int targetIdx;

	private Resolution targetResolution;

	private bool targetFullscreen;

	private bool targetVirtualSync;

	private Resolution[] allResolutions;

	public SlideBar slider;

	public ToggleButtonBehaviour Fullscreen;

	private ToggleButtonBehaviour VSync;

	public TextRenderer Display;

	public GameObject VirtualSync;

	private void Start()
	{
		targetVirtualSync = SaveManager.EnableVSync;
		FindPrefab();
		if (VirtualSync)
		{
            VSync = VirtualSync.GetComponent<ToggleButtonBehaviour>();
			var Button = VirtualSync.GetComponent<PassiveButton>();
			Button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
			Button.OnClick.AddListener(ToggleVSync);
			VSync.BaseText = "VSync";
			VSync.UpdateText(targetVirtualSync);
			VSync.transform.localPosition = new Vector3(VSync.transform.localPosition.x, -1.5f, VSync.transform.localPosition.z);
		}
		
	}

	private void FindPrefab()
	{
		if (VirtualSync) return;
		var resources = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(GameObject));
		if (resources != null)
		{
			foreach (var item in resources)
			{
				if (item.name == "FullScreenButton")
				{
					VirtualSync = GameObject.Instantiate<GameObject>(item as GameObject, base.transform);
					return;
				}
			}
		}
	}

	public void OnEnable()
	{
		allResolutions = Screen.resolutions.Where(delegate(Resolution r)
		{
			Resolution resolution = r;
			return resolution.height > 480;
		}).ToArray();
		targetFullscreen = Screen.fullScreen;
		targetResolution = (Screen.fullScreen ? Screen.currentResolution : new Resolution
		{
			width = Screen.width,
			height = Screen.height,
			refreshRate = Screen.currentResolution.refreshRate
		});
		targetIdx = allResolutions.IndexOf((Resolution e) => e.width == targetResolution.width && e.height == targetResolution.height);
		slider.Value = (float)targetIdx / ((float)allResolutions.Length - 1f);
		Display.Text = $"{targetResolution.width}x{targetResolution.height}";
		Fullscreen.UpdateText(targetFullscreen);
	}
	public void ToggleVSync()
	{
		targetVirtualSync = !targetVirtualSync;
        VSync.UpdateText(targetVirtualSync);
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
		ResolutionManager.SetVSync(targetVirtualSync);
	}
}
