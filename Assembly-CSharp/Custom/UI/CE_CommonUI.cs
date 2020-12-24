using UnityEngine;

public class CE_CommonUI
{
	private static Texture2D _staticRectTexture;

	private static GUIStyle _staticRectStyle;

	private static bool PlayHoverSound;

	private static Rect LastHoverRect;

	private static int TextUpscale = -15;
	private static int TextHeightUpscale = 20;


	public static Rect FullWindowRect;

	public static bool GameSettingsChanged;
	public static Texture2D SpecialClrChip;

	public static bool[] ColapsableGroupStateCollection;

	private static Texture2D ButtonTexture;

	private static Texture2D ButtonSelected;

	private static Texture2D MenuTexture;

	private static Texture2D GameButtonTexture;

	private static Texture2D GameButtonSelected;

	private static Texture2D GameMenuTexture;

	private static Texture2D GameScrollViewBackground;

	private static Texture2D GameScrollViewBackground2;

	private static Texture2D GameScrollViewBackground3;

	private static Texture2D GameMenuDropdownBGTexture;

	private static Texture2D GameMenuDropdownTexture;

	private static Texture2D GameMenuDropdownSelectedTexture;

	private static Texture2D GameMenuDropdownOpenTexture;

	private static Texture2D GameMenuDropdownOpenSelectedTexture;

