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
		float scale = GetScale(Screen.width, Screen.height);
		return new GUIStyle(GUI.skin.button)
		{
			fixedWidth = 50f * scale,
			fixedHeight = 50f * scale,
			fontSize = 25 * (int)scale,
			normal = 
			{
				textColor = Color.white
			}
		};
	}

	public static GUIStyle UpDownSettingLabel(float width = 250f)
	{
		float scale = GetScale(Screen.width, Screen.height);
		if (width == 0f)
		{
			return new GUIStyle(GUI.skin.label)
			{
				fixedHeight = 50f * scale,
				fixedWidth = width * scale,
				fontSize = 25 * (int)scale,
				normal = 
				{
					textColor = Color.white
				}
			};
		}
		return new GUIStyle(GUI.skin.label)
		{
			alignment = TextAnchor.MiddleCenter,
			fixedHeight = 50f * scale,
			fixedWidth = width * scale,
			fontSize = 25 * (int)scale,
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

	private static float GetScale(int width, int height, Vector2 scalerReferenceResolution, float scalerMatchWidthOrHeight)
	{
		return Mathf.Pow(width / scalerReferenceResolution.x, 1f - scalerMatchWidthOrHeight) *
			   Mathf.Pow(height / scalerReferenceResolution.y, scalerMatchWidthOrHeight);
	}

	private static float GetScale(int width, int height)
    {
		return GetScale(width, height, new Vector2(1920, 1080), 1f);
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
		float scale = GetScale(Screen.width, Screen.height);
		bool result = false;
		if (GUILayout.Button("Back", new GUIStyle(GUI.skin.button)
		{
			fixedHeight = 50f * scale,
			fontSize = 25 * (int)scale
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
		float scale = GetScale(Screen.width, Screen.height);

		int desired_width = 990;
		int desired_height = 810;

		float width = desired_width * scale;
		float height = desired_height * scale;

		float x = (Screen.width - width) / 2;
		float y = (Screen.height - height) / 2;

		return new Rect(x, y, width, height);
	}

	public static Rect StockSettingsRect()
	{
		float scale = GetScale(Screen.width, Screen.height);

		int desired_width = 996;
		int desired_height = 1040;

		float width = desired_width * scale;
		float height = desired_height * scale;

		float x = (Screen.width - width) / 2;
		float y = (Screen.height - height) / 2;

		return new Rect(x, y, width, height);
	}

	public static void CreateHeaderLabel(string text)
	{
		float scale = GetScale(Screen.width, Screen.height);

		GUILayout.Label(text, new GUIStyle(GUI.skin.label)
		{
			fixedHeight = 60f * scale,
			fontSize = 30 * (int)scale,
			fontStyle = FontStyle.Bold
		});
	}

	public static bool CreateCollapsable(string name, int index)
	{
		float scale = GetScale(Screen.width, Screen.height);
		bool flag = ColapsableGroupStateCollection[index];
		if (GUILayout.Button(name, new GUIStyle(GUI.skin.button)
		{
			fixedHeight = 50f * scale,
			fontSize = 25 * (int)scale,
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
		float scale = GetScale(Screen.width, Screen.height);

		using (new GUILayout.HorizontalScope())
		{
			GUILayout.Label(firstText, new GUIStyle(GUI.skin.label)
			{
				fixedHeight = 40f * scale,
				fontSize = 20 * (int)scale,
				fontStyle = FontStyle.Bold
			});
			GUILayout.FlexibleSpace();
			GUILayout.Label(nextText, new GUIStyle(GUI.skin.label)
			{
				fixedHeight = 40f * scale,
				fontSize = 20 * (int)scale,
				fontStyle = FontStyle.Normal
			});
		}
		HorizontalLine(Color.white);
	}
}
