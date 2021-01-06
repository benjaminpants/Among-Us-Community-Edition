using System.Linq;
using UnityEngine;
using System.IO;

public class CE_DevMinigame : MonoBehaviour
{
	private Vector2 scrollPosition;

	private static CE_DevMinigame instance;

	public static bool IsShown;

	private static int HatIndex;

	public static string TextDocument;

	private static string TextDocumentPath = System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Dev", "DeveloperTodo.txt");

	private void Load()
    {
		Directory.CreateDirectory(System.IO.Path.GetDirectoryName(TextDocumentPath));
		if (!File.Exists(TextDocumentPath)) File.Create(TextDocumentPath);
		TextDocument = File.ReadAllText(TextDocumentPath);
	}

	private void Save()
    {
		File.WriteAllText(TextDocumentPath, TextDocument);
	}

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
		Load();
	}

	private void OnGUI()
	{
		if (IsShown)
		{
			CE_CommonUI.WindowHoverBounds = GUILayout.Window(-5, CE_CommonUI.StockSettingsRect(), WindowMenu, "", CE_CommonUI.WindowStyle_TXT());
			if (CE_CommonUI.CreateCloseButton(CE_CommonUI.StockSettingsRect()))
			{
				Save();
				IsShown = false;
			}
		}
	}

	public static void Init()
	{
		if (!(instance != null))
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<CE_DevMinigame>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}

	private GUIStyle TXTStyle()
    {
		float scale = CE_CommonUI.GetScale(Screen.width, Screen.height);
		var style =  new GUIStyle(GUI.skin.textArea)
		{
			fontSize = (int)((42f + CE_CommonUI.TextUpscale) * scale),
			normal =
			{
				background = CE_CommonUI.TXT_Texture,
				textColor = Color.black
			},
			focused =
			{
				background = CE_CommonUI.TXT_Texture,
				textColor = Color.black
			},
			active =
			{
				background = CE_CommonUI.TXT_Texture,
				textColor = Color.black
			},
			hover =
			{
				background = CE_CommonUI.TXT_Texture,
				textColor = Color.black
			}

		};
		style.onNormal.textColor = Color.black;
		style.border = new RectOffset(15, 15, 15, 15);
		style.padding = new RectOffset(15, 15, 15, 15);
		style.onNormal.background = CE_CommonUI.TXT_Texture;
		return style;
	}

	private void WindowMenu(int windowID)
	{
		var lastColor = GUI.skin.settings.cursorColor;
		GUI.skin.settings.cursorColor = Color.black;
		TextDocument = GUILayout.TextArea(TextDocument, TXTStyle(), new GUILayoutOption[] { GUILayout.ExpandHeight(true) });
		GUI.skin.settings.cursorColor = lastColor;
	}

	public void Update()
	{

	}

	static CE_DevMinigame()
	{
	}
}