	private static void LoadAssets()
    {
		if (!MenuTexture)
        {
			MenuTexture = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "MenuOptionsBG.png"));
		}
		if (!ButtonTexture)
        {
			ButtonTexture = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "MenuOptionsButton.png"));
		}
		if (!ButtonSelected)
		{
			ButtonSelected = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "MenuOptionsButtonS.png"));
		}
		if (!GameMenuTexture)
		{
			GameMenuTexture = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "GameOptionsBG.png"));
		}
		if (!GameButtonTexture)
		{
			GameButtonTexture = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "GameOptionsButton.png"));
		}
		if (!ButtonSelected)
		{
			GameButtonSelected = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "GameOptionsButtonS.png"));
		}
		if (!GameScrollViewBackground)
		{
			GameScrollViewBackground = CE_TextureNSpriteExtensions.MakeTex(1, 1, new Color(0.459f, 0.545f, 0.467f));
		}
		if (!GameScrollViewBackground2)
		{
			GameScrollViewBackground2 = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "GameOptionRow.png"));
		}
		if (!GameScrollViewBackground3)
		{
			GameScrollViewBackground3 = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "GameOptionRow_Selected.png"));
		}
		if (!GameMenuDropdownBGTexture)
		{
			GameMenuDropdownBGTexture = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "GameOptionsDropdownBG.png"));
		}
		if (!GameMenuDropdownOpenTexture)
		{
			GameMenuDropdownOpenTexture = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "GameOptionsDropdownOpen.png"));
		}
		if (!GameMenuDropdownOpenSelectedTexture)
		{
			GameMenuDropdownOpenSelectedTexture = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "GameOptionsDropdownOpen_Selected.png"));
		}
		if (!GameMenuDropdownTexture)
		{
			GameMenuDropdownTexture = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "GameOptionsDropdown.png"));
		}
        if (!GameMenuDropdownSelectedTexture)
        {
            GameMenuDropdownSelectedTexture = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "GameOptionsDropdown_Selected.png"));
        }
		if (!SpecialClrChip)
		{
			SpecialClrChip = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "colorchip_special.png"));
		}
	}


	public static GUIStyle WindowStyle(bool GameWindow = false)
	{
		LoadAssets();
		float scale = GetScale(Screen.width, Screen.height);
		var style = new GUIStyle(GUI.skin.window)
		{
			normal =
			{
				background = (GameWindow ? GameMenuTexture : MenuTexture)
			},
			focused =
            {
				background = (GameWindow ? GameMenuTexture : MenuTexture)
			},
			active =
			{
				background = (GameWindow ? GameMenuTexture : MenuTexture)
			},
			hover =
            {
				background = (GameWindow ? GameMenuTexture : MenuTexture)
			}
			
		};
		style.padding = new RectOffset(30, 30, 30, 30);
		style.border = new RectOffset(15, 15, 15, 15);
		style.onNormal.background = (GameWindow ? GameMenuTexture : MenuTexture);
		return style;
	}

	public static GUIStyle GameDropDownBGStyle()
    {
		var style = new GUIStyle()
		{
			normal =
			{
				textColor = Color.white,
				background = GameMenuDropdownBGTexture
			},
			focused =
			{
				textColor = Color.white,
				background = GameMenuDropdownBGTexture
			},
			active =
			{
				textColor = Color.white,
				background = GameMenuDropdownBGTexture
			},
			hover =
			{
				textColor = Color.white,
				background = GameMenuDropdownBGTexture
			}
		};
		style.padding = new RectOffset(15, 15, 15, 15);
		style.border = new RectOffset(15, 15, 15, 15);
		style.onNormal.background = GameMenuDropdownBGTexture;
		return style;
	}

	public static GUIStyle HorizontalScopeStyle(bool gameSettings = false, bool readOnly = false)
    {
		if (gameSettings)
		{
			var style = new GUIStyle()
			{
				normal =
			{
				textColor = Color.white,
				background = GameScrollViewBackground2
			},
				focused =
			{
				textColor = Color.white,
				background = GameScrollViewBackground2
			},
				active =
			{
				textColor = Color.white,
				background = GameScrollViewBackground2
			},
				hover =
			{
				textColor = Color.white,
				background = readOnly ? GameScrollViewBackground2 : GameScrollViewBackground3
			}
			};
			style.padding = new RectOffset(15, 15, 15, 15);
			style.border = new RectOffset(15, 15, 15, 15);
			style.onNormal.background = GameScrollViewBackground2;
			return style;
		}
		else return new GUIStyle();
    }

	public static GUIStyle UpDownSettingButtons(bool gameSettings = false)
	{
		float scale = GetScale(Screen.width, Screen.height);
		var style = new GUIStyle(GUI.skin.button)
		{
			fixedWidth = (50f + TextHeightUpscale) * scale,
			fixedHeight = (50f + TextHeightUpscale) * scale,
			fontSize = (int)((45 + TextUpscale) * scale),
			normal = 
			{
				textColor = Color.white,
				background = (gameSettings ? GameButtonTexture : ButtonTexture)
			},
			focused =
			{
				textColor = Color.white,
				background = (gameSettings ? GameButtonTexture :ButtonTexture)
			},
			active =
			{
				textColor = Color.white,
				background = (gameSettings ? GameButtonTexture :ButtonTexture)
			},
			hover =
			{
				textColor = Color.white,
				background = (gameSettings ? GameButtonSelected :ButtonSelected)
			}
		};
		style.border = new RectOffset(15, 15, 15, 15);
		style.onNormal.background = ButtonTexture;
		return style;
	}

	public static GUIStyle UpDownSettingLabel(float width = 250f)
	{
		float scale = GetScale(Screen.width, Screen.height);
		if (width == 0f)
		{
			return new GUIStyle(GUI.skin.label)
			{
				fixedHeight = (50f + TextHeightUpscale) * scale,
				fixedWidth = width * scale,
				fontSize = (int)((45 + TextUpscale) * scale),
				normal = 
				{
					textColor = Color.white
				}
			};
		}
		return new GUIStyle(GUI.skin.label)
		{
			alignment = TextAnchor.MiddleCenter,
			fixedHeight = (50f + TextHeightUpscale) * scale,
			fixedWidth = width * scale,
			fontSize = (int)((45 + TextUpscale) * scale),
			normal = 
			{
				textColor = Color.white
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

	public static GUIStyle GameScrollbarStyleV()
	{
		return new GUIStyle(GUI.skin.verticalScrollbar);
	}

	public static GUIStyle GameScrollbarStyleH()
	{
		return new GUIStyle(GUI.skin.horizontalScrollbar);
	}

	public static GUIStyle GameScrollViewStyle()
	{
		var style = new GUIStyle(GUI.skin.scrollView)
		{
			normal =
			{
				background = GameScrollViewBackground
			},
			focused =
			{
				background = GameScrollViewBackground
			},
			active =
			{
				background = GameScrollViewBackground
			},
			hover =
			{
				background = GameScrollViewBackground
			}
		};
		style.padding = new RectOffset(10, 10, 10, 10);
		style.onNormal.background = GameScrollViewBackground;
		return style;
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
		ColapsableGroupStateCollection = new bool[8];
		LoadAssets();
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

	public static bool CreateExitButton(bool gameSettings = false)
	{
		float scale = GetScale(Screen.width, Screen.height);
		bool result = false;


		var style = new GUIStyle(GUI.skin.button)
		{
			fixedHeight = (50f + TextHeightUpscale) * scale,
			fontSize = (int)((45 + TextUpscale) * scale),
			alignment = TextAnchor.MiddleCenter,
			normal =
			{
				textColor = Color.white,
				background = (gameSettings ? GameMenuDropdownTexture : ButtonTexture)
			},
			focused =
			{
				textColor = Color.white,
				background = (gameSettings ? GameMenuDropdownTexture :ButtonTexture)
			},
			active =
			{
				textColor = Color.white,
				background = (gameSettings ? GameMenuDropdownTexture :ButtonTexture)
			},
			hover =
			{
				textColor = Color.white,
				background = (gameSettings ? GameMenuDropdownSelectedTexture :ButtonSelected)
			}
		};
		style.border = new RectOffset(15, 15, 15, 15);
		style.onNormal.background = ButtonTexture;

		if (CE_CustomUIElements.Button("Back", style))
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

		CE_CustomUIElements.Label(text, new GUIStyle(GUI.skin.label)
		{
			fixedHeight = (60f + TextHeightUpscale) * scale,
			fontSize = (int)((50 + TextUpscale) * scale),
			fontStyle = FontStyle.Bold
		});
	}

	public static bool CreateCollapsable(string name, int index, bool gameSettings = false)
	{
		float scale = GetScale(Screen.width, Screen.height);
		bool flag = ColapsableGroupStateCollection[index];

		var texture = (flag ? GameMenuDropdownOpenTexture : (gameSettings ? GameMenuDropdownTexture : ButtonTexture));
		var sel_texture = (flag ? GameMenuDropdownOpenSelectedTexture : (gameSettings ? GameMenuDropdownSelectedTexture : ButtonSelected));

		var style =  new GUIStyle(GUI.skin.button)
		{
			fixedHeight = (50f + TextHeightUpscale) * scale,
			fontSize = (int)((45 + TextUpscale) * scale),
			alignment = TextAnchor.MiddleCenter,
			normal =
			{
				textColor = Color.white,
				background = texture
			},
			focused =
			{
				textColor = Color.white,
				background = texture
			},
			active =
			{
				textColor = Color.white,
				background = texture
			},
			hover =
			{
				textColor = Color.white,
				background = sel_texture
			}
		};
		style.border = new RectOffset(15, 15, 15, 15);
		style.margin = new RectOffset(0, 0, 0, 0);
		style.onNormal.background = (gameSettings ? GameButtonTexture : ButtonTexture);
		if (CE_CustomUIElements.Button(name, style))
		{
			ClickSoundTrigger();
			flag = !ColapsableGroupStateCollection[index];
		}
		HoverSoundTrigger();
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

	public static int CreateStringPicker(int value, string[] valueNames, int min, int max, string title, bool gameSettings = false, bool readOnly = false)
	{
		var last_value = value;
		using (new GUILayout.HorizontalScope(HorizontalScopeStyle(gameSettings, readOnly)))
		{
			CE_CustomUIElements.Label(title, UpDownSettingLabel(0f));
			GUILayout.FlexibleSpace();
			if (CE_CustomUIElements.Button(readOnly ? "" : "-", UpDownSettingButtons(gameSettings)))
			{
				if (!readOnly) ClickSoundTrigger();
				if (value != min)
				{
					value--;
					if (title != "Map" && gameSettings)
					{
						UpdateSettings();
					}
				}
			}
			if (!readOnly) HoverSoundTrigger();
			CE_CustomUIElements.Label(valueNames[value], UpDownSettingLabel());
			if (CE_CustomUIElements.Button(readOnly ? "" : "+", UpDownSettingButtons(gameSettings)))
			{
				if (!readOnly) ClickSoundTrigger();
				if (value != max)
				{
					value++;
					if (title != "Map" && gameSettings)
					{
						UpdateSettings();
					}
				}
			}
			if (!readOnly) HoverSoundTrigger();
			if (readOnly) return last_value;
			return value;
		}
	}

	public static bool CreateBoolButton(bool value, string Title, bool gameSettings = false, bool readOnly = false)
	{
		var last_value = value;
		using (new GUILayout.HorizontalScope(HorizontalScopeStyle(gameSettings, readOnly)))
		{
			CE_CustomUIElements.Label(Title, UpDownSettingLabel(0f));
			GUILayout.FlexibleSpace();

			string checkbox_style_A = value ? "✓" : " ";
			string checkbox_style_B = value ? "☑" : "☐";

			if (CE_CustomUIElements.Button(gameSettings ? checkbox_style_B : checkbox_style_A, UpDownSettingButtons(gameSettings)))
			{
				if (!readOnly) ClickSoundTrigger();
				value = !value;
				if (gameSettings)
				{
					UpdateSettings();
				}
			}
			if (!readOnly) HoverSoundTrigger();
			if (readOnly) return last_value;
			return value;
		}
	}

	public static float CreateValuePicker(float value, float incrementAmount, float min, float max, string title, string subString, bool decmialView = false, bool gameSettings = false, bool readOnly = false)
	{
		float last_value = value;
		using (new GUILayout.HorizontalScope(HorizontalScopeStyle(gameSettings, readOnly)))
		{
			CE_CustomUIElements.Label(title, UpDownSettingLabel(0f));
			GUILayout.FlexibleSpace();
			if (CE_CustomUIElements.Button(readOnly ? "" : "-", UpDownSettingButtons(gameSettings)))
			{
				if (!readOnly) ClickSoundTrigger();
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
			if (!readOnly) HoverSoundTrigger();
			CE_CustomUIElements.Label((decmialView ? $"{value:0.##}" : value.ToString()) + subString, UpDownSettingLabel());
			if (CE_CustomUIElements.Button(readOnly ? "" : "+", UpDownSettingButtons(gameSettings)))
			{
				if (!readOnly) ClickSoundTrigger();
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
			if (!readOnly) HoverSoundTrigger();
			if (readOnly) return last_value;
			return value;
		}
	}

	public static int CreateStringPickerG(int value, string[] valueNames, int min, int max, string title, bool readOnly = false)
	{
		return CreateStringPicker(value, valueNames, min, max, title, gameSettings: true, readOnly);
	}

	public static bool CreateBoolButtonG(bool value, string Title, bool readOnly = false)
	{
		return CreateBoolButton(value, Title, gameSettings: true, readOnly);
	}

	public static float CreateValuePickerG(float value, float incrementAmount, float min, float max, string title, string subString, bool decmialView = false, bool readOnly = false)
	{
		return CreateValuePicker(value, incrementAmount, min, max, title, subString, decmialView, gameSettings: true, readOnly);
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
			CE_CustomUIElements.Label(firstText, new GUIStyle(GUI.skin.label)
			{
				fixedHeight = (40f + TextHeightUpscale) * scale,
				fontSize = (int)((40 + TextUpscale) * scale),
				fontStyle = FontStyle.Bold
			});
			GUILayout.FlexibleSpace();
			CE_CustomUIElements.Label(nextText, new GUIStyle(GUI.skin.label)
			{
				fixedHeight = (40f + TextHeightUpscale) * scale,
				fontSize = (int)((40 + TextUpscale) * scale),
				fontStyle = FontStyle.Normal
			});
		}
		HorizontalLine(Color.white);
	}
}
