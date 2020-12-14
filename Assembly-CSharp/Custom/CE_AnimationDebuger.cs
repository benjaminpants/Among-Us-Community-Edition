using System.Linq;
using UnityEngine;

public class CE_AnimationDebuger : MonoBehaviour
{
	private Vector2 scrollPosition;

	private static CE_AnimationDebuger instance;

	public static bool IsShown;

	private static int HatIndex;

	private void OnEnable()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Object.Destroy(this);
		}
	}

	private void OnGUI()
	{
		if (IsShown)
		{
			GUILayout.Window(-6, CE_CommonUI.FullWindowRect, GlobalSettingsMenu, "");
		}
	}

	public static void Init()
	{
		if (!(instance != null))
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<CE_AnimationDebuger>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}

	private void GlobalSettingsMenu(int windowID)
	{
		float testPlaybackSpeed = CE_WardrobeLoader.TestPlaybackSpeed;
		CE_UIHelpers.LoadCommonAssets();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
		CE_WardrobeLoader.AnimationTestingActive = CE_CommonUI.CreateBoolButton(CE_WardrobeLoader.AnimationTestingActive, "Enabled");
		CE_WardrobeLoader.TestPlaybackSpeed = CE_CommonUI.CreateValuePicker(CE_WardrobeLoader.TestPlaybackSpeed, 0.1f, 1f, float.MaxValue, "Animation Speed", "x");
		CE_WardrobeLoader.TestPlaybackMode = (int)CE_CommonUI.CreateValuePicker(CE_WardrobeLoader.TestPlaybackMode, 1f, 0f, 5f, "Playback Mode", "");
		CE_WardrobeLoader.TestPlaybackPause = CE_CommonUI.CreateBoolButton(CE_WardrobeLoader.TestPlaybackPause, "Pause Playback");
		HatIndex = (int)CE_CommonUI.CreateValuePicker(HatIndex, 1f, 0f, 13f, "Hat Index", "");
		CE_WardrobeLoader.HatPivotPoints[HatIndex] = CE_CommonUI.CreateValuePicker(CE_WardrobeLoader.HatPivotPoints[HatIndex], 0.1f, float.MinValue, float.MaxValue, "Pivot", "");
		CE_WardrobeLoader.HatPivotPoints[HatIndex] = CE_CommonUI.CreateValuePicker(CE_WardrobeLoader.HatPivotPoints[HatIndex], 0.01f, float.MinValue, float.MaxValue, "Pivot", "");
		GUILayout.Label("Current Position: " + CE_WardrobeLoader.TestPlaybackCurrentPosition);
		GUILayout.Label("Current Position (Skin): " + CE_WardrobeLoader.TestPlaybackCurrentPositionSkin);
		if (GUILayout.Button("Kill Overlay (As Killer)"))
		{
			PlayerControl playerControl = PlayerControl.AllPlayerControls.Where((PlayerControl x) => x != PlayerControl.LocalPlayer).FirstOrDefault();
			DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowOne(PlayerControl.LocalPlayer, playerControl.Data);
		}
		if (GUILayout.Button("Kill Overlay (As Victim)"))
		{
			PlayerControl killer = PlayerControl.AllPlayerControls.Where((PlayerControl x) => x != PlayerControl.LocalPlayer).FirstOrDefault();
			DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowOne(killer, PlayerControl.LocalPlayer.Data);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndScrollView();
		if (CE_CommonUI.CreateExitButton())
		{
			IsShown = false;
		}
		if (testPlaybackSpeed != CE_WardrobeLoader.TestPlaybackSpeed)
		{
			CE_WardrobeLoader.TestPlaybackResetAnimations = true;
		}
		GUI.color = Color.black;
		GUI.backgroundColor = Color.black;
		GUI.contentColor = Color.white;
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.F3))
		{
			DestroyableSingleton<HatManager>.Instance.ReloadCustomHatsAndSkins();
		}
		if (Input.GetKeyDown(KeyCode.F5) && SaveManager.EnableAnimationTestingMode)
		{
			if (IsShown)
			{
				IsShown = false;
			}
			else
			{
				IsShown = true;
			}
		}
	}

	static CE_AnimationDebuger()
	{
	}
}
