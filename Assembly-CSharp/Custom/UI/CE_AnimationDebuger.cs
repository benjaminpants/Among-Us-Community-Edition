using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

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

    private string PivotXTemp;
	private string PivotYTemp;
	private bool UpdateValues = false;

	public bool CreatePauseBoolButton(bool value, string Title)
	{
		using (new GUILayout.HorizontalScope())
		{
			GUILayout.Label(Title, CE_CommonUI.UpDownSettingLabel(0f));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(value ? "☑" : "☐", CE_CommonUI.UpDownSettingButtons()))
			{
				CE_CommonUI.ClickSoundTrigger();
				value = !value;
				UpdateValues = true;
			}
			CE_CommonUI.HoverSoundTrigger();
			return value;
		}
	}

	private void GlobalSettingsMenu(int windowID)
	{
		CE_UIHelpers.LoadCommonAssets();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
		CE_WardrobeLoader.AnimationEditor_Active = CE_CommonUI.CreateBoolButton(CE_WardrobeLoader.AnimationEditor_Active, "Enabled");
		CE_WardrobeLoader.AnimationEditor_Speed = CE_CommonUI.CreateValuePicker(CE_WardrobeLoader.AnimationEditor_Speed, 0.01f, 0f, float.MaxValue, "Animation Speed", "x");
		CE_WardrobeLoader.AnimationEditor_Speed = CE_CommonUI.CreateValuePicker(CE_WardrobeLoader.AnimationEditor_Speed, 0.1f, 0f, float.MaxValue, "Animation Speed (Large Shift)", "x");
		CE_WardrobeLoader.AnimationEditor_Mode = (int)CE_CommonUI.CreateValuePicker(CE_WardrobeLoader.AnimationEditor_Mode, 1f, 0f, 5f, "Playback Mode", "");
        CE_WardrobeLoader.AnimationEditor_IsPaused = CreatePauseBoolButton(CE_WardrobeLoader.AnimationEditor_IsPaused, "Pause");
		GUILayout.Label("Last Frame Name:" + CE_WardrobeLoader.AnimationEditor_LastFrame);
		GUILayout.Label("Last Pivot X: " + CE_WardrobeLoader.AnimationEditor_LastPivotX.ToString());
		GUILayout.BeginHorizontal();
		if (!CE_WardrobeLoader.AnimationEditor_Paused || UpdateValues)
		{
			UpdateValues = false;
		}
		PivotXTemp = GUILayout.TextField(PivotXTemp, new GUILayoutOption[0]);
		if (GUILayout.Button("UPDATE"))
		{
			if (float.TryParse(PivotXTemp, out float newPivotX))
			{
				CE_WardrobeLoader.SetCurrentFramePivotX(newPivotX);
				UpdateValues = true;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("+X Pivot"))
		{
			CE_WardrobeLoader.NudgeCurrentFramePivot(1, 0);
			UpdateValues = true;
		}
		if (GUILayout.Button("-X Pivot"))
		{
			CE_WardrobeLoader.NudgeCurrentFramePivot(-1, 0);
			UpdateValues = true;
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("Last Pivot Y: " + CE_WardrobeLoader.AnimationEditor_LastPivotY.ToString());
		GUILayout.BeginHorizontal();
		PivotYTemp = GUILayout.TextField(PivotYTemp, new GUILayoutOption[0]);
		if (GUILayout.Button("UPDATE"))
        {
			if (float.TryParse(PivotYTemp, out float newPivotY))
			{ 
				CE_WardrobeLoader.SetCurrentFramePivotY(newPivotY);
				UpdateValues = true;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("+Y Pivot"))
		{
			CE_WardrobeLoader.NudgeCurrentFramePivot(0, 1);
			UpdateValues = true;
		}
		if (GUILayout.Button("-Y Pivot"))
		{
			CE_WardrobeLoader.NudgeCurrentFramePivot(0, -1);
			UpdateValues = true;
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("Pause at:");
		CE_WardrobeLoader.AnimationEditor_PauseAt = GUILayout.TextArea(CE_WardrobeLoader.AnimationEditor_PauseAt);
		if (GUILayout.Button("Next Frame"))
		{
			var prefix = Regex.Match(CE_WardrobeLoader.AnimationEditor_PauseAt, "^\\D+").Value;
			var number = Regex.Replace(CE_WardrobeLoader.AnimationEditor_PauseAt, "^\\D+", "");
			var i = int.Parse(number) + 1;
			CE_WardrobeLoader.AnimationEditor_PauseAt = prefix + i.ToString(new string('0', number.Length));
			CE_WardrobeLoader.AnimationEditor_IsPaused = false;
			UpdateValues = true;
		}
		GUILayout.Space(15);
		if (GUILayout.Button("Reload Sprites"))
		{
			DestroyableSingleton<HatManager>.Instance.ReloadCustomHatsAndSkins();
		}
		GUILayout.Space(15);

		if (GUILayout.Button("Save Current"))
		{
			CE_WardrobeLoader.SaveCurrentSkin();
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
				CE_WardrobeLoader.NudgeCurrentFramePivot(0, -1);
			}
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				CE_WardrobeLoader.NudgeCurrentFramePivot(0, 1);
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				CE_WardrobeLoader.NudgeCurrentFramePivot(-1, 0);
			}
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				CE_WardrobeLoader.NudgeCurrentFramePivot(1, 0);
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
