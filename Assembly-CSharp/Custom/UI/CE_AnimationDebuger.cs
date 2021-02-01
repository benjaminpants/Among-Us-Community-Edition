using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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
		CE_WardrobeManager.AnimationEditor_Active = CE_CommonUI.CreateBoolButton(CE_WardrobeManager.AnimationEditor_Active, "Enabled");
		CE_WardrobeManager.AnimationEditor_Speed = CE_CommonUI.CreateValuePicker(CE_WardrobeManager.AnimationEditor_Speed, 0.01f, 0f, float.MaxValue, "Animation Speed", "x");
		CE_WardrobeManager.AnimationEditor_Speed = CE_CommonUI.CreateValuePicker(CE_WardrobeManager.AnimationEditor_Speed, 0.1f, 0f, float.MaxValue, "Animation Speed (Large Shift)", "x");
		CE_WardrobeManager.AnimationEditor_Mode = (int)CE_CommonUI.CreateValuePicker(CE_WardrobeManager.AnimationEditor_Mode, 1f, 0f, 5f, "Playback Mode", "");
        CE_WardrobeManager.AnimationEditor_IsPaused = CreatePauseBoolButton(CE_WardrobeManager.AnimationEditor_IsPaused, "Pause");
		GUILayout.Label("Last Frame Name:" + CE_WardrobeManager.AnimationEditor_LastFrame);
		GUILayout.Label("Last Pivot X: " + CE_WardrobeManager.AnimationEditor_LastPivotX.ToString());
		GUILayout.BeginHorizontal();
		if (!CE_WardrobeManager.AnimationEditor_Paused || UpdateValues)
		{
			UpdateValues = false;
		}
		PivotXTemp = GUILayout.TextField(PivotXTemp, new GUILayoutOption[0]);
		if (GUILayout.Button("UPDATE"))
		{
			if (float.TryParse(PivotXTemp, out float newPivotX))
			{
				CE_WardrobeManager.SetCurrentFramePivotX(newPivotX);
				UpdateValues = true;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("+X Pivot"))
		{
			CE_WardrobeManager.NudgeCurrentFramePivot(1, 0);
			UpdateValues = true;
		}
		if (GUILayout.Button("-X Pivot"))
		{
			CE_WardrobeManager.NudgeCurrentFramePivot(-1, 0);
			UpdateValues = true;
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("Last Pivot Y: " + CE_WardrobeManager.AnimationEditor_LastPivotY.ToString());
		GUILayout.BeginHorizontal();
		PivotYTemp = GUILayout.TextField(PivotYTemp, new GUILayoutOption[0]);
		if (GUILayout.Button("UPDATE"))
        {
			if (float.TryParse(PivotYTemp, out float newPivotY))
			{ 
				CE_WardrobeManager.SetCurrentFramePivotY(newPivotY);
				UpdateValues = true;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("+Y Pivot"))
		{
			CE_WardrobeManager.NudgeCurrentFramePivot(0, 1);
			UpdateValues = true;
		}
		if (GUILayout.Button("-Y Pivot"))
		{
			CE_WardrobeManager.NudgeCurrentFramePivot(0, -1);
			UpdateValues = true;
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("Pause at:");
		CE_WardrobeManager.AnimationEditor_PauseAt = GUILayout.TextArea(CE_WardrobeManager.AnimationEditor_PauseAt);
		if (GUILayout.Button("Next Frame"))
		{
			var prefix = Regex.Match(CE_WardrobeManager.AnimationEditor_PauseAt, "^\\D+").Value;
			var number = Regex.Replace(CE_WardrobeManager.AnimationEditor_PauseAt, "^\\D+", "");
			var i = int.Parse(number) + 1;
			CE_WardrobeManager.AnimationEditor_PauseAt = prefix + i.ToString(new string('0', number.Length));
			CE_WardrobeManager.AnimationEditor_IsPaused = false;
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
			CE_WardrobeManager.SaveCurrentSkin();
		}
		GUILayout.Space(15);

		PlayerControl.LocalPlayer.Data.IsDead = CE_CommonUI.CreateBoolButton(PlayerControl.LocalPlayer.Data.IsDead, "Is Player Dead");


		GUILayout.Space(15);
		/*if (GUILayout.Button("Stab (As Killer)"))
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
		}*/
		if (GUILayout.Button("Impostor Intro Cutscene"))
		{
			DestroyableSingleton<HudManager>.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.ForceShowIntro(PlayerControl.AllPlayerControls, true));
		}
		if (GUILayout.Button("Crewmate Intro Cutscene"))
		{
			DestroyableSingleton<HudManager>.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.ForceShowIntro(PlayerControl.AllPlayerControls, false));
		}
		if (GUILayout.Button("Exile Cutscene"))
		{
			CoExileControllerTest();
		}
		if (GUILayout.Button("Victory Cutscene"))
		{
			ShipStatus.RpcEndGame(GameOverReason.HumansByTask, false);
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


	private void CoExileControllerTest()
    {
		var resources = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(ExileController));
		if (resources != null)
		{
			foreach (var item in resources)
			{
				if (item.name == "ExileCutscene")
				{
					DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.clear, Color.black, 1f);
					ExileController exileController = GameObject.Instantiate<ExileController>(item as ExileController);
					exileController.transform.SetParent(DestroyableSingleton<HudManager>.Instance.transform, worldPositionStays: false);
					exileController.transform.localPosition = new Vector3(0f, 0f, -60f);
					exileController.IsDebugging = true;
					exileController.Begin(PlayerControl.LocalPlayer.Data, false);
					PlayerControl.LocalPlayer.Data.IsDead = false;
				}
			}
		}
	}

	public void Update()
	{
		if (IsShown)
        {
			if (CE_Input.CE_GetKeyDown(KeyCode.DownArrow))
			{
				CE_WardrobeManager.NudgeCurrentFramePivot(0, -1);
			}
			if (CE_Input.CE_GetKeyDown(KeyCode.UpArrow))
			{
				CE_WardrobeManager.NudgeCurrentFramePivot(0, 1);
			}
			if (CE_Input.CE_GetKeyDown(KeyCode.LeftArrow))
			{
				CE_WardrobeManager.NudgeCurrentFramePivot(-1, 0);
			}
			if (CE_Input.CE_GetKeyDown(KeyCode.RightArrow))
			{
				CE_WardrobeManager.NudgeCurrentFramePivot(1, 0);
			}
		}
		/*if (CE_Input.CE_GetKeyDown(KeyCode.F3))
		{
			DestroyableSingleton<HatManager>.Instance.ReloadCustomHatsAndSkins();
		}*/
		//disabled the above keybind for technical reasons.
		if (CE_Input.CE_GetKeyDown(KeyCode.F5) && SaveManager.EnableAnimationTestingMode)
		{
			if ((bool)AmongUsClient.Instance)
			{
				if (AmongUsClient.Instance.GameMode == GameModes.OnlineGame)
				{
					return;
				}
			}
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
