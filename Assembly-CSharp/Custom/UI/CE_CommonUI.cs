using UnityEngine;

public class CE_CommonUI
{
	static CE_CommonUI()
	{
		LoadAssets();
	}

	#region Textures / Assets

	private static Texture2D ButtonTexture;

	private static Texture2D ButtonSelected;

	private static Texture2D MenuTexture;

	public static Texture2D TXT_Texture;

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

    private static Texture2D CloseButtonTexture;

	public static Texture2D ModsButton;

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
		if (!CloseButtonTexture)
		{
			CloseButtonTexture = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "UI", "CloseButton.png"));
		}
        if (!TXT_Texture)
        {
            TXT_Texture = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "TXTBackground.png"));
        }
		if (!ModsButton)
		{
			ModsButton = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "mods_texture.png"));
		}
	}

	#endregion

	#region UI Scaling Methods

	public static int TextUpscale = -15;
	public static int TextHeightUpscale = 20;
	public static float GetScale(int width, int height, Vector2 scalerReferenceResolution, float scalerMatchWidthOrHeight)
	{
		return Mathf.Pow(width / scalerReferenceResolution.x, 1f - scalerMatchWidthOrHeight) *
			   Mathf.Pow(height / scalerReferenceResolution.y, scalerMatchWidthOrHeight);
	}
	public static float GetScale(int width, int height)
    {
		return GetScale(width, height, new Vector2(1920, 1080), 1f);
    }

	#endregion

	#region UI Sound FX Methods

	private static bool InScrollView = false;
	public static Rect WindowHoverBounds;
	private static Rect CurrentScrollViewSize;
	private static Rect LastHoverRect;

	public static void HoverSoundTrigger()
	{
		Rect lastRect = GUILayoutUtility.GetLastRect();
		bool IsRepaint = Event.current.type == EventType.Repaint;
		bool IsHoveringOver = GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
		if (IsRepaint && IsHoveringOver)
		{
			if (lastRect != LastHoverRect)
			{
				LastHoverRect = lastRect;
				SoundManager.Instance.PlaySoundImmediate(CE_UIHelpers.HoverSound, loop: false);
			}
		}
	}
	public static void ClickSoundTrigger()
	{
		SoundManager.Instance.PlaySoundImmediate(CE_UIHelpers.ClickSound, loop: false);
	}

	#endregion

	#region Game Settings Controls / Styles

	public static bool GameSettingsChanged;
	public static bool[] ColapsableGroupStateCollection_GS = new bool[8];

	public static GUIStyle WindowStyle_GS()
	{
		LoadAssets();
		float scale = GetScale(Screen.width, Screen.height);
		var style = new GUIStyle(GUI.skin.window)
		{
			normal =
			{
				background = GameMenuTexture
			},
			focused =
			{
				background = GameMenuTexture
			},
			active =
			{
				background = GameMenuTexture
			},
			hover =
			{
				background = GameMenuTexture
			}

		};
		style.padding = new RectOffset(30, 30, 30, 30);
		style.border = new RectOffset(15, 15, 15, 15);
		style.onNormal.background = GameMenuTexture;
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
	public static GUIStyle HorizontalScopeStyle_GS(bool readOnly = false)
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
	public static GUIStyle UpDownSettingButtons_GS()
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
				background = GameButtonTexture
			},
			focused =
			{
				textColor = Color.white,
				background = GameButtonTexture
			},
			active =
			{
				textColor = Color.white,
				background = GameButtonTexture
			},
			hover =
			{
				textColor = Color.white,
				background = GameButtonSelected
			}
		};
		style.border = new RectOffset(15, 15, 15, 15);
		style.onNormal.background = ButtonTexture;
		return style;
	}
	public static Rect GameSettingsRect()
	{
		float scale = GetScale(Screen.width, Screen.height);

		int extended_width = 200;

		int desired_width = 990 + extended_width;
		int desired_height = 810;

		float width = desired_width * scale;
		float height = desired_height * scale;

		float x = (Screen.width - width) / 2;
		float y = (Screen.height - height) / 2;

		return new Rect(x, y, width, height);
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
	public static bool CreateCollapsable_GS(string name, int index)
	{
		float scale = GetScale(Screen.width, Screen.height);
		bool flag = ColapsableGroupStateCollection_GS[index];

		var texture = (flag ? GameMenuDropdownOpenTexture : GameMenuDropdownTexture);
		var sel_texture = (flag ? GameMenuDropdownOpenSelectedTexture : GameMenuDropdownSelectedTexture);

		var style = new GUIStyle(GUI.skin.button)
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
		style.onNormal.background = GameButtonTexture;
		if (CE_CustomUIElements.Button(name, style))
		{
			ClickSoundTrigger();
			flag = !ColapsableGroupStateCollection_GS[index];
		}
		HoverSoundTrigger();
		ColapsableGroupStateCollection_GS[index] = flag;
		return flag;
	}
	public static int CreateStringPicker_GS(int value, string[] valueNames, int min, int max, string title, bool readOnly = false)
	{
		var last_value = value;
		using (new GUILayout.HorizontalScope(HorizontalScopeStyle_GS(readOnly)))
		{
			CE_CustomUIElements.Label(title, UpDownSettingLabel(0f));
			GUILayout.FlexibleSpace();
			if (!readOnly && CE_CustomUIElements.Button(readOnly ? "" : "-", UpDownSettingButtons_GS()))
			{
				if (!readOnly) ClickSoundTrigger();
				if (value != min)
				{
					value--;
					if (title != "Map")
					{
						UpdateSettings_GS();
					}
				}
			}
			if (!readOnly) HoverSoundTrigger();
			try
			{
				CE_CustomUIElements.Label(valueNames[value], UpDownSettingLabel());
			}
			catch
			{
				CE_CustomUIElements.Label("Invalid", UpDownSettingLabel());
			}
			if (!readOnly && CE_CustomUIElements.Button(readOnly ? "" : "+", UpDownSettingButtons_GS()))
			{
				if (!readOnly) ClickSoundTrigger();
				if (value != max)
				{
					value++;
					if (title != "Map")
					{
						UpdateSettings_GS();
					}
				}
			}
			if (!readOnly) HoverSoundTrigger();
			if (readOnly) return last_value;
			return value;
		}
	}
	public static bool CreateBoolButton_GS(bool value, string Title, bool readOnly = false)
	{
		var last_value = value;
		using (new GUILayout.HorizontalScope(HorizontalScopeStyle_GS(readOnly)))
		{
			if (readOnly)
            {
				Title = Title + (value ? "					☑" : "					☐"); //This is a placeholder solution, CarJem please add a proper fix.
			}
			CE_CustomUIElements.Label(Title, UpDownSettingLabel(0f));
			GUILayout.FlexibleSpace();

			string checkbox_style_B = value ? "☑" : "☐";

			if (!readOnly && CE_CustomUIElements.Button(checkbox_style_B, UpDownSettingButtons_GS()))
			{
				if (!readOnly) ClickSoundTrigger();
				value = !value;
				UpdateSettings_GS();
			}
			if (!readOnly) HoverSoundTrigger();
			if (readOnly) return last_value;
			return value;
		}
	}
	public static float CreateValuePicker_GS(float value, float incrementAmount, float min, float max, string title, string subString, bool decmialView = false, bool readOnly = false)
	{
		float last_value = value;
		using (new GUILayout.HorizontalScope(HorizontalScopeStyle_GS(readOnly)))
		{
			CE_CustomUIElements.Label(title, UpDownSettingLabel(0f));
			GUILayout.FlexibleSpace();
			if (!readOnly && CE_CustomUIElements.Button(readOnly ? "" : "-", UpDownSettingButtons_GS()))
			{
				if (!readOnly) ClickSoundTrigger();
				if (value > min)
				{
					value -= incrementAmount;
					UpdateSettings_GS();
				}
				else value = min;
			}
			if (!readOnly) HoverSoundTrigger();
			CE_CustomUIElements.Label((decmialView ? $"{value:0.##}" : value.ToString()) + subString, UpDownSettingLabel());
			if (!readOnly && CE_CustomUIElements.Button(readOnly ? "" : "+", UpDownSettingButtons_GS()))
			{
				if (!readOnly) ClickSoundTrigger();
				if (value < max)
				{
					value += incrementAmount;
					UpdateSettings_GS();
				}
				else value = max;
			}
			if (!readOnly) HoverSoundTrigger();
			if (readOnly) return last_value;
			return value;
		}
	}
	public static void UpdateSettings_GS()
	{
		PlayerControl.GameOptions.isDefaults = false;
		GameSettingsChanged = true;
	}
	public static void SyncSettings_GS()
	{
		if (GameSettingsChanged)
		{
			PlayerControl localPlayer = PlayerControl.LocalPlayer;
			if (!(localPlayer == null))
			{
				localPlayer.RpcSyncSettings(PlayerControl.GameOptions);
				GameSettingsChanged = false; //reset this
			}
		}
	}

	#endregion

	#region General Controls / Styles

	public static Rect FullWindowRect = new Rect(0f, 0f, Screen.width, Screen.height);
	private static GUIStyle _SolidRectStyle;
	private static Texture2D _SolidRectTexture;

	public static bool CreateCloseButton(Rect WindowSize)
    {
		LoadAssets();
		float scale = GetScale(Screen.width, Screen.height);
		var style = new GUIStyle();
		style.padding = new RectOffset(0, 0, 0, 0);

		float width = CloseButtonTexture.width * 1.25f * scale;
		float height = CloseButtonTexture.height * 1.25f * scale;

		GUI.DrawTexture(new Rect(WindowSize.x - width, WindowSize.y, width, height), CloseButtonTexture);
		return GUI.Button(new Rect(WindowSize.x - width, WindowSize.y, width, height), GUIContent.none, style);
    }
	public static void GUIDrawRect(Rect position, Color color)
	{
		if (_SolidRectTexture == null)
		{
			_SolidRectTexture = new Texture2D(1, 1);
		}
		if (_SolidRectStyle == null)
		{
			_SolidRectStyle = new GUIStyle();
		}
		_SolidRectTexture.SetPixel(0, 0, color);
		_SolidRectTexture.Apply();
		_SolidRectStyle.normal.background = _SolidRectTexture;
		GUI.Box(position, GUIContent.none, _SolidRectStyle);
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
	public static bool CreateSimpleBoolSwitch(bool value)
	{
		string checkbox_style = value ? "☑" : "☐";

		if (CE_CustomUIElements.Button(checkbox_style, GUI.skin.button))
		{
			ClickSoundTrigger();
			value = !value;
		}
		HoverSoundTrigger();
		return value;
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
	public static void CreateHeaderLabel(string text, FontStyle style = FontStyle.Bold)
	{
		float scale = GetScale(Screen.width, Screen.height);

		CE_CustomUIElements.Label(text, new GUIStyle(GUI.skin.label)
		{
			fixedHeight = (60f + TextHeightUpscale) * scale,
			fontSize = (int)((50 + TextUpscale) * scale),
			fontStyle = style
		});
	}

	#endregion

	#region Options Menu Controls / Styles

	public static bool[] ColapsableGroupStateCollection = new bool[8];
	public static GUIStyle WindowStyle()
	{
		LoadAssets();
		float scale = GetScale(Screen.width, Screen.height);
		var style = new GUIStyle(GUI.skin.window)
		{
			normal =
			{
				background = MenuTexture
			},
			focused =
			{
				background = MenuTexture
			},
			active =
			{
				background = MenuTexture
			},
			hover =
			{
				background = MenuTexture
			}

		};
		style.padding = new RectOffset(30, 30, 30, 30);
		style.border = new RectOffset(15, 15, 15, 15);
		style.onNormal.background = MenuTexture;
		return style;
	}
	public static GUIStyle WindowStyle_TXT()
	{
		LoadAssets();
		float scale = GetScale(Screen.width, Screen.height);
		var style = new GUIStyle(GUI.skin.window)
		{
			normal =
			{
				background = TXT_Texture
			},
			focused =
			{
				background = TXT_Texture
			},
			active =
			{
				background = TXT_Texture
			},
			hover =
			{
				background = TXT_Texture
			}

		};
		style.onNormal.background = TXT_Texture;
		style.padding = new RectOffset(30, 30, 30, 30);
		style.border = new RectOffset(15, 15, 15, 15);
		style.onNormal.background = TXT_Texture;
		return style;
	}
	public static Rect StockSettingsRect()
	{
		float scale = GetScale(Screen.width, Screen.height);

		int extended_width = 200;

		int desired_width = 996 + extended_width;
		int desired_height = 1040;

		float width = desired_width * scale;
		float height = desired_height * scale;

		float x = (Screen.width - width) / 2;
		float y = (Screen.height - height) / 2;

		return new Rect(x, y, width, height);
	}
	public static GUIStyle HorizontalScopeStyle()
	{
		return new GUIStyle();
	}
    public static GUIStyle UpDownSettingButtons()
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
                background = ButtonTexture
            },
            focused =
            {
                textColor = Color.white,
                background = ButtonTexture
            },
            active =
            {
                textColor = Color.white,
                background = ButtonTexture
            },
            hover =
            {
                textColor = Color.white,
                background = ButtonSelected
            }
        };
        style.border = new RectOffset(15, 15, 15, 15);
        style.onNormal.background = ButtonTexture;
        return style;
    }

	public static GUIStyle ScreenLengthButton()
	{
		float scale = GetScale(Screen.width, Screen.height);
		var style = new GUIStyle(GUI.skin.button)
		{
			fixedWidth =  360f * scale,
			fixedHeight = (50f + TextHeightUpscale) * scale,
			fontSize = (int)((45 + TextUpscale) * scale),
			normal =
			{
				textColor = Color.white,
				background = ButtonTexture
			},
			focused =
			{
				textColor = Color.white,
				background = ButtonTexture
			},
			active =
			{
				textColor = Color.white,
				background = ButtonTexture
			},
			hover =
			{
				textColor = Color.white,
				background = ButtonSelected
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
	public static bool CreateCollapsable(string name, int index)
	{
		float scale = GetScale(Screen.width, Screen.height);
		bool flag = ColapsableGroupStateCollection[index];

		var texture = (flag ? GameMenuDropdownOpenTexture : ButtonTexture);
		var sel_texture = (flag ? GameMenuDropdownOpenSelectedTexture : ButtonSelected);

		var style = new GUIStyle(GUI.skin.button)
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
		style.onNormal.background = ButtonTexture;
		if (CE_CustomUIElements.Button(name, style))
		{
			ClickSoundTrigger();
			flag = !ColapsableGroupStateCollection[index];
		}
		HoverSoundTrigger();
		ColapsableGroupStateCollection[index] = flag;
		return flag;
	}
	public static int CreateStringPicker(int value, string[] valueNames, int min, int max, string title)
	{
		var last_value = value;
		using (new GUILayout.HorizontalScope(HorizontalScopeStyle()))
		{
			CE_CustomUIElements.Label(title, UpDownSettingLabel(0f));
			GUILayout.FlexibleSpace();
			if (CE_CustomUIElements.Button("-", UpDownSettingButtons()))
			{
				ClickSoundTrigger();
				if (value != min)
				{
					value--;
				}
			}
			HoverSoundTrigger();
			try
			{
				CE_CustomUIElements.Label(valueNames[value], UpDownSettingLabel());
			}
			catch
            {
				CE_CustomUIElements.Label("Invalid", UpDownSettingLabel());
			}
			if (CE_CustomUIElements.Button("+", UpDownSettingButtons()))
			{
				ClickSoundTrigger();
				if (value != max)
				{
					value++;
				}
			}
			HoverSoundTrigger();
			return value;
		}
	}
    public static bool CreateBoolButton(bool value, string Title)
    {
        var last_value = value;
        using (new GUILayout.HorizontalScope(HorizontalScopeStyle()))
        {
            CE_CustomUIElements.Label(Title, UpDownSettingLabel(0f));
            GUILayout.FlexibleSpace();

            string checkbox_style_A = value ? "✓" : " ";

            if (CE_CustomUIElements.Button(checkbox_style_A, UpDownSettingButtons()))
            {
                ClickSoundTrigger();
                value = !value;
            }
            HoverSoundTrigger();
            return value;
        }
    }

	public static bool CreateBoolButtonNoCheck(bool value, string check, string uncheck)
	{
		var last_value = value;
		using (new GUILayout.HorizontalScope(HorizontalScopeStyle()))
		{
			GUILayout.FlexibleSpace();

			string checkbox_style_A = value ? check : uncheck;

			if (CE_CustomUIElements.Button(checkbox_style_A, ScreenLengthButton()))
			{
				ClickSoundTrigger();
				value = !value;
			}
			HoverSoundTrigger();
			return value;
		}
	}

	public static float CreateValuePicker(float value, float incrementAmount, float min, float max, string title, string subString, bool decmialView = false)
	{
		float last_value = value;
		using (new GUILayout.HorizontalScope(HorizontalScopeStyle()))
		{
			CE_CustomUIElements.Label(title, UpDownSettingLabel(0f));
			GUILayout.FlexibleSpace();
			if (CE_CustomUIElements.Button("-", UpDownSettingButtons()))
			{
				ClickSoundTrigger();
				if (value > min)
				{
					value -= incrementAmount;
				}
				else value = min;
			}
			HoverSoundTrigger();
			CE_CustomUIElements.Label(title == "FPS Cap" ? value == max ? "Uncapped" : (decmialView ? $"{value:0.##}" : value.ToString()) : (decmialView ? $"{value:0.##}" : value.ToString()) + subString, UpDownSettingLabel());
			if (CE_CustomUIElements.Button("+", UpDownSettingButtons()))
			{
				ClickSoundTrigger();
				if (value < max)
				{
					value += incrementAmount;
				}
				else value = max;
			}
			HoverSoundTrigger();
			return value;
		}
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
	public static bool CreateExitButton()
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
				background = ButtonTexture
			},
			focused =
			{
				textColor = Color.white,
				background = ButtonTexture
			},
			active =
			{
				textColor = Color.white,
				background = ButtonTexture
			},
			hover =
			{
				textColor = Color.white,
				background = ButtonSelected
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

	#endregion
}
