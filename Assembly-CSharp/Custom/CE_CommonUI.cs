using UnityEngine;

public class CE_CommonUI
{
	private static Texture2D _staticRectTexture;

	private static GUIStyle _staticRectStyle;

	private static bool PlayHoverSound;

	private static Rect LastHoverRect;

	public static Rect FullWindowRect;

	public static bool GameSettingsChanged;

	public static bool[] ColapsableGroupStateCollection;

	public static GUIStyle UpDownSettingButtons()
	{
		return new GUIStyle(GUI.skin.button)
		{
			fixedWidth = 50f,
			fixedHeight = 50f,
			fontSize = 25,
			normal = 
			{
				textColor = Color.white
			}
		};
	}

	public static GUIStyle UpDownSettingLabel(float width = 250f)
	{
		if (width == 0f)
		{
			return new GUIStyle(GUI.skin.label)
			{
				fixedHeight = 50f,
				fixedWidth = width,
				fontSize = 25,
				normal = 
				{
					textColor = Color.white
				}
			};
		}
		return new GUIStyle(GUI.skin.label)
		{
			alignment = TextAnchor.MiddleCenter,
			fixedHeight = 50f,
			fixedWidth = width,
			fontSize = 25,
			normal = 
			{
				textColor = Color.white
			}
		};
	}

	public static GUIStyle WindowStyle(int w, int h)
	{
		return new GUIStyle(GUI.skin.window)
		{
			normal = 
			{
				background = CE_TextureNSpriteExtensions.MakeTex(1, 1, Color.black)
			}
		};
	}

	public static void GUIDrawRect(Rect position, Color color)
	{
		if (_staticRectTexture == null)
		{
			_staticRectTexture = new Texture2D(1, 1);
		}
		if (_staticRectStyle == null)
		{
			_staticRectStyle = new GUIStyle();
		}
		_staticRectTexture.SetPixel(0, 0, color);
		_staticRectTexture.Apply();
		_staticRectStyle.normal.background = _staticRectTexture;
		GUI.Box(position, GUIContent.none, _staticRectStyle);
	}

	static CE_CommonUI()
	{
		FullWindowRect = new Rect(0f, 0f, Screen.width, Screen.height);
		ColapsableGroupStateCollection = new bool[4];
	}

	public static void HoverSoundTrigger()
	{
		Rect lastRect = GUILayoutUtility.GetLastRect();
		if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
		{
			if (lastRect != LastHoverRect)
			{
				LastHoverRect = lastRect;
				SoundManager.Instance.PlaySoundImmediate(CE_UIHelpers.HoverSound, loop: false);
			}
			PlayHoverSound = false;
		}
	}

	public static void ClickSoundTrigger()
	{
		SoundManager.Instance.PlaySoundImmediate(CE_UIHelpers.ClickSound, loop: false);
	}

	public static bool CreateExitButton()
	{
		bool result = false;
		if (GUILayout.Button("Back", new GUIStyle(GUI.skin.button)
		{
			fixedHeight = 50f,
			fontSize = 25
		}))
		{
			ClickSoundTrigger();
			result = true;
		}
		HoverSoundTrigger();
		return result;
	}

	public static Rect GameSettingsRect()
	{
		float num = (float)Screen.width / 12f;
		float num2 = (float)Screen.height / 8f;
		float width = (float)Screen.width - num * 2f;
		float height = (float)Screen.height - num2 * 2f;
		return new Rect(num, num2, width, height);
	}

	public static Rect StockSettingsRect()
	{
		float num = (float)Screen.width / 12f;
		float num2 = (float)Screen.height / 54f;
		float width = (float)Screen.width - num * 2f;
		float height = (float)Screen.height - num2 * 2f;
		return new Rect(num, num2, width, height);
	}

	public static void CreateHeaderLabel(string text)
	{
		GUILayout.Label(text, new GUIStyle(GUI.skin.label)
		{
			fixedHeight = 60f,
			fontSize = 30,
			fontStyle = FontStyle.Bold
		});
	}

	public static bool CreateCollapsable(string name, int index)
	{
		bool flag = ColapsableGroupStateCollection[index];
		if (GUILayout.Button(name, new GUIStyle(GUI.skin.button)
		{
			fixedHeight = 50f,
			fontSize = 25,
			alignment = TextAnchor.MiddleCenter
		}))
		{
			ClickSoundTrigger();
			flag = !ColapsableGroupStateCollection[index];
		}
		HoverSoundTrigger();
		GUI.backgroundColor = new Color(0.531f, 0.649f, 0.539f);
		ColapsableGroupStateCollection[index] = flag;
		return flag;
	}

