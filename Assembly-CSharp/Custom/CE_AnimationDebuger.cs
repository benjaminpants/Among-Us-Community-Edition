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
			GUILayout.Window(-6, new Rect(Screen.width / 2, 0f, Screen.width / 2, Screen.height), GlobalSettingsMenu, "");
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
		float testPlaybackSpeed = CE_WardrobeLoader.AnimationEditor_Speed;
		CE_UIHelpers.LoadCommonAssets();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
		CE_WardrobeLoader.AnimationEditor_Active = CE_CommonUI.CreateBoolButton(CE_WardrobeLoader.AnimationEditor_Active, "Enabled");
		CE_WardrobeLoader.AnimationEditor_Speed = CE_CommonUI.CreateValuePicker(CE_WardrobeLoader.AnimationEditor_Speed, 0.1f, 0f, float.MaxValue, "Animation Speed", "x");
		CE_WardrobeLoader.AnimationEditor_Mode = (int)CE_CommonUI.CreateValuePicker(CE_WardrobeLoader.AnimationEditor_Mode, 1f, 0f, 5f, "Playback Mode", "");
        CE_WardrobeLoader.AnimationEditor_Pause = CE_CommonUI.CreateBoolButton(CE_WardrobeLoader.AnimationEditor_Pause, "Pause");
		GUILayout.Label("Last Frame Name:" + CE_WardrobeLoader.AnimationEditor_LastFrame);
		GUILayout.Label("Last Pivot X:" + CE_WardrobeLoader.AnimationEditor_LastPivotX);
		GUILayout.Label("Last Pivot Y:" + CE_WardrobeLoader.AnimationEditor_LastPivotY);
		GUILayout.Label("Pause at:");
		CE_WardrobeLoader.AnimationEditor_PauseAt = GUILayout.TextArea(CE_WardrobeLoader.AnimationEditor_PauseAt);
		GUILayout.Space(15);
		if (GUILayout.Button("Reload Sprites"))
		{
			DestroyableSingleton<HatManager>.Instance.ReloadCustomHatsAndSkins();
		}
		GUILayout.Space(15);
		if (GUILayout.Button("+X Pivot"))
		{
			CE_WardrobeLoader.SetCurrentFramePivot(1, 0);
		}
		if (GUILayout.Button("-X Pivot"))
		{
			CE_WardrobeLoader.SetCurrentFramePivot(-1, 0);
		}
		if (GUILayout.Button("+Y Pivot"))
		{
			CE_WardrobeLoader.SetCurrentFramePivot(0, 1);
		}
		if (GUILayout.Button("-Y Pivot"))
		{
			CE_WardrobeLoader.SetCurrentFramePivot(0, -1);
		}
		GUILayout.Space(15);
		if (GUILayout.Button("Stab (As Killer)"))
		{
			PlayerControl playerControl = PlayerControl.AllPlayerControls.Where((PlayerControl x) => x != PlayerControl.LocalPlayer).FirstOrDefault();
			var animation = DestroyableSingleton<HudManager>.Instance.KillOverlay.KillAnims[0];
			DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowOne(animation, PlayerControl.LocalPlayer, playerControl.Data);
		}
		if (GUILayout.Button("Neck (As Killer)"))
		{
			PlayerControl playerControl = PlayerControl.AllPlayerControls.Where((PlayerControl x) => x != PlayerControl.LocalPlayer).FirstOrDefault();
			var animation = DestroyableSingleton<HudManager>.Instance.KillOverlay.KillAnims[1];
			DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowOne(animation, PlayerControl.LocalPlayer, playerControl.Data);
		}
		if (GUILayout.Button("Alien (As Killer)"))
		{
			PlayerControl playerControl = PlayerControl.AllPlayerControls.Where((PlayerControl x) => x != PlayerControl.LocalPlayer).FirstOrDefault();
			var animation = DestroyableSingleton<HudManager>.Instance.KillOverlay.KillAnims[2];
			DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowOne(animation, PlayerControl.LocalPlayer, playerControl.Data);
		}
		if (GUILayout.Button("Shoot (As Killer)"))
		{
			PlayerControl playerControl = PlayerControl.AllPlayerControls.Where((PlayerControl x) => x != PlayerControl.LocalPlayer).FirstOrDefault();
			var animation = DestroyableSingleton<HudManager>.Instance.KillOverlay.KillAnims[3];
			DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowOne(animation, PlayerControl.LocalPlayer, playerControl.Data);
		}
		if (GUILayout.Button("Stab (As Victim)"))
		{
			PlayerControl killer = PlayerControl.AllPlayerControls.Where((PlayerControl x) => x != PlayerControl.LocalPlayer).FirstOrDefault();
			var animation = DestroyableSingleton<HudManager>.Instance.KillOverlay.KillAnims[0];
			DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowOne(animation, killer, PlayerControl.LocalPlayer.Data);
		}
		if (GUILayout.Button("Neck (As Victim)"))
		{
			PlayerControl killer = PlayerControl.AllPlayerControls.Where((PlayerControl x) => x != PlayerControl.LocalPlayer).FirstOrDefault();
			var animation = DestroyableSingleton<HudManager>.Instance.KillOverlay.KillAnims[1];
			DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowOne(animation, killer, PlayerControl.LocalPlayer.Data);
		}
		if (GUILayout.Button("Alien (As Victim)"))
		{
			PlayerControl killer = PlayerControl.AllPlayerControls.Where((PlayerControl x) => x != PlayerControl.LocalPlayer).FirstOrDefault();
			var animation = DestroyableSingleton<HudManager>.Instance.KillOverlay.KillAnims[2];
			DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowOne(animation, killer, PlayerControl.LocalPlayer.Data);
		}
		if (GUILayout.Button("Shoot (As Victim)"))
		{
			PlayerControl killer = PlayerControl.AllPlayerControls.Where((PlayerControl x) => x != PlayerControl.LocalPlayer).FirstOrDefault();
			var animation = DestroyableSingleton<HudManager>.Instance.KillOverlay.KillAnims[3];
			DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowOne(animation, killer, PlayerControl.LocalPlayer.Data);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndScrollView();
		if (CE_CommonUI.CreateExitButton())
		{
			IsShown = false;
		}
		if (testPlaybackSpeed != CE_WardrobeLoader.AnimationEditor_Speed)
		{
			CE_WardrobeLoader.AnimationEditor_Reset = true;
		}
		GUI.color = Color.black;
		GUI.backgroundColor = Color.black;
		GUI.contentColor = Color.white;
	}

	public void Update()
	{
		if (IsShown)
        {
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				CE_WardrobeLoader.SetCurrentFramePivot(0, -1);
			}
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				CE_WardrobeLoader.SetCurrentFramePivot(0, 1);
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				CE_WardrobeLoader.SetCurrentFramePivot(-1, 0);
			}
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				CE_WardrobeLoader.SetCurrentFramePivot(1, 0);
			}
		}
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
