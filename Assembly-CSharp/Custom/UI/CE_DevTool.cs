using System.Linq;
using UnityEngine;

public class CE_DevTool : MonoBehaviour
{
	private Vector2 scrollPosition;

	private static CE_DevTool instance;

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
			GUILayout.Window(-7, new Rect(Screen.width / 2, 0f, Screen.width / 2, Screen.height), GlobalSettingsMenu, "");
		}
	}

	public static void Init()
	{
		if (!(instance != null))
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<CE_DevTool>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}

	public static float DevFloat1 = 0;
	public static float DevFloat2 = 0;
	public static float DevFloat3 = 0;
	public static float DevFloat4 = 0;
	public static float DevFloat5 = 0;
	public static float DevFloat6 = 0;
	public static float DevFloat7 = 0;
	public static float DevFloat8 = 0;
	public static float DevFloat9 = 0;
	public static float DevFloat10 = 0;
	private static string DevFloat1S;
	private static string DevFloat2S;
	private static string DevFloat3S;
	private static string DevFloat4S;
	private static string DevFloat5S;
	private static string DevFloat6S;
	private static string DevFloat7S;
	private static string DevFloat8S;
	private static string DevFloat9S;
	private static string DevFloat10S;

	private static int TabIndex = 0;


	private int CurrentTabUI(int LastTab)
    {
		GUILayout.BeginHorizontal();
        if (GUILayout.Button("Value Shifter")) LastTab = 0;
		if (GUILayout.Button("Loaded Roles")) LastTab = 1;
		if (GUILayout.Button("Loaded Objects")) LastTab = 2;
		GUILayout.EndHorizontal();
		CE_CommonUI.CreateSeperator();
		return LastTab;

	}

	private void LoadedRoleListTabPage()
    {
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
		if (CE_RoleManager.Roles != null)
        {
			RoleLister("ID", "Role");
			CE_CommonUI.CreateSeperator();
			foreach (var entry in CE_RoleManager.Roles)
			{
				RoleLister(entry.Value.UUID, entry.Value.RoleName);
			}
		}

		GUILayout.EndScrollView();
	}

	private void ElementsListerTabPage()
    {
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
		GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
		foreach (GameObject go in allObjects)
        {
			GUILayout.BeginHorizontal();
			try
			{
				RoleLister(go.name, go.GetComponent<MeshRenderer>().material.mainTexture.name);
				if (CE_CommonUI.CreateSimpleBoolSwitch(false))
				{
					PlayerControl.LocalPlayer.NetTransform.SnapTo(new Vector2(go.transform.position.x, go.transform.position.y));
				}
			}
			catch
            {
			
            }

			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
	}

	private void ValueShifterTabPage()
    {
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
		DevFloat1 = ValueShifter("DevFloat1", ref DevFloat1S, DevFloat1);
		DevFloat2 = ValueShifter("DevFloat2", ref DevFloat2S, DevFloat2);
		DevFloat3 = ValueShifter("DevFloat3", ref DevFloat3S, DevFloat3);
		DevFloat4 = ValueShifter("DevFloat4", ref DevFloat4S, DevFloat4);
		DevFloat5 = ValueShifter("DevFloat5", ref DevFloat5S, DevFloat5);
		DevFloat6 = ValueShifter("DevFloat6", ref DevFloat6S, DevFloat6);
		DevFloat7 = ValueShifter("DevFloat7", ref DevFloat7S, DevFloat7);
		DevFloat8 = ValueShifter("DevFloat8", ref DevFloat8S, DevFloat8);
		DevFloat9 = ValueShifter("DevFloat9", ref DevFloat9S, DevFloat9);
		DevFloat10 = ValueShifter("DevFloat10", ref DevFloat10S, DevFloat10);
		GUILayout.EndScrollView();
	}

	private void GlobalSettingsMenu(int windowID)
	{
		TabIndex = CurrentTabUI(TabIndex);
		if (TabIndex == 0) ValueShifterTabPage();
		else if (TabIndex == 1) LoadedRoleListTabPage();
		else if (TabIndex == 2) ElementsListerTabPage();
	}

	private void RoleLister(string num, string name)
    {
		GUILayout.SelectionGrid(0, new string[]{ num, name }, 3);
	}

	private float ValueShifter(string name, ref string typed_value, float current_value)
    {
		GUILayout.Label("Name:" + name);
		GUILayout.Label("Current Value:" + current_value.ToString());
		GUILayout.BeginHorizontal();
		typed_value = GUILayout.TextField(typed_value, new GUILayoutOption[0]);
		if (GUILayout.Button("UPDATE"))
		{
			if (float.TryParse(typed_value, out float new_value))
			{
				current_value = new_value;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("-"))
		{
			current_value -= 1f;
		}
		if (GUILayout.Button("+"))
		{
			current_value += 1f;
		}
		GUILayout.EndHorizontal();
		return current_value;
	}

	public void Update()
	{
		if (CE_Input.CE_GetKeyDown(KeyCode.F12))
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

	static CE_DevTool()
	{
	}
}