	public static void CreateSeperator()
	{
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.normal.background = Texture2D.whiteTexture;
		gUIStyle.margin = new RectOffset(0, 0, 4, 4);
		gUIStyle.fixedHeight = 1f;
		Color color = GUI.color;
		GUI.color = Color.white;
		GUILayout.Box(GUIContent.none, gUIStyle);
		GUI.color = color;
	}

	public static void UpdateSettings()
	{
		PlayerControl.GameOptions.isDefaults = false;
		GameSettingsChanged = true;
	}

	public static void SyncSettings()
	{
		if (GameSettingsChanged)
		{
			PlayerControl localPlayer = PlayerControl.LocalPlayer;
			if (!(localPlayer == null))
			{
				localPlayer.RpcSyncSettings(PlayerControl.GameOptions);
			}
		}
	}

	public static int CreateStringPicker(int value, string[] valueNames, int min, int max, string title, bool gameSettings = false)
	{
		using (new GUILayout.HorizontalScope())
		{
			GUILayout.Label(title, UpDownSettingLabel(0f));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("-", UpDownSettingButtons()))
			{
				ClickSoundTrigger();
				if (value != min)
				{
					value--;
					if (title != "Map" && gameSettings)
					{
						UpdateSettings();
					}
				}
			}
			HoverSoundTrigger();
			GUILayout.Label(valueNames[value], UpDownSettingLabel());
			if (GUILayout.Button("+", UpDownSettingButtons()))
			{
				ClickSoundTrigger();
				if (value != max)
				{
					value++;
					if (title != "Map" && gameSettings)
					{
						UpdateSettings();
					}
				}
			}
			HoverSoundTrigger();
			return value;
		}
	}

	public static bool CreateBoolButton(bool value, string Title, bool gameSettings = false)
	{
		using (new GUILayout.HorizontalScope())
		{
			GUILayout.Label(Title, UpDownSettingLabel(0f));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(value ? "☑" : "☐", UpDownSettingButtons()))
			{
				ClickSoundTrigger();
				value = !value;
				if (gameSettings)
				{
					UpdateSettings();
				}
			}
			HoverSoundTrigger();
			return value;
		}
	}

	public static float CreateValuePicker(float value, float incrementAmount, float min, float max, string title, string subString, bool decmialView = false, bool gameSettings = false)
	{
		using (new GUILayout.HorizontalScope())
		{
			GUILayout.Label(title, UpDownSettingLabel(0f));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("-", UpDownSettingButtons()))
			{
				ClickSoundTrigger();
				if (value > min)
				{
					value -= incrementAmount;
					if (gameSettings)
					{
						UpdateSettings();
					}
				}
				else value = min;
			}
			HoverSoundTrigger();
			GUILayout.Label((decmialView ? $"{value:0.##}" : value.ToString()) + subString, UpDownSettingLabel());
			if (GUILayout.Button("+", UpDownSettingButtons()))
			{
				ClickSoundTrigger();
				if (value < max)
				{
					value += incrementAmount;
					if (gameSettings)
					{
						UpdateSettings();
					}
				}
				else value = max;
			}
			HoverSoundTrigger();
			return value;
		}
	}

	public static int CreateStringPickerG(int value, string[] valueNames, int min, int max, string title)
	{
		return CreateStringPicker(value, valueNames, min, max, title, gameSettings: true);
	}

	public static bool CreateBoolButtonG(bool value, string Title)
	{
		return CreateBoolButton(value, Title, gameSettings: true);
	}

	public static float CreateValuePickerG(float value, float incrementAmount, float min, float max, string title, string subString, bool decmialView = false)
	{
		return CreateValuePicker(value, incrementAmount, min, max, title, subString, decmialView, gameSettings: true);
	}

	public static void HorizontalLine(Color color)
	{
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.normal.background = Texture2D.whiteTexture;
		gUIStyle.margin = new RectOffset(0, 0, 4, 4);
		gUIStyle.fixedHeight = 1f;
		Color color2 = GUI.color;
		GUI.color = color;
		GUILayout.Box(GUIContent.none, gUIStyle);
		GUI.color = color2;
	}

	public static void CreateButtonLabel(string buttonName, string firstText, string nextText)
	{
		using (new GUILayout.HorizontalScope())
		{
			GUILayout.Label(firstText, new GUIStyle(GUI.skin.label)
			{
				fixedHeight = 40f,
				fontSize = 20,
				fontStyle = FontStyle.Bold
			});
			GUILayout.FlexibleSpace();
			GUILayout.Label(nextText, new GUIStyle(GUI.skin.label)
			{
				fixedHeight = 40f,
				fontSize = 20,
				fontStyle = FontStyle.Normal
			});
		}
		HorizontalLine(Color.white);
	}
}
